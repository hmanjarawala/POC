using EmailAutomationLibrary.Imap;
using System.Collections.Generic;
using System.Net.Mail;

namespace EmailAutomationLibrary
{
    public enum MailFlag { All = 1, Seen, Unseen };
    public interface IMailClient
    {
        string GetHostName();

        string LogIn(string username, string password);

        string LogOut();

        string GetTotalMail(out int mailCount, string mailbox = "Inbox", string flags = "messages");

        string DeleteMail(int mailIndex, string mailbox = "Inbox");

        MailMessage FetchMailFromHeader(int emailId, string mailbox = "Inbox");

        MailMessage FetchMail(int emailId, string mailbox = "Inbox");

        #region IMAP4 Related Methods        

        string GetAllMessagesFlag(out IEnumerable<int> mailIds, string mailbox = "Inbox");

        string SearchMail(string criteria, out IEnumerable<int> mailIds, string mailbox = "Inbox");

        string FetchMailboxes(out IEnumerable<Mailbox> folders, string directory = "", string pattern = "*");

        #endregion
    }
}
