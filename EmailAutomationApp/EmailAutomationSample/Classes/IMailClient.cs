using System.Collections.Generic;

namespace EmailAutomationSample.Classes
{
    public enum MailFlag { All = 1, Seen, Unseen };

    public interface IMailClient
    {
        string GetHostName();

        string Login(string username, string password);

        string GetTotalMail(out int totalMailCount, string mailbox = "Inbox", string flag = "messages");

        string GetAllMessageFlags(MailFlag flag, out IEnumerable<int> mailIndexes, string mailbox = "Inbox");

        string LogOut();
    }
}
