using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using EmailAutomationLibrary.Properties;
using System.Security.Authentication;
using System.Net.Mail;
using EmailAutomationLibrary.Imap;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailAutomationLibrary
{
    public abstract class MailClient : IMailClient, ITraceable, IDisposable
    {
        #region Constants
        protected const string INVALID_AUTHENTICATION = "Invalid username or password.";
        protected const string INVALID_PORT = "Please provide a valid port number.";
        protected const string INVALID_HOST_PORT = "Unable to establish connection. Please verify hostname or port number.";
        protected const string SERVER_NOT_SUPPORT_SSL = "Provided server does not support secure connection. Please try again without secure connection.";
        #endregion

        #region Constructor
        protected MailClient(string host, int port, bool isSsl, RemoteCertificateValidationCallback validate)
        {
            HostName = host;
            PortNo = port;
            IsSsl = isSsl;
            Validate = validate;
        }

        protected MailClient(Stream clientStream)
        {
            _clientStream = clientStream;
            _auth = true;
        }
        #endregion

        #region Protected Properties

        protected string HostName { get; private set; }

        protected int PortNo { get; private set; }

        protected bool IsSsl { get; private set; }

        protected RemoteCertificateValidationCallback Validate { get; private set; }

        #endregion

        #region Protected Variables

        protected string _response = string.Empty;

        #endregion

        #region Private Variables

        private const bool IsAutoReconnect = true;
        private bool _isTimeoutReconnect;
        private bool _disposed = false;
        private ITcpClientAdapter _client;
        private Stream _clientStream;
        private readonly object readLock = new object();
        private readonly object writeLock = new object();
        private readonly bool _auth = false;

        public event TraceEventHandler LogTrace;
        #endregion

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Trace(TraceEventArg e)
        {
            if (LogTrace != null)
                LogTrace(this, e);
        }

        #region Common Metheods For IMP4 & POP3

        public string GetHostName()
        {
            return HostName;
        }

        public string LogIn(string username, string password)
        {
            string retVal = string.Empty;

            if (!_auth)
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

        public string GetTotalMail(out int mailCount, string mailbox = "Inbox", string flags = "messages")
        {
            mailCount = -1;
            return getTotalMail(out mailCount, mailbox, flags);
        }

        public string DeleteMail(int mailIndex, string mailbox = "Inbox")
        {
            selectMailbox(mailbox);
            return deleteMail(mailIndex);
        }

        public MailMessage FetchMailFromHeader(int emailId, string mailbox = "Inbox")
        {
            selectMailbox(mailbox);
            return fetchMailFromHeader(emailId);
        }

        public MailMessage FetchMail(int emailId, string mailbox = "Inbox")
        {
            selectMailbox(mailbox);
            return fetchMail(emailId);
        }

        public string GetAllMessagesFlag(out IEnumerable<int> mailIds, string mailbox = "Inbox")
        {
            selectMailbox(mailbox);
            return getAllMessagesFlag(out mailIds);
        }

        public string SearchMail(string criteria, out IEnumerable<int> mailIds, string mailbox = "Inbox")
        {
            selectMailbox(mailbox);
            return searchMail(criteria, out mailIds);
        }

        #endregion

        #region IMAP4 Related Methods

        public virtual string FetchMailboxes(out IEnumerable<Mailbox> folders, string directory = "", string pattern = "*") { folders = null; return string.Empty; }

        #endregion
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Reconnect, if there is a timeout exception and isAutoReconnect is true
        /// </summary>
        //private bool ExecuteReconnect(IOException ex, string command)
        //{
        //    Trace(new TraceEventArg(() => "EmailClient::ExecuteReconnect > Entry"));

        //    if (ex.InnerException != null && ex.InnerException is SocketException)
        //    {
        //        SocketException innerEx = (SocketException)ex.InnerException;

        //        if (innerEx.ErrorCode == 10053)
        //        {
        //            //probably timeout: An established connection was aborted by the software in your host machine.
        //            if (IsAutoReconnect)
        //            {
        //                //try to reconnect and send one more time
        //                _isTimeoutReconnect = true;
        //                try
        //                {

        //                    //  try to auto reconnect
        //                    initializeConnection();
        //                    SendCommand(command);
        //                    Trace(new TraceEventArg(() => "EmailClient::ExecuteReconnect > Exit with True"));
        //                    return true;
        //                }
        //                finally
        //                {
        //                    _isTimeoutReconnect = false;
        //                }
        //            }
        //        }
        //    }
        //    Trace(new TraceEventArg(() => "EmailClient::ExecuteReconnect > Exit with False"));
        //    return false;
        //}

        internal void SetTcpClient(ITcpClientAdapter client)
        {
            _client = client;
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
                _client = new TcpClientAdapter { ReceiveTimeout = 20000 };
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

        #endregion

        #region Protected Methods

        protected string initializeConnection()
        {
            Trace(new TraceEventArg(() => "EmailAutomationClient::InitializeConnection > Entry"));

            try
            {
                _response = string.Empty; // need this statment becoz during re-connect we need to clear prior response.

                Connect(HostName, PortNo, IsSsl, Validate);

            }
            catch (ArgumentOutOfRangeException ex)
            {
                _response = GlobalResources.EmailAutomationInvalidPort;
            }
            catch (AuthenticationException ex)
            {
                _response = GlobalResources.EmailAutomationInvalidHostNamePortNumber;
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
                        _response = GlobalResources.EmailAutomationInvalidHostNamePortNumber;
                        break;
                    default:
                        _response = GlobalResources.EmailAutomationInvalidHostNamePortNumber;
                        break;
                }
            }
            catch (Exception ex)
            {
                _response = GlobalResources.EmailAutomationInvalidHostNamePortNumber;
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

        protected virtual string selectMailbox(string mailbox = "Inbox") { return string.Empty; }

        protected virtual string searchMail(string criteria, out IEnumerable<int> mailIds)
        {
            mailIds = null;
            return string.Empty;
        }

        protected abstract string logOut();

        protected abstract string getTotalMail(out int mailCount, string mailbox = "Inbox", string flags = "messages");

        protected abstract string deleteMail(int mailIndex);

        protected abstract MailMessage fetchMailFromHeader(int emailId);

        protected abstract MailMessage fetchMail(int emailId);

        protected abstract string getAllMessagesFlag(out IEnumerable<int> mailIds);

        protected abstract bool IsSuccess(string response);

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

        //protected MailParser CreateMailParser(string mailText)
        //{
        //    var parser = new MailParser(mailText);
        //    parser.LogTrace += new TraceEventHandler((s, e) => {
        //        Trace(e);
        //    });
        //    return parser;
        //}

        protected void ResetReceiveTimeout()
        {
            _client.ReceiveTimeout = 0;
        }

        //protected void ProcessSocketException(SocketException ex)
        //{
        //    Trace(new TraceEventArg(() => "EmailClient::ProcessSocketException > Entry"));
        //    switch (ex.ErrorCode)
        //    {
        //        case 11001:
        //            _response = ex.Message;
        //            break;
        //        case 10060:
        //            _response = "Invalid PortNumber" + Environment.NewLine + Environment.NewLine + ex.Message;
        //            break;
        //        case 10061:
        //            _response = "Invalid PortNumber" + Environment.NewLine + Environment.NewLine + ex.Message;
        //            break;
        //        case 10056:
        //            initializeConnection();
        //            //Socket is already connected.
        //            _response = string.Empty;
        //            break;
        //        default:
        //            _response = ex.Message;
        //            break;
        //    }
        //    Trace(new TraceEventArg(() => $"EmailClient::ProcessSocketException > Exit with _response: {_response}"));
        //}

        protected void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                    logOut();
                CleanSocket();
                this._disposed = true;
            }
        }

        #endregion

        #region Private Class
        private class TcpClientAdapter : ITcpClientAdapter
        {
            private readonly TcpClient _client;

            public TcpClientAdapter()
            {
                _client = new TcpClient();
            }

            public bool Connected
            {
                get
                {
                    return _client.Connected;
                }
            }

            public int ReceiveTimeout
            {
                get
                {
                    return _client.ReceiveTimeout;
                }

                set
                {
                    _client.ReceiveTimeout = value;
                }
            }

            public void Close()
            {
                _client.Close();
            }

            public void Connect(string hostname, int port)
            {
                _client.Connect(hostname, port);
            }

            public Stream GetStream()
            {
                return _client.GetStream();
            }
        }
        #endregion
    }
}
