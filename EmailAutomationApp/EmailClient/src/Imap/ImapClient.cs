using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Security;
using EmailAutomationLibrary.Properties;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.IO;

namespace EmailAutomationLibrary.Imap
{
    public class ImapClient : MailClient
    {
        #region Constants
        const string LOGIN = "$ LOGIN";
        const string OK = "$ OK";
        const string NO = "$ NO";
        const string LOGOUT = "$ LOGOUT";
        const string BYE = "* BYE";
        const string STATUS = "$ STATUS";
        const string SELECT = "$ SELECT";
        const string LIST = "$ LIST";
        const string CLOSE = "$ CLOSE";
        const string FETCH = "$ FETCH";
        const string REGULAR_EXP_MAIL = "[0-9]*[0-9]";
        const string SEARCH = "$ SEARCH";
        const string BAD = "$ BAD";
        const string AUTHENTICATION_FAILED = "AUTHENTICATIONFAILED";
        const string INVALID_CREDENTIAL = "Invalid credentials";
        const string LOGIN_FAILED = "login";
        #endregion

        #region Constructor
        public ImapClient(string host, int port, bool isSsl = false, RemoteCertificateValidationCallback validate = null) : base(host, port, isSsl, validate)
        {
            builder = new ImapMessageBuilder();
        }

        internal ImapClient(Stream stream) : base(stream)
        {
            builder = new ImapMessageBuilder();
        }
        #endregion

        #region Private Variable

        private bool _isConnected;
        private bool _isValidCredential;

        private int _totalMail;
        private int _totalUnreadMail;

        private bool _disposed = false;

        private readonly MessageBuilder builder;

        #endregion

        #region Public Methods

        /// <summary>
        ///  Selects specified folder (mailbox) as the current folder so that messages inside can be accessed.
        /// </summary>
        /// <returns/>* FLAGS (\Answered \Flagged \Draft \Deleted \Seen) * OK [PERMANENTFLAGS (\Answered \Flagged \Draft \Deleted \Seen \*)]
        // * OK [UIDVALIDITY 3]
        //  * 34 EXISTS
        // * 0 RECENT
        // * OK [UIDNEXT 4195]
        // $ OK [READ-WRITE] INBOX selected. (Success)
        //</returns>
        protected override string selectMailbox(string mailbox = "Inbox")
        {
            Trace(new TraceEventArg(() => "ImapClient::SelectFolder > Entry"));

            Trace(new TraceEventArg(() => "ImapClient::SelectFolder > Calling SendCommand"));

            _response = SendCommandGetResponse(SELECT + " " + mailbox.QuoteString(), false);

            while (_response.StartsWith("*"))
            {
                _response = GetResponse();
            }

            if (IsSuccess(_response))
            {
                _response = string.Empty;
            }

            return _response;
        }

        protected override string getAllMessagesFlag(out IEnumerable<int> mailIds)
        {
            Trace(new TraceEventArg(() => "ImapClient::GetAllMessagesFlag > Entry"));

            Trace(new TraceEventArg(() => "ImapClient::GetAllMessagesFlag > Calling SendCommand"));

            mailIds = new List<int>();

            _response = SendCommandGetResponse(FETCH + " 1:* FLAGS", true);

            if (!_response.StartsWith("Error:"))
            {
                while(_response.StartsWith("*"))
                {
                    Match m = Regex.Match(_response, "\\* (\\d+) FETCH \\((.*)\\)");
                    if (m.Success)
                    {
                        var mailId = Convert.ToInt32(m.Groups[1].Value);
                        ((List<int>)mailIds).Add(mailId);
                    }
                    _response = GetResponse(true);

                }

                if (IsSuccess(_response))
                    _response = string.Empty;
            }
            // check error handling
            return _response;
        }

