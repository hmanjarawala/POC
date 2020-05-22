namespace Com.Service.Email.ExchangeServer
{
    public class ExchangeServiceFactory
    {
        public static IMSExchangeService GetExchangeWebClient(string username, string password, string emailAddress)
        {
            return new MSExchangeService(username, password, emailAddress);
        }

        public static IMSExchangeService GetExchangeClient(string username, string password, string domain, string emailAddress)
        {
            return new MSExchangeService(username, password, domain, emailAddress);
        }
    }
}
