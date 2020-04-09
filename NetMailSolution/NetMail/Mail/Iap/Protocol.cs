using CoffeeBean.Mail.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace CoffeeBean.Mail.Iap
{
    public class Protocol
	{
        protected string host;
        private TcpClient client;
        // in case we turn on TLS, we'll need these later
        protected bool quote;
        protected Util.Properties props;
        protected string prefix;
        private volatile ResponseReader reader;
        private volatile StreamWriter writer;
        private int tagCounter = 0;
        private string localHostName;
        private long timestamp;

        private readonly IList<IResponseHandler> handlers
            = new List<IResponseHandler>();

        private static readonly byte[] CRLF = { (byte)'\r', (byte)'\n' };

        public Protocol(string host, int port,
            Util.Properties props, string prefix, 
            bool isSsl)
        {
            bool connected = false; // did constructor succeed?
            try
            {

            }
            finally
            {
                /*
	             * If we get here because an exception was thrown, we need
	             * to disconnect to avoid leaving a connected socket that
	             * no one will be able to use because this object was never
	             * completely constructed.
	             */
                if (!connected)
                    disconnect();
            }
        }

        /// <summary>
        /// Returns the timestamp.
        /// </summary>
        public long TimeStamp { get { return timestamp; } }

        /// <summary>
        /// Adds a response handler.
        /// </summary>
        /// <param name="h">the response handler</param>
        public void AddResponseHandler(IResponseHandler h)
        {
            lock (this)
            {
                handlers.Add(h);
            }            
        }

        /// <summary>
        /// Removed the specified response handler.
        /// </summary>
        /// <param name="h">the response handler</param>
        public void RemoveResponseHandler(IResponseHandler h)
        {
            lock (this)
            {
                handlers.Remove(h);
            }
        }

        /// <summary>
        /// Notify response handlers
        /// </summary>
        /// <param name="responses">the Responses</param>
        public void NotifyResponseHandlers(Response[] responses)
        {
            if (handlers.IsEmpty()) return;

            foreach(var r in responses)
            {
                if (r.IsNotNull())
                {
                    foreach(var handler in handlers)
                    {
                        if (handler.IsNotNull()) handler.HandleResponse(r);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <exception cref="ProtocolException"></exception>
        protected void processGreeting(Response r)
        {
            if (r.IsBYE)
                throw new ConnectionException(this, r);
        }

        /// <summary>
        /// Return the Protocol's reader.
        /// </summary>
        /// <returns>the reader</returns>
        protected internal ResponseReader GetReader()
        {
            return reader;
        }

        /// <summary>
        /// Return the Protocol's writer.
        /// </summary>
        /// <returns>the writer</returns>
        protected internal StreamWriter GetWriter()
        {
            return writer;
        }

        /// <summary>
        /// Returns whether this Protocol supports non-synchronizing literals
        /// Default is false. Subclasses should override this if required
        /// </summary>
        /// <returns>true if the server supports non-synchronizing literals</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
		protected internal virtual bool SupportsNonSyncLiterals()
        {
            return false;
        }

        /// <summary>
        /// Is another response available in our buffer?
        /// </summary>
        /// <returns>true if another response is in the buffer</returns>
        public bool HasResponse()
        {
            try
            {
                return reader.Available() > 0;
            }
            catch (IOException) { }
            return false;
        }

        /// <summary>
        /// Return a buffer to be used to read a response.
        /// The default implementation returns null, which causes
        /// a new buffer to be allocated for every response.
        /// </summary>
        /// <returns>the buffer to use</returns>
        protected internal virtual ByteArray GetResponseBuffer()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        /// <returns></returns>
        public string WriteCommand(string command, Argument args)
        {
            string tag = "A" + Convert.ToString(tagCounter++, 10);
            try
            {
                writer.Write(string.Concat(tag, " ", command));
                if (args.IsNotNull())
                {
                    writer.Write(' ');
                    args.Write(this);
                }
                writer.Write(CRLF);
                writer.Flush();
            }
            catch (IOException) { throw; }
            catch (ProtocolException) { throw; }
            catch(Exception e) { throw new ProtocolException(e.Message, e); }
            return tag;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Response[] Command(string command, Argument args)
        {
            commandStart(command);
            IList<Response> v = new List<Response>();
            bool done = false;
            string tag = null;

            //write command
            try
            {
                tag = WriteCommand(command, args);
            }
            catch (LiteralException lex)
            {
                v.Add(lex.Response);
                done = true;
            }
            catch(Exception ex)
            {
                // Convert this into a BYE response
                v.Add(Response.ByeResponse(ex));
            }

            Response byeResp = null;
            while (!done)
            {
                Response r = null;
                try
                {
                    r = ReadResponse();
                }
                catch (IOException ioex)
                {
                    if (byeResp.IsNull()) // convert this into a BYE response
                        byeResp = Response.ByeResponse(ioex);
                    // else, connection closed after BYE was sent
                    break;
                }
                catch (ProtocolException) { continue; }

                if (r.IsBYE)
                {
                    byeResp = r;
                    continue;
                }

                v.Add(r);

                // If this is a matching command completion response, we are done
                if (r.IsTagged && r.Tag.Equals(tag))
                    done = true;
            }

            if (byeResp.IsNotNull()) v.Add(byeResp); // must be last

            Response[] responses = new Response[v.Count];
            v.CopyTo(responses, 0);
            timestamp = DateTime.Now.CurrentTimeMillis();
            commandEnd();
            return responses;
        }

        /// <summary>
        /// Convenience routine to handle OK, NO, BAD and BYE responses.
        /// </summary>
        /// <param name="response">the response</param>
        /// <exception cref="ProtocolException">for protocol failures</exception>
        public void HandleResult(Response response)
        {
            if (response.IsOK) return;
            else if (response.IsNO) throw new CommandFailedException(response);
            else if (response.IsBAD) throw new BadCommandException(response);
            else if (response.IsBYE)
            {
                disconnect();
                throw new ConnectionException(this, response);
            }
        }

        /// <summary>
        /// Convenience routine to handle simple IAP commands
        /// that do not have responses specific to that command.
        /// </summary>
        /// <param name="command">the command</param>
        /// <param name="args">the arguments</param>
        /// <exception cref="ProtocolException">for protocol failures</exception>
        public void SimpleCommand(string command, Argument args)
        {
            // Issue command
            Response[] r = Command(command, args);

            // dispatch untagged responses
            NotifyResponseHandlers(r);

            // Handle result of this command
            HandleResult(r[r.Length - 1]);
        }

        public Response ReadResponse()
        {
            return new Response(this);
        }

        public bool SupportsUtf8()
        {
            return false;
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void disconnect()
        {
            if (client.IsNotNull())
            {
                client.Close();
                client = null;
            }
        }

        private void commandStart(string command) { }
        private void commandEnd() { }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Protocol()
        {
            disconnect();
        }
	}
}