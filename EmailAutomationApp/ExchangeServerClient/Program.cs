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
            IMSExchangeService service = ExchangeServiceFactory.GetExchangeClient("himanshu.manjarawala", "a0dF@theR9", "MEA");

            var mails = service.ReadEmails();
        }
    }
}
