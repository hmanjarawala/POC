using Com.Service.Email.ExchangeServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeServerClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //string _uri = "https://outlook.office365.com/EWS/Exchange.asmx";
            IMSExchangeService service = ExchangeServiceFactory.GetExchangeWebClient("h.manjarawala@hotmail.com", "gr@ndf@ther","h.manjarawala@hotmail.com");

            var mails = service.ReadEmails();

            foreach(var mail in mails)
            {
                Console.WriteLine(string.Format("{0}:{1}", mail.Id, mail.Subject));
            }

            var folders = service.GetFolders();

            foreach(var folder in folders)
            {
                Console.WriteLine(string.Format("{0}:{1}", folder.Id, folder.DisplayName));
            }
            Console.Read();
        }
    }
}
