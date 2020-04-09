using System;
using System.Net.Security;
using System.Text;

namespace MailServerTester
{
    class ImapClient : EmailClient
    {
        public ImapClient(string host, int port, bool isSsl = false, RemoteCertificateValidationCallback validate = null) : base(host, port, isSsl, validate) { }

        public override void Login(string userName, string password)
        {
            var response = SendCommandGetResponse(string.Format("$ LOGIN {0} {1}", userName, password));
            while (response.StartsWith("*") && !response.StartsWith("* BYE"))
            {
                response = GetResponse(true);
            }
            if (IsSuccess(response))
            {

            }
            else if(response.StartsWith("$ NO"))
            {
                if(response.Contains("AUTHENTICATIONFAILED") || response.Contains("Invalid credentials") || response.ToLower().Contains("login") || response.Contains("$ BAD") || response.Contains("* BYE"))
                {
                    throw new Exception("Invalid username or password");
                }
            }
        }

        public override void LogOut()
        {
            var response = SendCommandGetResponse("$ LOGOUT");

            if(!response.Contains("* BYE"))
            {
                throw new Exception("Unable to logout");
            }
        }

        protected override bool IsSuccess(string response)
        {
            bool isSuccess = false;

            if (string.IsNullOrWhiteSpace(response) || response.LastIndexOf("$ OK") >= 0)
                isSuccess = true;
            return isSuccess;
        }

        protected override string ReadResponse()
        {
            return GetResponse(false);
        }

        public override void SelectMailbox()
        {
            SendCommand("$ SELECT \"Inbox\"");
            var response = ReadAllLines();

            if (!IsSuccess(response))
            {
                throw new Exception("Unable to select mailbox");
            }
        }

        protected override void ValidateResponse(string response)
        {
            if(response.StartsWith("* OK"))
            {
                ResetReceiveTimeout();
            }
            else if(response.StartsWith("+ OK"))
            {
                throw new Exception("Invalid port number");
            }
        }

        protected override string ReadAllLines()
        {
            StringBuilder builder = new StringBuilder();
            var response = string.Empty;

            do
            {
                response = ReadResponse();
                builder.AppendLine(response);
            } while (!response.StartsWith("$ OK") && !response.StartsWith("$ BAD"));
            return builder.ToString();
        }

        public override string ExecuteCommand(string command)
        {
            SendCommand(command);
            return ReadAllLines();
        }
    }
}
