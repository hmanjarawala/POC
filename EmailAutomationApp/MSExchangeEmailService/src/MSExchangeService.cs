using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Service.Email.ExchangeServer
{
    internal class MSExchangeService : IMSExchangeService
    {
        private ExchangeService service;

        public MSExchangeService(string username, string password, string emailAddress)
        {
            service = new ExchangeService
            {
                Credentials = new WebCredentials(username, password),
                Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx")
            };

            //try
            //{
            //    service.AutodiscoverUrl(emailAddress, sslRedirectionCallback);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        public MSExchangeService(string username, string password, string domain, string emailAddress)
        {
            service = new ExchangeService
            {
                Credentials = new WebCredentials(username, password, domain),
                Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx")
            };
            //try
            //{
            //    service.AutodiscoverUrl(emailAddress, sslRedirectionCallback);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

        }

        public IEnumerable<Folder> GetFolders()
        {
            FolderView view = new FolderView(int.MaxValue);
            // Create an extended property definition for the PR_ATTR_HIDDEN property,
            // so that your results will indicate whether the folder is a hidden folder.
            ExtendedPropertyDefinition isHiddenProp = new ExtendedPropertyDefinition(0x10f4, MapiPropertyType.Boolean);
            // As a best practice, limit the properties returned to only those required.
            // In this case, return the folder ID, DisplayName, and the value of the isHiddenProp
            // extended property.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, FolderSchema.DisplayName, isHiddenProp);
            // Indicate a Traversal value of Deep, so that all subfolders are retrieved.
            view.Traversal = FolderTraversal.Deep;
            // Call FindFolders to retrieve the folder hierarchy, starting with the MsgFolderRoot folder.
            // This method call results in a FindFolder call to EWS.
            FindFoldersResults findFolderResults = service.FindFolders(WellKnownFolderName.MsgFolderRoot, view);

            return findFolderResults.ToList();
        }

        public EmailMessage ReadEmail(ItemId id)
        {
            try
            {
                Item item = Item.Bind(service, id, PropertySet.FirstClassProperties);
                return EmailMessage.Bind(service, item.Id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<EmailMessage> ReadEmails(string folderName = "Inbox")
        {
            try
            {
                ItemView view = new ItemView(10)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, EmailMessageSchema.Sender,
                        ItemSchema.HasAttachments, EmailMessageSchema.IsRead, ItemSchema.DateTimeSent,
                        ItemSchema.DateTimeReceived)
                };
                Folder folder = getFolderByName(folderName);
                FindItemsResults<Item> emailMessages = service.FindItems(folder.Id, view);

                return emailMessages.Cast<EmailMessage>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        Folder getFolderByName(string folderName)
        {
            SearchFilter filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);
            FindFoldersResults folders = service.FindFolders(WellKnownFolderName.MsgFolderRoot, filter,
                new FolderView(int.MaxValue) { Traversal = FolderTraversal.Deep });
            return folders.FirstOrDefault((f) => { return f.DisplayName.Equals(folderName, StringComparison.CurrentCultureIgnoreCase); });
        }

        bool sslRedirectionCallback(string serviceUri)
        {
            return serviceUri.ToLower().StartsWith("https://");
        }
    }
}
