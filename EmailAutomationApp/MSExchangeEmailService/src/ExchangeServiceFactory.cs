using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Service.Email.ExchangeServer
{
    public class ExchangeServiceFactory
    {
        public static IMSExchangeService GetExchangeWebClient(string username, string password)
        {
            return new MSExchangeService(username, password);
        }

        public static IMSExchangeService GetExchangeClient(string username, string password, string domain)
        {
            return new MSExchangeService(username, password, domain);
        }
    }
}
