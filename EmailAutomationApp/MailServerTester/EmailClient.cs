using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MailServerTester
{
    abstract class EmailClient : IDisposable
    {
        private ITcpClientAdapter _client;
        private Stream _stream;
        private readonly object readLock = new object();
        private readonly object writeLock = new object();

        protected EmailClient(string host, int port, bool isSsl, RemoteCertificateValidationCallback validate)
        {
            connect(host, port, isSsl, validate);
        }

        private void connect(string host, int port, bool isSsl, RemoteCertificateValidationCallback validate)
        {
            if (_client == null)
            {
                _client = new TcpClientAdapter { ReceiveTimeout = 20000 };
            }

            if (!_client.Connected)
            {
                try
                {
                    _client.Connect(host, port);
                }
                catch (SocketException ex)
                {
                    var retVal = string.Empty;
                    switch (ex.ErrorCode)
                    {
                        case 11001: // Host not found.
                            retVal = ex.Message;
                            break;
                        case 10060: //Connection timed out.  (invalid portNo) (yahoo,hotmail and indiatimes)
                        case 10061: //Connection refused.
                        case 10013: // Permission denied. from hotmail
                            retVal = "Invalid host or port number";
                            break;
                        default:
                            retVal = "Invalid host or port number";
                            break;
                    }

                    throw new Exception(retVal);
                }
            }

            if (_client.Connected)
            {
                _stream = _client.GetStream();
                if (isSsl)
                {
                    try
                    {
                        var sslStream = new SslStream(_stream, false, validate ?? ((sender, cert, chain, err) => true));
                        sslStream.AuthenticateAsClient(host);
                        _stream = sslStream;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }

                var response = ReadResponse();

                ValidateResponse(response);
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
                _stream.Write(bytes, 0, bytes.Length);
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
            const int NewLine = 10, CarrieageReturn = 13;

            using (var mem = new MemoryStream())
            {
                lock (readLock)
                {
                    while (true)
                    {
                        int i = _stream.ReadByte();
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
                            return s;
                        }
                        else
                            mem.WriteByte(b);
                    }
                }
            }
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
            byte[] buffer = new byte[4096];
            using (var mem = new MemoryStream())
            {
                lock (readLock)
                {
                    while (byteCount > 0)
                    {
                        int request = byteCount > buffer.Length ? buffer.Length : byteCount;
                        int read = _stream.Read(buffer, 0, request);
                        mem.Write(buffer, 0, read);
                        byteCount = byteCount - read;
                    }
                }
                return Encoding.ASCII.GetString(mem.ToArray());
            }
        }

        protected void ResetReceiveTimeout()
        {
            _client.ReceiveTimeout = 0;
        }

        protected void SetReceiveTimeout(int timeoutInSec)
        {
            _client.ReceiveTimeout = timeoutInSec * 1000;
        }

        protected abstract string ReadResponse();

        protected abstract bool IsSuccess(string response);

        protected abstract void ValidateResponse(string response);

        protected abstract string ReadAllLines();

        public virtual void SelectMailbox() { }

        public abstract void Login(string userName, string password);

        public abstract void LogOut();

        public abstract string ExecuteCommand(string command);

        protected string SendCommandGetResponse(string command)
        {
            lock (readLock)
            {
                lock (writeLock)
                {
                    SendCommand(command);
                }
                return ReadResponse();
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
            }
        }

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
    }
}
