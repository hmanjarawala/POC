namespace Com.Service.Email.ExchangeServer
{
    public class ExchangeServiceFactory
    {
        public static IMSExchangeService GetExchangeWebClient(string username, string password, string uri)
        {
            return new MSExchangeService(username, password, uri);
        }

        public static IMSExchangeService GetExchangeClient(string username, string password, string domain, string uri)
        {
            return new MSExchangeService(username, password, domain, uri);
        }
    }
}
