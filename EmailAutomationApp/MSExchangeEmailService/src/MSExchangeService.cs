using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Service.Email.ExchangeServer
{
    internal class MSExchangeService : IMSExchangeService
    {
        private ExchangeService service;

        public MSExchangeService(string username, string password)
        {
            service = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            service.Url = new Uri(@"https://outlook.office365.com/owa/in.ey.com");

            ExchangeCredentials credentials = new WebCredentials(username, password);
            service.Credentials = credentials;
        }

        public MSExchangeService(string username, string password, string domain)
        {
            service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            service.Url = new Uri(@"https://outlook.office365.com/owa/in.ey.com");

            ExchangeCredentials credentials = new WebCredentials(username, password, domain);
            service.Credentials = credentials;
        }

        public IEnumerable<EmailMessage> ReadEmails(string folderName = "Inbox")
        {
            try
            {
                Folder folder = getFolderByName(folderName);
                FindItemsResults<Item> emailMessages = service.FindItems(folder.Id, new ItemView(10));

                return emailMessages.Cast<EmailMessage>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private Folder getFolderByName(string folderName)
        {
            SearchFilter filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);
            FindFoldersResults folders = service.FindFolders(WellKnownFolderName.MsgFolderRoot, filter,
                new FolderView(int.MaxValue) { Traversal = FolderTraversal.Deep });
            return folders.FirstOrDefault((f) => { return f.DisplayName.Equals(folderName, StringComparison.CurrentCultureIgnoreCase); });
        }
    }
}
