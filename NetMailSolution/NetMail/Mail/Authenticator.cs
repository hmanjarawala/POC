using System;

namespace CoffeeBean.Mail
{
    public abstract class Authenticator : IAuthenticator
    {
        public abstract PasswordAuthentication GetPasswordAuthentication();
    }
}
