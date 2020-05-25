namespace Com.Service.Email.ExchangeServer
{
    public class ExchangeServiceFactory
    {
        public static IMSExchangeService GetExchangeWebClient(string username, string password, string emailAddress, string serviceUri)
        {
            return new MSExchangeService(username, password, emailAddress, serviceUri);
        }

        public static IMSExchangeService GetExchangeClient(string username, string password, string domain, string emailAddress, string serviceUri)
        {
            return new MSExchangeService(username, password, domain, emailAddress, serviceUri);
        }
    }
}
