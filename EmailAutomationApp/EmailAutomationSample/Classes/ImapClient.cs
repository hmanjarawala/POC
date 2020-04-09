using EmailAutomationSample.Properties;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Text.RegularExpressions;

namespace EmailAutomationSample.Classes
{
    class ImapClient : MailClient
    {
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
        const string BAD = "$ BAD";
        const string AUTHENTICATION_FAILED = "AUTHENTICATIONFAILED";
        const string INVALID_CREDENTIAL = "Invalid credentials";
        const string LOGIN_FAILED = "login";

        public ImapClient(string hostName, int port, bool isSecure = false, RemoteCertificateValidationCallback validate = null)
            : base(hostName, port, isSecure, validate) { }

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

            _response = SendCommandGetResponse(SELECT + " \"" + mailbox + "\"", false);

            while (_response.StartsWith("*"))
            {
                _response = GetResponse();
            }

            if (isSuccess(_response))
            {
                _response = string.Empty;
            }

            return _response;
        }

        protected override string getAllMessagesFlag(MailFlag flag, out IEnumerable<int> mailIds)
        {
            Trace(new TraceEventArg(() => "ImapClient::GetAllMessagesFlag > Entry"));

            Trace(new TraceEventArg(() => "ImapClient::GetAllMessagesFlag > Calling SendCommand"));

            mailIds = new List<int>();

            _response = SendCommandGetResponse(FETCH + " 1:* FLAGS", true);

            if (!_response.StartsWith("Error:"))
            {
                while (_response.StartsWith("*"))
                {
                    Match m = Regex.Match(_response, "\\* (\\d+) FETCH \\((.*)\\)");
                    if (m.Success)
                    {
                        var mailId = Convert.ToInt32(m.Groups[1].Value);
                        var seenFlag = m.Groups[2].Value.ToLower();

                        switch (flag)
                        {
                            case MailFlag.All:
                                ((List<int>)mailIds).Add(mailId);
                                break;
                            case MailFlag.Seen:
                                if (seenFlag.Contains("\\seen"))
                                    ((List<int>)mailIds).Add(mailId);
                                break;
                            case MailFlag.Unseen:
                                if (!seenFlag.Contains("\\seen"))
                                    ((List<int>)mailIds).Add(mailId);
                                break;
                        }
                    }
                    _response = GetResponse(true);

                }

                if (isSuccess(_response))
                    _response = string.Empty;
            }
            // check error handling
            return _response;
        }

        protected override void ValidateResponse(string response)
        {
            Trace(new TraceEventArg(() => "ImapClient::ValidateResponse > Entry"));

            if (response.StartsWith("* OK"))
            {
                _response = string.Empty;
                ResetReceiveTimeout();
            }
            // +OK hello , +OK POP3 ready
            else if (response.StartsWith("+OK "))
            {
                _response = Resources.EmailAutomationInvalidPort;
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

            _response = SendCommandGetResponse(LOGIN + " \"" + username + "\" \"" + password + "\"", true);

            if (_response.Contains(OK))
            {
                _response = string.Empty;
            }
            else if (_response.StartsWith(NO))
            {
                if (_response.Contains(AUTHENTICATION_FAILED) || _response.Contains(INVALID_CREDENTIAL)  // from google and automation anywhere
                    || _response.ToLower().Contains(LOGIN_FAILED)) //AOL
                {
                    _response = Resources.EmailAutomationInvalidAuthentication;
                }
                else if (_response.Contains(BAD) || _response.Contains(BYE)) // from tapan and AOL
                {
                    _response = Resources.EmailAutomationInvalidAuthentication;
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

            _response = SendCommandGetResponse(STATUS + " \"" + mailbox + "\" (" + flags + ")", false);

            while (_response.StartsWith("*"))
            {
                Match m = Regex.Match(_response, REGULAR_EXP_MAIL);
                if (m.Success)
                {
                    mailCount = Convert.ToInt32(m.Groups[0].Value.ToString());
                }
                _response = GetResponse();
            }

            if (isSuccess(_response))
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
                if (isSuccess(_response))
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
        private bool isSuccess(string response)
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

        ~ImapClient()
        {
            Dispose(false);
        }
    }
}
