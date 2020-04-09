namespace CoffeeBean.Mail
{
    public class PasswordAuthentication
    {
        readonly string username;
        readonly string password;

        public PasswordAuthentication(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public string UserName { get { return username; } }

        public string Password { get { return password; } }
    }
}
