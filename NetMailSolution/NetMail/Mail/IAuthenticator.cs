namespace CoffeeBean.Mail
{
    public interface IAuthenticator
    {
        PasswordAuthentication GetPasswordAuthentication();
    }
}
