using EmailAutomation.Test.Properties;
using EmailAutomationLibrary;
using EmailAutomationLibrary.Imap;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Xunit;

namespace EmailAutomation.Test
{
    [ExcludeFromCodeCoverage]
    public class ImapClientTest
    {
        private static IMailClient GetMailClient(Stream stream)
        {
            var client = new ImapClient(stream);
            client.LogTrace += (s, e) => Console.WriteLine(e.MessageDelegate());
            return client;
        }

        private static IMailClient GetMailClient(string hostname, int port, bool useSsl)
        {
            var client = new ImapClient(hostname, port, useSsl);
            client.LogTrace += (s, e) => Console.WriteLine(e.MessageDelegate());
            return client;
        }

        public static IEnumerable<object[]> LoginData
        {
            get
            {
                return new[]
                {
                    new object[] { "$ OK LOGIN completed.\r\n", string.Empty },
                    new object[] { "$ NO LOGIN failed.\r\n", "Invalid username or password" },
                    new object[] { "$ NO AUTHENTICATIONFAILED.\r\n", "Invalid username or password" },
                    new object[] { "$ NO Invalid credentials.\r\n", "Invalid username or password" },
                    new object[] { "$ NO $ BAD Response.\r\n", "Invalid username or password" },
                    new object[] { "$ NO * BYE Microsoft Exchange Server 2016 IMAP4 server signing off.\r\n", "Invalid username or password" },
                };
            }
        }

        public static IEnumerable<object[]> ConnectionData
        {
            get
            {
                return new[]
                {
                    new object[] {"outlook.office365.com", 993, true, "abc@pqr.com", Resources.CORRECT_PASSWORD, string.Empty},
                    new object[] {"outlook.office365.com", 143, false, "abc@pqr.com", Resources.CORRECT_PASSWORD, "$ BAD Command received in Invalid state."},
                    new object[] {"outlook.office365.com", 995, true, "abc@pqr.com", Resources.CORRECT_PASSWORD, "Please provide a valid port number."},
                    new object[] {"outlook.office364.com", 993, true, "abc@pqr.com", Resources.CORRECT_PASSWORD, "Unable to establish connection. Please verify hostname or port number."},
                    new object[] {"outlook.office365.com", 993, false, "abc@pqr.com", Resources.CORRECT_PASSWORD, "Unable to establish connection. Please verify hostname or port number."},
                    new object[] {"outlook.office365.com", 993, true, "abc@pqr.com", Resources.INCORRECT_PASSWORD, "Invalid username or password"},
                };
            }
        }

        public static IEnumerable<object[]> GetTotalMailData
        {
            get
            {
                return new[]
                {
                    new object[]{ "* STATUS INBOX (MESSAGES 9370) \r\n$ OK STATUS completed.\r\n", 9370, string.Empty },
                    new object[] { "$ BAD Command Argument Error. 11\r\n", -1, "$ BAD Command Argument Error. 11" }
                };
            }
        }

        public static IEnumerable<object[]> FetchMailboxesData
        {
            get
            {
                var array = new Mailbox[]
                {
                    new Mailbox("Calendar", "Calendar"),
                    new Mailbox("Clutter", "Clutter"),
                    new Mailbox("Contacts", "Contacts"),
                    new Mailbox("INBOX", "INBOX"),
                    new Mailbox("AA-TA", "INBOX/AA-TA"),
                };
                return new[]
                {
                    new object[] { "* LIST (\\HasNoChildren) \"/\" Calendar\r\n* LIST (\\HasNoChildren) \"/\" Clutter\r\n* LIST (\\HasChildren) \"/\" Contacts\r\n* LIST (\\Marked \\HasChildren) \"/\" INBOX\r\n* LIST (\\HasNoChildren) \"/\" INBOX/AA-TA\r\n* LIST (\\NOSELECT \\HasChildren) \"/\" Junc\r\n$ OK LIST completed.\r\n", 5, array, string.Empty },
                    new object[] { "$ BAD Command Argument Error. 11\r\n", 0, new Mailbox[0], "$ BAD Command Argument Error. 11" }
                };
            }
        }

        public static IEnumerable<object[]> DeleteData
        {
            get
            {
                return new[]
                {
                    new object[] { "$ OK STORE completed.\r\n", string.Empty },
                    new object[] { null, "String reference not set to an instance of a String.\r\nParameter name: s" }
                };
            }
        }

