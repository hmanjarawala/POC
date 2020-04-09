using EmailAutomationSample.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailAutomationSample.Classes
{
    abstract class MailClient : IMailClient, ITraceable, IDisposable
    {
        public event TraceEventHandler LogTrace;

        protected string _response = string.Empty;

        private const bool IsAutoReconnect = true;
        private bool _isTimeoutReconnect;
        private bool _disposed = false;
        private TcpClient _client;
        private Stream _clientStream;
        private readonly object readLock = new object();
        private readonly object writeLock = new object();
        private readonly string hostName;
        private readonly int port;
        private readonly bool isSecure;
        private readonly RemoteCertificateValidationCallback validate;

        public MailClient(string hostname, int port, bool isSecure = false, RemoteCertificateValidationCallback validate = null)
        {
            this.hostName = hostname;
            this.port = port;
            this.isSecure = isSecure;
            this.validate = validate;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string GetAllMessageFlags(MailFlag flag, out IEnumerable<int> mailIndexes, string mailbox = "Inbox")
        {
            selectMailbox(mailbox);
            return getAllMessagesFlag(flag, out mailIndexes);
        }

        public string GetHostName()
        {
            return hostName;
        }

        public string GetTotalMail(out int totalMailCount, string mailbox = "Inbox", string flag = "messages")
        {
            return getTotalMail(out totalMailCount, mailbox, flag);
        }

        public string Login(string username, string password)
        {
            string retVal = string.Empty;

            retVal = initializeConnection();

            if (string.IsNullOrWhiteSpace(retVal))
            {
                retVal = authenticateUser(username, password);
            }

            return retVal;
        }

        public string LogOut()
        {
            return logOut();
        }

        public void Trace(TraceEventArg e)
        {
            LogTrace?.Invoke(this, e);
        }


        protected string initializeConnection()
        {
            Trace(new TraceEventArg(() => "EmailAutomationClient::InitializeConnection > Entry"));

            try
            {
                _response = string.Empty; // need this statment becoz during re-connect we need to clear prior response.

                Connect(hostName, port, isSecure, validate);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                _response = Resources.EmailAutomationInvalidPort;
            }
            catch (AuthenticationException ex)
            {
                _response = Resources.EmailAutomationInvalidHostNamePortNumber;
            }
            catch (SocketException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 11001: // Host not found.
                        _response = ex.Message;
                        break;
                    case 10060: //Connection timed out.  (invalid portNo) (yahoo,hotmail and indiatimes)
                    case 10061: //Connection refused.
                    case 10013: // Permission denied. from hotmail
                        _response = Resources.EmailAutomationInvalidHostNamePortNumber;
                        break;
                    default:
                        _response = Resources.EmailAutomationInvalidHostNamePortNumber;
                        break;
                }
            }
            catch (Exception ex)
            {
                _response = Resources.EmailAutomationInvalidHostNamePortNumber;
            }
            Trace(new TraceEventArg(() => $"EmailAutomationClient::InitializeConnection > _response is: {_response}"));

            return _response;
        }

        protected void CleanSocket()
        {
            Trace(new TraceEventArg(() => "EmailClient::CleanSocket > Entry"));
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            if (_clientStream != null)
            {
                _clientStream.Close();
                _clientStream = null;
            }
            Trace(new TraceEventArg(() => "EmailClient::CleanSocket > Exit"));
        }

        /// <summary>
		/// Reads the specified amount of bytes from the server. This method blocks until the specified
		/// amount of bytes has been read from the network stream.
		/// </summary>
		/// <param name="byteCount">The number of bytes to read.</param>
		/// <returns>The read bytes as an ASCII-encoded string.</returns>
		/// <exception cref="IOException">The underlying socket is closed or there was a failure
		/// reading from the network.</exception>
		protected string GetData(int byteCount)
        {
            Trace(new TraceEventArg(() => "EmailClient::GetData > Entry"));

            byte[] buffer = new byte[4096];
            using (var mem = new MemoryStream())
            {
                lock (readLock)
                {
                    while (byteCount > 0)
                    {
                        int request = byteCount > buffer.Length ? buffer.Length : byteCount;
                        int read = _clientStream.Read(buffer, 0, request);
                        mem.Write(buffer, 0, read);
                        byteCount = byteCount - read;
                    }
                }
                var s = Encoding.ASCII.GetString(mem.ToArray());
                Trace(new TraceEventArg(() => $"EmailClient::GetData > Exit with response {s}"));
                return s;
            }
        }

        protected abstract string authenticateUser(string username, string password);

        protected abstract string getTotalMail(out int totalMailCount, string mailbox = "Inbox", string flag = "messages");

        protected virtual string selectMailbox(string mailbox = "Inbox") { return string.Empty; }

        protected abstract string logOut();

        protected abstract string getAllMessagesFlag(MailFlag flag, out IEnumerable<int> mailIds);

        protected abstract void ValidateResponse(string response);

        /// <summary>
        /// Sends command and waits for the server response.
        /// </summary>
        /// <param name="commandName">CommandName e.g. "NOOP".</param>
        /// <returns>empty string if successfull</returns>
        protected string SendCommandGetResponse(string commandName, bool resolveLiterals)
        {
            lock (readLock)
            {
                lock (writeLock)
                {
                    SendCommand(commandName);
                }
                return GetResponse(resolveLiterals);
            }
        }

        /// <summary>
		/// Sends a command string to the server. This method blocks until the command has been
		/// transmitted.
		/// </summary>
		/// <param name="command">The command to send to the server. The string is suffixed by CRLF
		/// prior to sending.</param>
        protected void SendCommand(string command)
        {
            var bytes = Encoding.ASCII.GetBytes($"{command}\r\n");
            lock (writeLock)
            {
                _clientStream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
		/// Waits for a response from the server. This method blocks until a response has been received.
		/// </summary>
		/// <param name="resolveLiterals">Set to true to resolve possible literals returned by the
		/// server (Refer to RFC 3501 Section 4.3 for details).</param>
		/// <returns>A response string from the server</returns>
		/// <exception cref="IOException">The underlying socket is closed or there was a failure
		/// reading from the network.</exception>
        protected string GetResponse(bool resolveLiterals = false)
        {
            Trace(new TraceEventArg(() => "EmailClient::GetResponse > Entry"));
            const int NewLine = 10, CarrieageReturn = 13;

            using (var mem = new MemoryStream())
            {
                lock (readLock)
                {
                    while (true)
                    {
                        int i = _clientStream.ReadByte();
                        if (i == -1)
                            throw new IOException("The stream could not be read.");
                        byte b = (byte)i;
                        if (b == CarrieageReturn)
                            continue;
                        if (b == NewLine)
                        {
                            string s = Encoding.ASCII.GetString(mem.ToArray());
                            if (resolveLiterals)
                            {
                                s = Regex.Replace(s, @"{(\d+)}$", m =>
                                {
                                    return "\"" + GetData(Convert.ToInt32(m.Groups[1].Value)) +
                                        "\"" + GetResponse(false);
                                });
                            }
                            Trace(new TraceEventArg(() => $"EmailClient::GetResponse > Exit with response {s}"));
                            return s;
                        }
                        else
                            mem.WriteByte(b);
                    }
                }
            }
        }

        protected void ResetReceiveTimeout()
        {
            _client.ReceiveTimeout = 0;
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    logOut();
                CleanSocket();
                _disposed = true;
            }
        }

        /// <summary>
		/// Connects to the specified port on the specified host, optionally using the Secure Socket Layer
		/// (SSL) security protocol.
		/// </summary>
		/// <param name="hostname">The DNS name of the server to which you intend to connect.</param>
		/// <param name="port">The port number of the server to which you intend to connect.</param>
		/// <param name="ssl">Set to true to use the Secure Socket Layer (SSL) security protocol.</param>
		/// <param name="validate">Delegate used for verifying the remote Secure Sockets Layer (SSL)
		/// certificate which is used for authentication. Can be null if not needed.</param>
		/// <exception cref="ArgumentOutOfRangeException">The port parameter is not between MinPort
		/// and MaxPort.</exception>
		/// <exception cref="ArgumentNullException">The hostname parameter is null.</exception>
		/// <exception cref="IOException">There was a failure writing to or reading from the
		/// network.</exception>
		/// <exception cref="SocketException">An error occurred while accessing the socket used for
		/// establishing the connection to the IMAP server. Use the ErrorCode property to obtain the
		/// specific error code.</exception>
		/// <exception cref="System.Security.Authentication.AuthenticationException">An authentication
		/// error occured while trying to establish a secure connection.</exception>
        private void Connect(string host, int port, bool isSsl, RemoteCertificateValidationCallback validate)
        {
            if (_client == null)
            {
                _client = new TcpClient { ReceiveTimeout = 20000 };
            }

            if (!_client.Connected)
            {
                _client.Connect(host, port);
            }

            if (_client.Connected)
            {
                _clientStream = _client.GetStream();
                if (isSsl)
                {
                    var sslStream = new SslStream(_clientStream, false, validate ?? ((sender, cert, chain, err) => true));
                    sslStream.AuthenticateAsClient(host);
                    _clientStream = sslStream;
                }

                _response = GetResponse(false);

                ValidateResponse(_response);
            }
        }
    }
}
