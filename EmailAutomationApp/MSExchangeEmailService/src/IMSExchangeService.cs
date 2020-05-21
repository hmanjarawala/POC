using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace Com.Service.Email.ExchangeServer
{
    public interface IMSExchangeService
    {
        IEnumerable<EmailMessage> ReadEmails(string folderName = "Inbox");

        EmailMessage ReadEmail(ItemId id);

        IEnumerable<Folder> GetFolders();
    }
}