        public static IEnumerable<object[]> GetMessageFlagsData
        {
            get
            {
                return new[]
                {
                    new object[] {GetMessageFlagsResponse(), 4, new int[] { 9370, 9371, 9372, 9373 }, string.Empty },
                    new object[] {"Error: unknown error occurs.\r\n", 0, new int[0], "Error: unknown error occurs." },
                };
            }
        }

        public static IEnumerable<object[]> GetMailHeaderData
        {
            get
            {
                return new[]
                {
                    new object[] {1013, MailMessageUtil.GetMailHeaderForHtmlMessage(), MailMessageUtil.CreateDummyMessageForHtmlMessage()},
                    new object[] {263, MailMessageUtil.GetMailHeaderForTextMessage(), MailMessageUtil.CreateDummyMessageForTextMessage()},
                    new object[] {326, MailMessageUtil.GetMailHeaderForHtmlMessageWithEmbeddedImage(), MailMessageUtil.CreateDummyMessageForHtmlMessageWithEmbeddedImage()}
                };
            }
        }

        public static IEnumerable<object[]> GetMailMessageData
        {
            get
            {
                return new[]
                {
                    new object[] { 19818, MailMessageUtil.GetHtmlMessage(), MailMessageUtil.CreateDummyMessageForHtmlMessage(), 3},
                    new object[] { 445, MailMessageUtil.GetTextMessage(), MailMessageUtil.CreateDummyMessageForTextMessage(), 1},
                    new object[] { 30376, MailMessageUtil.GetHtmlMessageWithEmbeddedImage(), MailMessageUtil.CreateDummyMessageForHtmlMessageWithEmbeddedImage(), 3},
                    new object[] { 4627, MailMessageUtil.GetTextMessageWithAttachment(), MailMessageUtil.CreateDummyMessageForHtmlMessageWithEmbeddedImage(), 2}
                };
            }
        }

        [Theory, MemberData(nameof(LoginData))]
        private void ImapClient_Authentication_Test(string input, string expected)
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.LOGN, input);

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.LogIn(string.Empty, string.Empty);

