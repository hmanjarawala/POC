using EmailAutomationLibrary;
using EmailAutomationLibrary.Pop3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using EmailAutomation.Test.Properties;
using System.Net.Mail;

namespace EmailAutomation.Test
{
    [ExcludeFromCodeCoverage]
    public class Pop3ClientTest
    {
        private static IMailClient GetMailClient(Stream stream)
        {
            var client = new Pop3Client(stream);
            client.LogTrace += (s, e) => Console.WriteLine(e.MessageDelegate());
            return client;
        }

        private static IMailClient GetMailClient(string hostname, int port, bool useSsl)
        {
            var client = new Pop3Client(hostname, port, useSsl);
            client.LogTrace += (s, e) => Console.WriteLine(e.MessageDelegate());
            return client;
        }

        public static IEnumerable<object[]> LoginData
        {
            get
            {
                const string POP_NOT_ALLOWED_RESPONSE = "Your account is not enabled for POP access. Please visit options section in yahoo mail accounts for enable POP access.";
                const string POP_NOT_ENABLED_RESPONSE = "Your account is not enabled for POP access.Please visit your Gmail settings page and enable your account for POP access.";

                return new[]
                {
                    new object[] { "+OK User successfully logged on.\r\n", string.Empty },
                    new object[] { "pop not allowed for user.\r\n", POP_NOT_ALLOWED_RESPONSE },
                    new object[] { "not enabled for POP access\r\n", POP_NOT_ENABLED_RESPONSE },
                    new object[] { "Invalid \r\n", "Invalid username or password." },
                    new object[] { "-ERR: authorization failed \r\n", "POP3 service for your account is not free" },
                    new object[] {"-ERR []Invalid command\r\n", "Invalid command." }
                };
            }
        }

        public static IEnumerable<object[]> ConnectionData
        {
            get
            {
                return new[]
                {
                    new object[] {"outlook.office365.com", 995, true, "engg-qa-test@automationanywhere.com", Resources.CORRECT_PASSWORD, string.Empty},
                    new object[] {"outlook.office365.com", 110, false, "engg-qa-test@automationanywhere.com", Resources.CORRECT_PASSWORD, "-ERR Command is not valid in this state."},
                    new object[] {"outlook.office365.com", 993, true, "engg-qa-test@automationanywhere.com", Resources.CORRECT_PASSWORD, "Please provide a valid port number."},
                    new object[] {"outlook.office364.com", 995, true, "engg-qa-test@automationanywhere.com", Resources.CORRECT_PASSWORD, "Unable to establish connection. Please verify hostname or port number."},
                    new object[] {"outlook.office365.com", 995, false, "engg-qa-test@automationanywhere.com", Resources.CORRECT_PASSWORD, "Unable to establish connection. Please verify hostname or port number."},
                    new object[] {"outlook.office365.com", 995, true, "engg-qa-test@automationanywhere.com", Resources.INCORRECT_PASSWORD, "Logon failure: unknown user name or bad password."},
                };
            }
        }

        public static IEnumerable<object[]> GetTotalMailData
        {
            get
            {
                return new[]
                {
                    new object[]{ "+OK 9318 1006667431\r\n", 9318, string.Empty },
                    new object[] { "-ERR Protocol error. 19\r\n", -1, "-ERR Protocol error. 19" }
                };
            }
        }

        public static IEnumerable<object[]> GetMessageFlagsData
        {
            get
            {
                return new[]
                {
                    new object[] {GetMessageFlagsResponse(), 5, new int[] { 1, 2, 3, 4, 5 }, string.Empty },
                    new object[] { "-ERR Protocol error. 14\r\n", 0, new int[0], "-ERR Protocol error. 14" },
                };
            }
        }

        public static IEnumerable<object[]> GetMailHeaderData
        {
            get
            {
                return new[]
                {
                    new object[] {MailMessageUtil.GetMailHeaderForHtmlMessage(), MailMessageUtil.CreateDummyMessageForHtmlMessage()},
                    new object[] {MailMessageUtil.GetMailHeaderForTextMessage(), MailMessageUtil.CreateDummyMessageForTextMessage()},
                    new object[] {MailMessageUtil.GetMailHeaderForHtmlMessageWithEmbeddedImage(), MailMessageUtil.CreateDummyMessageForHtmlMessageWithEmbeddedImage()}
                };
            }
        }

        public static IEnumerable<object[]> GetMailMessageData
        {
            get
            {
                return new[]
                {
                    new object[] {MailMessageUtil.GetHtmlMessage(), MailMessageUtil.CreateDummyMessageForHtmlMessage(), 3},
                    new object[] {MailMessageUtil.GetTextMessage(), MailMessageUtil.CreateDummyMessageForTextMessage(), 1},
                    new object[] {MailMessageUtil.GetHtmlMessageWithEmbeddedImage(), MailMessageUtil.CreateDummyMessageForHtmlMessageWithEmbeddedImage(), 3},
                    new object[] {MailMessageUtil.GetTextMessageWithAttachment(), MailMessageUtil.CreateDummyMessageForHtmlMessageWithEmbeddedImage(), 2}
                };
            }
        }

