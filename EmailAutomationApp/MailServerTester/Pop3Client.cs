using System;
using System.Net.Security;
using System.Text;

namespace MailServerTester
{
    class Pop3Client : EmailClient
    {
        public Pop3Client(string host, int port, bool isSsl = false, RemoteCertificateValidationCallback validate = null) : base(host, port, isSsl, validate) { }

        public override string ExecuteCommand(string command)
        {
            var response = SendCommandGetResponse(command);

            if (IsSuccess(response))
            {
                var lowerCommand = command.ToLower();
                if(lowerCommand.Contains("list") || lowerCommand.Contains("capa") || lowerCommand.Contains("top") || lowerCommand.Contains("retr"))
                {
                    return response + Environment.NewLine + ReadAllLines();
                }
            }

            return response;
        }

        public override void Login(string userName, string password)
        {
            var response = SendCommandGetResponse("USER " + userName);

            if (IsSuccess(response))
            {
                response = SendCommandGetResponse("PASS " + password);

                if (IsSuccess(response))
                {

                }
                else if(response.Contains("pop not allowed for user"))
                {
                    throw new Exception("Your account is not enabled for POP access. Please visit options section is yahoo mail account for enable POP access.");
                }
                else if (response.Contains("not enabled for POP access"))
                {
                    throw new Exception("Your account is not enabled for POP access. Please visit Gmail settings pabe and enable your account for POP access.");
                }
                else if (response.StartsWith("Invalid"))
                {
                    throw new Exception("Invalid username or password.");
                }
                else if (response.StartsWith("-ERR"))
                {
                    if(response.Contains("authorization failed"))
                    {
                        throw new Exception("POP3 service for your account is not free");
                    }
                    else
                    {
                        response = response.Substring(4);
                        response = response.Trim();
                        if (response.StartsWith("["))
                        {
                            response = response.Substring(response.LastIndexOf("]") + 1) + ".";
                            throw new Exception(response);
                        }
                    }
                }
            }
        }

        public override void LogOut()
        {
            var response = SendCommandGetResponse("QUIT");

            if (!IsSuccess(response))
            {
                throw new Exception("Unable to logout");
            }
        }

        protected override bool IsSuccess(string response)
        {
            bool isSuccess = false;

            if (response.Contains("+OK"))
                isSuccess = true;
            else if (response.StartsWith("-ERR"))
                isSuccess = false;
            return isSuccess;
        }

        protected override string ReadAllLines()
        {
            StringBuilder builder = new StringBuilder();
            var response = string.Empty;

            do
            {
                response = ReadResponse();
                builder.AppendLine(response);

            } while (response != "." && response.IndexOf("-ERR") == -1);
            return builder.ToString();
        }

        protected override string ReadResponse()
        {
            return GetResponse();
        }

        protected override void ValidateResponse(string response)
        {
            if(response.IndexOf("Invalid") > 0 || response.LastIndexOf("* OK") > 0)
            {
                throw new Exception("Invalid port number");
            }
            else if (IsSuccess(response))
            {
                ResetReceiveTimeout();
            }
            else
            {
                throw new Exception("Invalid host or port number");
            }
        }
    }
}
