using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Service.Email.ExchangeServer
{
    public interface IMSExchangeService
    {
        IEnumerable<EmailMessage> ReadEmails(string folderName = "Inbox");
    }
}