        [Theory, MemberData(nameof(LoginData))]
        private void Pop3Client_Authentication_Test(string input, string expected)
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.USER, "+OK\r\n");
            dict.Add(TcpCommand.PASS, input);

            using(var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.LogIn(string.Empty, string.Empty);

                Assert.Equal(expected, result);
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        private void Pop3Client_Authentication_With_Real_Data_Test(string hostname, int port, bool useSsl, string username, string password, string expected)
        {
            var client = GetMailClient(hostname, port, useSsl);
            var result = client.LogIn(username, password);

            Assert.Equal(expected, result);
        }

        [Fact]
        private void Pop3Client_LogOut_With_Null_Stream_Test()
        {
            var client = GetMailClient(null);
            var result = client.LogOut();

            Assert.Equal("Object reference not set to an instance of an object.", result);
        }

        [Fact]
        private void Pop3Client_LogOut_Test()
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.LOGT, "+OK Microsoft Exchange Server 2016 POP3 server signing off.\r\n");

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.LogOut();

                Assert.True(string.IsNullOrEmpty(result));
            }
        }

        [Theory, MemberData(nameof(GetTotalMailData))]
        private void Pop3Client_GetTotalMail_Test(string input, int totalMail, string expected)
        {
            int actMail = -1;
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.STAT, input);
            dict.Add(TcpCommand.LOGT, "+OK Microsoft Exchange Server 2016 POP3 server signing off.\r\n");

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.GetTotalMail(out actMail);
                ((IDisposable)client).Dispose();

                Assert.Equal(expected, result);
                Assert.Equal(totalMail, actMail);
            }
        }

        [Theory, MemberData(nameof(GetMessageFlagsData))]
        private void Pop3Client_GetMessageFlags_Test(string input, int totalIndexCount, int[] indexes, string expected)
        {
            IEnumerable<int> outMailboxes = null;
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.LIST, input);

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.GetAllMessagesFlag(out outMailboxes);

                Assert.Equal(expected, result);
                Assert.Equal(totalIndexCount, outMailboxes.Count());
                Assert.True(CompareMailIndexes(indexes, outMailboxes));
            }
        }

        [Theory, MemberData(nameof(GetMailHeaderData))]
        private void Pop3Client_FetchMailFromHeader_Test(string mailHeader, MailMessage message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("+OK");
            builder.AppendLine(mailHeader);
            builder.AppendLine("."); ;
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.TOP, builder.ToString());

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMailFromHeader(9370);

                Assert.Equal(message.Headers, result.Headers);
                Assert.Equal(message.Subject, result.Subject);
                Assert.Equal(message.From, result.From);
                Assert.Equal(message.To, result.To);
                Assert.Equal(message.CC, result.CC);
            }
        }

        [Fact]
        private void Pop3Client_FetchMailFromHeader_Returns_Null_If_Error_Occurs_Test()
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.TOP, "-ERR Protocol error. 19\r\n");

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMailFromHeader(9370);

                Assert.Null(result);
            }
        }

        [Theory, MemberData(nameof(GetMailMessageData))]
        private void Pop3Client_FetchMail_Test(string mailMessage, MailMessage message, int mimePartCount)
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.RETR, GetMailMessage(mailMessage));

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMail(9370);

                Assert.Equal(message.Headers, result.Headers);
                Assert.Equal(message.Subject, result.Subject);
                Assert.Equal(message.From, result.From);
                Assert.Equal(message.To, result.To);
                Assert.Equal(message.CC, result.CC);
            }
        }

        [Fact]
        private void Pop3Client_DeleteMail_Test()
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.DELE, "+OK\r\n");

            using (var stream = new Pop3Stream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.DeleteMail(1050);

                Assert.Equal(string.Empty, result);
            }
        }

        private static string GetMailMessage(string mailMessage)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("+OK");
            builder.Append(mailMessage);
            builder.AppendLine(".");

            return builder.ToString();
        }

        private static string GetMessageFlagsResponse()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("+OK 5 525246");
            builder.AppendLine("1 49164");
            builder.AppendLine("2 51988");
            builder.AppendLine("3 12710");
            builder.AppendLine("4 5971");
            builder.AppendLine("5 405413");
            builder.AppendLine("5   405413");
            builder.AppendLine(".");

            return builder.ToString();
        }

        private static bool CompareMailIndexes(IEnumerable<int> src, IEnumerable<int> dest)
        {
            if (src.Count() != dest.Count()) return false;

            int equalityIndex = 0;

            foreach (int srcIndex in src)
            {
                foreach (int destIndex in dest)
                {
                    if (srcIndex.Equals(destIndex))
                        equalityIndex++;
                }
            }

            return (equalityIndex == src.Count());
        }
    }
}
