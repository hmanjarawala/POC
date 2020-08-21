using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Com.Service.Email.ExchangeServer
{
    internal class MSExchangeService : IMSExchangeService
    {
        private ExchangeService service;
        
        public MSExchangeService(string username, string password, string emailAddress, string serviceUri)
        {
            service = new ExchangeService
            {
                Credentials = new WebCredentials(username, password),
                Url = new Uri(serviceUri)
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

        public MSExchangeService(string username, string password, string domain, string emailAddress, string serviceUri)
        {
            service = new ExchangeService
            {
                Credentials = new WebCredentials(username, password, domain),
                Url = new Uri(serviceUri)
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
                Item item = Item.Bind(service, id);
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
                ItemView view = new ItemView(50)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, EmailMessageSchema.Sender,
                        ItemSchema.HasAttachments, EmailMessageSchema.IsRead, ItemSchema.DateTimeSent,
                        ItemSchema.DateTimeReceived)
                };
                var folders = new string[] { "Inbox", "Sent Items" };
                var results = new List<Item>();
                foreach(var folderItem in folders)
                {
                    Folder folder = getFolderByName(folderItem);
                    FindItemsResults<Item> emailMessages = service.FindItems(folder?.Id, view);
                    results.AddRange(emailMessages);
                }

                //ICollection<FolderId> folders = new Collection<FolderId> { folder.Id, WellKnownFolderName.Drafts,
                //    WellKnownFolderName.SentItems };

                results.Sort(new EmailMessageComparer());
                return results.Cast<EmailMessage>();
            }
            catch (Exception)
            {

                throw;
            }
        }

        Folder getFolderByName(string folderName)
        {
            ExtendedPropertyDefinition allFoldersType =
                new ExtendedPropertyDefinition(13825, MapiPropertyType.Integer);

            FolderId rootFolderId = new FolderId(WellKnownFolderName.MsgFolderRoot);
            //SearchFilter searchFilter1 = new SearchFilter.IsEqualTo(allFoldersType, "2");
            SearchFilter searchFilter2 = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);

            SearchFilter.SearchFilterCollection searchFilterCollection =
                new SearchFilter.SearchFilterCollection(LogicalOperator.And);
            //searchFilterCollection.Add(searchFilter1);
            searchFilterCollection.Add(searchFilter2);

            FindFoldersResults folders = service.FindFolders(rootFolderId, searchFilterCollection,
                new FolderView(int.MaxValue) { Traversal = FolderTraversal.Shallow });
            return folders.FirstOrDefault((f) => { return f.DisplayName.Equals(folderName, StringComparison.CurrentCultureIgnoreCase); });
        }

        bool sslRedirectionCallback(string serviceUri)
        {
            return serviceUri.ToLower().StartsWith("https://");
        }

        private class EmailMessageComparer : IComparer<Item>
        {
            public int Compare(Item x, Item y)
            {
                if(x == null)
                {
                    if (y == null)
                        return 0;
                    else
                        return -1;
                }
                else
                {
                    if (y == null)
                        return 1;
                    else
                        return x.DateTimeReceived.CompareTo(y.DateTimeReceived);
                }                
            }
        }
    }
}
