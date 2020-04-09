using System.Collections;

namespace CoffeeBean.Mail
{
    public sealed class Session
    {
        Util.Properties props;
        IAuthenticator authenticator;
        ArrayList providers = new ArrayList();
        Hashtable providerByProtocol = new Hashtable();
        static Session defaultSession;
        static readonly object sync = new object();

        public Util.Properties Properties { get { return props; } }

        private Session(Util.Properties props, IAuthenticator authenticator)
        {
            this.props = props;
            this.authenticator = authenticator;
        }

        public static Session GetInstance(Util.Properties props)
        {
            return new Session(props, null);
        }

        public static Session GetInstance(Util.Properties props, IAuthenticator authenticator)
        {
            return new Session(props, authenticator);
        }

        public static Session GetDefaultInstance(Util.Properties props, IAuthenticator authenticator)
        {
            lock (sync)
            {
                if (defaultSession == null)
                    defaultSession = new Session(props, authenticator);
                return defaultSession;
            }
        }

        public void AddProvider(Provider provider)
        {
            lock (sync)
            {
                providers.Add(provider);
                if (!providerByProtocol.ContainsKey(provider.Protocol))
                    providerByProtocol.Add(provider.Protocol, provider);
            }
        }

        private void loadProviders()
        {
            if(providers.Count == 0)
            {
                //AddProvider(new Provider(Provider.ProviderType.Store, Protocol.Imap));
                //AddProvider(new Provider(Provider.ProviderType.Store, Protocol.Imaps));
                //AddProvider(new Provider(Provider.ProviderType.Store, Protocol.Pop));
                //AddProvider(new Provider(Provider.ProviderType.Store, Protocol.Pops));
                //AddProvider(new Provider(Provider.ProviderType.Transport, Protocol.Smtp));
                //AddProvider(new Provider(Provider.ProviderType.Transport, Protocol.Smtps));
            }
        }
    }
}