        protected override string searchMail(string criteria, out IEnumerable<int> mailIds)
        {
            Trace(new TraceEventArg(() => "ImapClient::SearchMail > Entry"));

            Trace(new TraceEventArg(() => "ImapClient::SearchMail > Calling SendCommand"));

            mailIds = new List<int>();

            StringReader reader = new StringReader(criteria);

            bool useUtf8 = criteria.Contains(Environment.NewLine);

            string line = reader.ReadLine();

            line = (useUtf8) ? $"CHARSET UTF-8 {line}" : line;

            _response = SendCommandGetResponse($"{SEARCH} {line}", true);

            while((line = reader.ReadLine()) != null)
            {
                if (!_response.StartsWith("+"))
                {
                    _response = "Please restrict you search to ASCII-only characters.";
                }
                _response = SendCommandGetResponse(line, true);
            }

            while (_response.StartsWith("*"))
            {
                Match m = Regex.Match(_response, @"^\* SEARCH (.+)");
                if (m.Success)
                {
                    string[] v = m.Groups[1].Value.Trim().Split(' ');
                    foreach (string s in v)
                    {
                        try
                        {
                            ((List<int>)mailIds).Add(Convert.ToInt32(s));
                        }
                        catch (FormatException) { }
                    }
                }
                _response = GetResponse(true);
            }

            if (IsSuccess(_response))
                _response = string.Empty;

            return _response;
        }

        public override string FetchMailboxes(out IEnumerable<Mailbox> mailboxes, string directory = "", string pattern = "*")
        {
            mailboxes = new List<Mailbox>();

            _response = SendCommandGetResponse(LIST + " " + directory.QuoteString() + " " + pattern.QuoteString() + Environment.NewLine, true);

            while (_response.StartsWith("*"))
            {
                Match m = Regex.Match(_response, "\\* LIST \\((.*)\\)\\s+\"([^\"]+)\"\\s+(.+)");

                if (m.Success)
                {
                    string[] attr = m.Groups[1].Value.Split(' ');
                    bool add = true;
                    foreach (string a in attr)
                    {
                        // Only list mailboxes that can actually be selected.
                        if (a.ToLower() == @"\noselect")
                            add = false;
                    }
                    // Names _should_ be enclosed in double-quotes but not all servers follow through with
                    // this, so we don't enforce it in the above regex.
                    if (add)
                    {
                        string separator = m.Groups[2].Value;
                        string fullname = Regex.Replace(m.Groups[3].Value, "^\"(.+)\"$", "$1");
                        string name = (fullname.LastIndexOf(separator) > 0) ? fullname.Substring(fullname.LastIndexOf(separator) + 1, fullname.Length - (fullname.LastIndexOf(separator) + 1)) : fullname;
                        ((List<Mailbox>)mailboxes).Add(new Mailbox(name, fullname, separator));
                    }
                }

                _response = GetResponse(true);
            }

            if (IsSuccess(_response))
            {
                _response = string.Empty;
            }

            return _response;
        }

        #endregion

        #region Private Methods

        private string GetMessageHeaders(int mailIndex)
        {
            Trace(new TraceEventArg(() => "ImapClient::GetMessageHeaders > Entry"));
            _response = string.Empty;

            _response = SendCommandGetResponse(FETCH + " " + mailIndex + " (BODY[HEADER])", false);

            StringBuilder builder = new StringBuilder();

            while (_response.StartsWith("*"))
            {
                Match m = Regex.Match(_response, @"\* \d+ FETCH .* {(\d+)}");

                if (m.Success)
                {
                    int size = Convert.ToInt32(m.Groups[1].Value);
                    builder.Append(GetData(size));
                    _response = GetResponse(true);
                }
            }

            _response = GetResponse();

            if (IsSuccess(_response))
            {
                _response = builder.ToString();
                builder.Remove(0, builder.Length);
                builder = null;
            }

            Trace(new TraceEventArg(() => $"ImapClient::GetMessageHeaders > Exit with _response: {_response}"));
            return _response;
        }

        private string GetMessageData(int mailIndex)
        {
            Trace(new TraceEventArg(() => "ImapClient::GetMessageData > Entry"));
            _response = string.Empty;
            _response = SendCommandGetResponse(FETCH + " " + mailIndex + " (BODY[])", false);

            StringBuilder builder = new StringBuilder();

            while (_response.StartsWith("*"))
            {
                Match m = Regex.Match(_response, @"\* \d+ FETCH .* {(\d+)}");

                if (m.Success)
                {
                    int size = Convert.ToInt32(m.Groups[1].Value);
                    builder.Append(GetData(size));
                    _response = GetResponse(true);
                }
            }

            _response = GetResponse();

            if (IsSuccess(_response))
            {
                _response = builder.ToString();
                builder.Remove(0, builder.Length);
                builder = null;
            }
            Trace(new TraceEventArg(() => $"ImapClient::GetMessageData > Exit with _response: {_response}"));
            return _response;
        }