                Assert.Equal(expected, result);
            }
        }

        [Theory, MemberData(nameof(ConnectionData))]
        private void ImapClient_Authentication_With_Real_Data_Test(string hostname, int port, bool useSsl, string username, string password, string expected)
        {
            var client = GetMailClient(hostname, port, useSsl);
            var result = client.LogIn(username, password);

            Assert.Equal(expected, result);
        }

        [Fact]
        private void ImapClient_LogOut_With_Null_Stream_Test()
        {
            var client = GetMailClient(null);
            var result = client.LogOut();

            Assert.Equal("Object reference not set to an instance of an object.", result);
        }

        [Fact]
        private void ImapClient_LogOut_Test()
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.LOGT, "* BYE Microsoft Exchange Server 2016 IMAP4 server signing off.\r\n");
            dict.Add(TcpCommand.CLOS, null);

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.LogOut();

                Assert.True(string.IsNullOrEmpty(result));
            }
        }

        [Theory, MemberData(nameof(GetTotalMailData))]
        private void ImapClient_GetTotalMail_Test(string input, int totalMail, string expected)
        {
            int actMail = -1;
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.STAT, input);
            dict.Add(TcpCommand.CLOS, "\r\n");
            dict.Add(TcpCommand.LOGT, "* BYE Microsoft Exchange Server 2016 IMAP4 server signing off.\r\n");

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.GetTotalMail(out actMail);
                ((IDisposable)client).Dispose();

                Assert.Equal(expected, result);
                Assert.Equal(totalMail, actMail);
            }
        }

        [Theory, MemberData(nameof(DeleteData))]
        private void ImapClient_DeleteMail_Test(string input, string expected)
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.SELE, GetSelectInboxResponse());
            dict.Add(TcpCommand.DELE, input);

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.DeleteMail(1050);

                Assert.Equal(expected, result);
            }
        }

        [Theory, MemberData(nameof(FetchMailboxesData))]
        private void ImapClient_FetchMailboxes_Test(string input, int totalMailboxes, Mailbox[] array, string expected)
        {
            IEnumerable<Mailbox> outMailboxes = null;
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.LIST, input);
            dict.Add(TcpCommand.CLOS, "$ OK CLOSE completed.\r\n");
            dict.Add(TcpCommand.LOGT, "* BYE Microsoft Exchange Server 2016 IMAP4 server signing off.\r\n");

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMailboxes(out outMailboxes);
                ((IDisposable)client).Dispose();

                outMailboxes.ToList().Sort();

                Assert.Equal(expected, result);
                Assert.Equal(totalMailboxes, outMailboxes.Count());
                Assert.True(CompareMailboxes(array, outMailboxes));
            }
        }
        
        [Theory, MemberData(nameof(GetMessageFlagsData))]
        private void ImapClient_GetMessageFlags_Test(string input, int totalIndexCount, int[] indexes, string expected)
        {
            IEnumerable<int> outMailboxes = null;
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.SELE, GetSelectInboxResponse());
            dict.Add(TcpCommand.FETC, input);

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.GetAllMessagesFlag(out outMailboxes);

                Assert.Equal(expected, result);
                Assert.Equal(totalIndexCount, outMailboxes.Count());
                Assert.True(CompareMailIndexes(indexes, outMailboxes));
            }
        }

        [Theory, MemberData(nameof(GetMailHeaderData))]
        private void ImapClient_FetchMailFromHeader_Test(int size, string messageHeader, MailMessage message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("* 9370 FETCH (BODY[HEADER] {" + size.ToString() + "}");
            builder.Append(messageHeader);
            builder.AppendLine("");
            builder.AppendLine(" FLAGS (\\Seen))");
            builder.AppendLine("$ OK FETCH completed.");

            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.SELE, GetSelectInboxResponse());
            dict.Add(TcpCommand.FETC, builder.ToString());

            using(var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMailFromHeader(9370);

                Assert.Equal(message.Headers, result.Headers);
                Assert.Equal(message.Subject, result.Subject);
                Assert.Equal(message.From, result.From);
                Assert.Equal(message.To, result.To);
            }
        }

        [Fact]
        private void ImapClient_FetchMailFromHeader_Returns_Null_If_Error_Occurs_Test()
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.SELE, GetSelectInboxResponse());
            dict.Add(TcpCommand.FETC, null);

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMailFromHeader(1050);

                Assert.Null(result);
            }
        }

        [Theory, MemberData(nameof(GetMailMessageData))]
        private void ImapClient_FetchMail_Test(int size, string mailMessage, MailMessage message, int mimePartCount)
        {
            var dict = new Dictionary<TcpCommand, string>();

            dict.Add(TcpCommand.SELE, GetSelectInboxResponse());
            dict.Add(TcpCommand.FETC, GetMailMessage(size, mailMessage));

            using (var stream = new ImapStream(dict))
            {
                var client = GetMailClient(stream);
                var result = client.FetchMail(9370);

                Assert.Equal(message.Headers, result.Headers);
                Assert.Equal(message.Subject, result.Subject);
                Assert.Equal(message.From, result.From);
                Assert.Equal(message.To, result.To);
            }
        }

        private static string GetMailMessage(int size, string mailMessage)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("* 9370 FETCH (BODY[] {" + size.ToString() + "}");
            builder.Append(mailMessage);
            builder.AppendLine(" FLAGS (\\Seen))");
            builder.AppendLine("$ OK FETCH completed.");

            return builder.ToString();
        }

        private static string GetSelectInboxResponse()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("* 9372 EXISTS");
            builder.AppendLine("* 0 RECENT");
            builder.AppendLine(@"* FLAGS (\Seen \Answered \Flagged \Deleted \Draft $MDNSent)");
            builder.AppendLine(@"* OK [PERMANENTFLAGS (\Seen \Answered \Flagged \Deleted \Draft $MDNSent)] Permanent flags");
            builder.AppendLine("* OK [UIDVALIDITY 14] UIDVALIDITY value");
            builder.AppendLine("* OK [UIDNEXT 44816] The next unique identifier value");
            builder.AppendLine("$ OK [READ-WRITE] SELECT completed.");

            return builder.ToString();
        }

        private static string GetMessageFlagsResponse()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(@"* 9370 FETCH (FLAGS (\Seen))");
            builder.AppendLine(@"* 9371 FETCH (FLAGS (\Seen))");
            builder.AppendLine(@"* 9372 FETCH (FLAGS (\Seen \Deleted))");
            builder.AppendLine("* 9373 FETCH (FLAGS ())");
            builder.AppendLine("$ OK FETCH completed.");

            return builder.ToString();
        }

        private static bool CompareMailboxes(IEnumerable<Mailbox> src, IEnumerable<Mailbox> dest)
        {
            if (src.Count() != dest.Count()) return false;

            int equalityIndex = 0;

            foreach(Mailbox srcMailbox in src)
            {
                foreach(Mailbox destMailbox in dest)
                {
                    if (srcMailbox.Equals(destMailbox))
                        equalityIndex++;
                }
            }

            return (equalityIndex == src.Count());
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
