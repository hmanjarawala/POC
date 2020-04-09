using EmailAutomationSample.Properties;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text.RegularExpressions;

namespace EmailAutomationSample.Classes
{
    class PopClient : MailClient
    {
        const string OK = "* OK ";
        const string INVALID = "Invalid";
        const string USER = "USER ";
        const string PASS = "PASS ";
        const string ERR = "-ERR";
        const string QUIT = "QUIT";
        const string STAT = "STAT";
        const string DELETE = "DELE ";
        const string TOP = "TOP ";
        const string LIST = "LIST ";

        const string POP_NOT_ALLOWED = "pop not allowed for user.";
        const string POP_NOT_ALLOWED_RESPONSE = "Your account is not enabled for POP access. Please visit options section in yahoo mail accounts for enable POP access.";

        const string POP_NOT_ENABLED = "not enabled for POP access";
        const string POP_NOT_ENABLED_RESPONSE = "Your account is not enabled for POP access.Please visit your Gmail settings page and enable your account for POP access.";

        const string AUTHORIZATION_FAIL = "authorization failed";
        const string AUTHORIZATION_FAIL_RESPONSE = "POP3 service for your account is not free";

        public PopClient(string hostname, int port, bool isSecure = false, RemoteCertificateValidationCallback validate = null)
            : base(hostname, port, isSecure, validate) { }

        /// <summary>
        /// This method instantiates necessary objects and connects to the POP3 server on the given host and port. 
        /// If it was specified in the constructor to open a secure connection to the POP3 server, 
        /// a SslStream object is created which encapsulates the NetworkStream returned from the TcpClient.GetStream() method
        /// </summary>
        /// <returns>Return empty string when Successfully login, otherwise return error message</returns>
        protected override void ValidateResponse(string serverResponse)
        {
            Trace(new TraceEventArg(() => "POP3::ValidateResponse > Entry"));

            // if provide imap protocol portNo : 993 server : mail.automationanywhere.com , pop.aol.com,pop.gmail.com
            if (serverResponse.Contains(INVALID))
            {
                _response = Resources.EmailAutomationInvalidPort;
            }

            else if (isSuccess(serverResponse))
            {
                ResetReceiveTimeout();  // reset to default
                _response = string.Empty;
            }
            //* OK IMAP4 ready
            // this will come if we specify port number which is used by IMAP4
            else if (serverResponse.Contains(OK))
            {
                _response = Resources.EmailAutomationInvalidPort;
            }
            else
            {
                _response = Resources.EmailAutomationInvalidHostNamePortNumber;
            }
            Trace(new TraceEventArg(() => $"POP3::ValidateResponse > Exit with _response: {_response}"));
        }

        /// <summary>
        /// This method sends the user email and password to the server.
        /// <remarks>Set IsValidCredential property true, if both are correct, otherwise set False</remarks>
        /// </summary>
        protected override string authenticateUser(string username, string password)
        {

            Trace(new TraceEventArg(() => "POP3::AuthenticateUser > Entry"));

            _response = SendCommandGetResponse(USER + username, false);

            if (isSuccess(_response))
            {
                _response = SendCommandGetResponse(PASS + password, false);

                Trace(new TraceEventArg(() => $"POP3::AuthenticateUser > _response after SendCommand is: {_response}"));

                if (isSuccess(_response))
                {
                    _response = string.Empty;
                }
                else if (_response.Contains(POP_NOT_ALLOWED))  // from yahoo
                {
                    _response = POP_NOT_ALLOWED_RESPONSE;
                    CleanSocket();
                }
                else if (_response.Contains(POP_NOT_ENABLED)) // from gmail
                {
                    _response = POP_NOT_ENABLED_RESPONSE;
                    CleanSocket();
                }
                //from indiatimes mail ::  Invalid PortNumber
                else if (_response.StartsWith(INVALID))
                {
                    _response = Resources.EmailAutomationInvalidAuthentication;
                    CleanSocket();

                }
                // invalid from yahoo : invalid user/password
                // invalid from aol : Invalid login or password
                // invalid from indiatimes : invalid username/password

                // from gmail : -ERR [AUTH] Username and password not accepted.
                // this will come when user enter valid username and password, but host server is different (wrong)
                // e.g host : pop.gmail.com and use yahoo password and username  

                else if (_response.StartsWith(ERR))
                {
                    CleanSocket();

                    if (_response.Contains(AUTHORIZATION_FAIL))
                    {
                        _response = AUTHORIZATION_FAIL_RESPONSE;
                    }
                    else
                    {
                        _response = _response.Substring(4);
                        _response = _response.Trim();
                        if (_response.StartsWith("["))
                        {
                            _response = _response.Substring(_response.LastIndexOf("]") + 1) + ".";

                        }
                    }
                }

            }

            Trace(new TraceEventArg(() => $"POP3::AuthenticateUser > Exit with _response as: {_response}"));

            return _response;
        }