        /// <summary>
        /// Issues CLOSE command to the server. some server don't support this method. so, handle this case (e.g: Bug ID - 7089)
        /// CLOSE command permanently removes all messages that have the Flag.Deleted from currently selected folder and 
        /// returns to the authenticated state from the selected state.
        /// </summary>
        ///  AOL :: $ OK CLOSE completed
        /// Xchange server 2010 : $ OK CLOSE completed.
        /// <returns>Gmail :: $ OK Returned to authenticated state. (Success)</returns>
        private string CloseInbox()
        {
            Trace(new TraceEventArg(() => "ImapClient::CloseInbox > Entry"));
            try
            {
                _response = SendCommandGetResponse(CLOSE, true);
                Trace(new TraceEventArg(() => $"ImapClient::CloseInbox > _response is: {_response}"));
                if (IsSuccess(_response))
                {
                    _response = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _response = ex.Message;
            }
            return _response;
        }

        /// <summary>
        /// This method parsed server response.
        /// if the response was successfull i.e., last line of response contains " $ OK" message.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected override bool IsSuccess(string response)
        {
            Trace(new TraceEventArg(() => "ImapClient::IsSuccess > Entry"));

            Trace(new TraceEventArg(() => $"ImapClient::IsSuccess > @response: {response}"));
            bool isSucess = false;

            if (response.StartsWith(OK))
            {
                isSucess = true;
            }
            else if (string.IsNullOrEmpty(response))
            {
                isSucess = true;
            }
            Trace(new TraceEventArg(() => $"ImapClient::IsSuccess > Exit with isSuccess: {isSucess}"));
            return isSucess;
        }

        #endregion

        #region Protected Methods

        protected override void ValidateResponse(string response)
        {
            Trace(new TraceEventArg(() => "ImapClient::ValidateResponse > Entry"));

            if (response.StartsWith("* OK"))
            {
                _response = string.Empty;
                _isConnected = true;
                ResetReceiveTimeout();
            }
            // +OK hello , +OK POP3 ready
            else if (response.StartsWith("+OK "))
            {
                _response = GlobalResources.EmailAutomationInvalidPort;
            }

            Trace(new TraceEventArg(() => $"ImapClient::ValidateResponse > Exit with response {_response}"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// $ OK LOGIN completed
        /// $ OK [CAPABILITY IMAP4rev1 LITERAL+ SASL-IR LOGIN-REFERRALS ID ENABLE SORT SORT=DISPLAY 
        /// THREAD=REFERENCES THREAD=REFS MULTIAPPEND UNSELECT IDLE CHILDREN NAMESPACE UIDPLUS LIST-EXTENDED I18NLEVEL=1 CONDSTORE QRESYNC ESEARCH ESORT SEARCHRES WITHIN CONTEXT=SEARCH LIST-STATUS QUOTA] Logged in
        /// <returns> * CAPABILITY IMAP4rev1 UNSELECT IDLE NAMESPACE QUOTA ID XLIST CHILDREN X-GM-EXT-1 UIDPLUS COMPRESS=DEFLATE $ OK automation.rajesh@gmail.com rajesh patel authenticated (Success)</returns>
        protected override string authenticateUser(string username, string password)
        {
            Trace(new TraceEventArg(() => "ImapClient::AuthenticateUser > Entry"));
            _response = string.Empty;

            Trace(new TraceEventArg(() => "ImapClient::AuthenticateUser > Calling SendCommand"));

            _response = SendCommandGetResponse(LOGIN + " " + username.QuoteString() + " " + password.QuoteString(), true);

            while (_response.StartsWith("*") && !_response.StartsWith(BYE))
            {
                _response = GetResponse(true);
            }

            if (_response.Contains(OK))
            {
                _isValidCredential = true;
                _response = string.Empty;
            }
            else if (_response.StartsWith(NO))
            {
                if (_response.Contains(AUTHENTICATION_FAILED) || _response.Contains(INVALID_CREDENTIAL)  // from google and automation anywhere
                    || _response.ToLower().Contains(LOGIN_FAILED)) //AOL
                {
                    _response = GlobalResources.EmailAutomationInvalidAuthentication;
                    _isValidCredential = false;
                }
                else if (_response.Contains(BAD) || _response.Contains(BYE)) // from tapan and AOL
                {
                    _response = GlobalResources.EmailAutomationInvalidAuthentication;
                    _isValidCredential = false;
                }
                Trace(new TraceEventArg(() => $"ImapClient::AuthenticateUser > Fails: {_response}"));
            }

            return _response;
        }

        /// <summary>
        /// * BYE IMAP4rev1 Server logging out.
        /// /// * BYE dbmail imap server kisses you goodbye
        /// automation anywhere ::  * BYE Logging out. $ OK Logout completed.
        /// </summary>
        /// <returns> Gmail :: * BYE LOGOUT Requested $ OK 73 good day (Success)</returns>
        protected override string logOut()
        {
            try
            {
                CloseInbox();
                Trace(new TraceEventArg(() => "ImapClient::LogOut > Entry"));
                _response = SendCommandGetResponse(LOGOUT, true);
                Trace(new TraceEventArg(() => $"ImapClient::LogOut > The response after firing LogOut is : {_response}"));
                if (_response.Contains(BYE))
                {
                    _response = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _response = ex.Message;
                Trace(new TraceEventArg(() => $"ImapClient::LogOut > {ex.Message}, {ex}"));
            }

            return _response;
        }

        /// <summary>
        /// Get totalmail from user account
        /// </summary>
        /// <returns>* STATUS "INBOX" (MESSAGES 34) $ OK Success</returns>
        protected override string getTotalMail(out int mailCount, string mailbox = "Inbox", string flags = "messages")
        {
            Trace(new TraceEventArg(() => "ImapClient::GetTotalMail > Entry"));

            Trace(new TraceEventArg(() => "ImapClient::GetTotalMail > Calling SendCommand"));

            mailCount = -1;

            _response = SendCommandGetResponse(STATUS + " " + mailbox.QuoteString() + " (" + flags + ")", false);

            while (_response.StartsWith("*"))
            {
                Match m = Regex.Match(_response, REGULAR_EXP_MAIL);
                if (m.Success)
                {
                    mailCount = Convert.ToInt32(m.Groups[0].Value.ToString());
                }
                _response = GetResponse();
            }

            if (IsSuccess(_response))
            {
                _response = string.Empty;       
            }
            else
            {
                mailCount = -1;
            }
            return _response;
        }

        /// <summary>
        ///  Deletes message specified by the mailIndex.  Issues EXPUNGE command after.
        /// </summary>
        /// <param name="mailIndex"></param>
        /// <returns>UID OK Success</returns>
        protected override string deleteMail(int mailIndex)
        {
            try
            {
                Trace(new TraceEventArg(() => "ImapClient::DeleteMail > Entry"));
                _response = SendCommandGetResponse("$ STORE " + mailIndex + " +FLAGS.SILENT (\\Deleted)", true);
                Trace(new TraceEventArg(() => $"ImapClient::DeleteMail > Response is: {_response}"));
                if (IsSuccess(_response))
                {
                    _response = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _response = ex.Message;
            }
            return _response;
        }

        protected override MailMessage fetchMailFromHeader(int emailId)
        {
            Trace(new TraceEventArg(() => "ImapClient::FetchEmail > Entry"));
            try
            {
                string result = GetMessageHeaders(emailId);
                var parser = builder.FromHeader(result);
                Trace(new TraceEventArg(() => "ImapClient::FetchEmail > Exit"));
                return parser;
            }
            catch (Exception e)
            {
                Trace(new TraceEventArg(() => $"ImapClient::FetchEmail > Exit with error: {e.Message}"));
            }
            return null;
        }

        protected override MailMessage fetchMail(int emailId)
        {
            MailMessage message = null;
            Trace(new TraceEventArg(() => "ImapClient::FetchEmail > Entry"));
            string bodyText = GetMessageData(emailId);

            if (!string.IsNullOrEmpty(bodyText.Trim()))
            {
                message = builder.FromMIME822(bodyText);
                Trace(new TraceEventArg(() => "ImapClient::FetchEmail > Exit"));
                //return Utility.ParseMessageParts(bodyText);
            }
            Trace(new TraceEventArg(() => "ImapClient::FetchMessageParts > Exit with Null"));
            return message;
        }

        #endregion

        ~ImapClient()
        {
            Dispose(false);
        }
    }
}