        /// <summary>
        /// Exit session.Remove all deleted messages from the server.
        /// Closes the user session with the server
        ///  Hotmail : +OK mailbox unchanged, POP3 server signing off
        /// </summary>
        /// <returns>+OK server signing off.</returns>
        protected override string logOut()
        {
            Trace(new TraceEventArg(() => "POP3::LogOut > Entry"));
            try
            {
                _response = SendCommandGetResponse(QUIT, false);

                if (isSuccess(_response))
                {
                    _response = string.Empty;
                }

            }
            catch (Exception ex)
            {
                _response = ex.Message;
            }
            Trace(new TraceEventArg(() => $"POP3::LogOut > Exit with _response as: {_response}"));
            return _response;
        }

        /// <summary>
        /// Get the drop listing.
        /// </summary>
        /// <param name="mailCount"></param>
        /// <returns></returns>
        protected override string getTotalMail(out int mailCount, string folderName = "Inbox", string flags = "messages")
        {
            Trace(new TraceEventArg(() => "POP3::GetTotalMail > Entry"));
            _response = SendCommandGetResponse(STAT, false);

            if (isSuccess(_response))
            {
                string[] arr = _response.Substring(4).Split(' ');
                mailCount = Convert.ToInt32(arr[0]);
                _response = string.Empty;
            }
            else
            {
                mailCount = -1;
            }
            Trace(new TraceEventArg(() => $"POP3::GetTotalMail > Exit with _response as: {_response}"));
            return _response;
        }

        protected override string getAllMessagesFlag(MailFlag flag, out IEnumerable<int> mailIds)
        {
            Trace(new TraceEventArg(() => "POP3::GetAllMessagesFlag > Entry"));
            mailIds = new List<int>();
            _response = SendCommandGetResponse(LIST, false);

            if (isSuccess(_response))
            {
                while ((_response = GetResponse()) != ".")
                {
                    Match m = Regex.Match(_response, @"(\d+)\s(\d+)");
                    if (!m.Success)
                        continue;
                    int number = Convert.ToInt32(m.Groups[1].Value);
                    ((IList<int>)mailIds).Add(number);
                }
                _response = string.Empty;
            }
            Trace(new TraceEventArg(() => $"POP3::GetAllMessagesFlag > Exit with _response as: {_response}"));
            return _response;
        }

        /// <summary>
        /// This method parsed server response.
        /// if the response was successfull i.e., it began with an "+OK" message or 
        /// if it was an error i.e., it began with an "-ERR" message
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool isSuccess(string response)
        {
            Trace(new TraceEventArg(() => "POP3::IsSuccess > Entry"));

            Trace(new TraceEventArg(() => $"POP3::IsSuccess > @response: {response}"));

            bool isSuccess = false;

            if (response.StartsWith("+OK"))
            {
                isSuccess = true;
            }
            else if (response.StartsWith(ERR))
            {
                isSuccess = false;
            }
            Trace(new TraceEventArg(() => $"POP3::IsSuccess > Exit with isSuccess: {isSuccess}"));
            return isSuccess;
        }

        ~PopClient()
        {
            Dispose(false);
        }
    }
}
