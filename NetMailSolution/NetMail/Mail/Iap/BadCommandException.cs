namespace CoffeeBean.Mail.Iap
{
    /// <author>Himanshu Manjarawala</author>
    public class BadCommandException : ProtocolException
    {
        private static readonly long serialVersionUID = 5769722539397237515L;

        /// <summary>
        /// Constructs an BadCommandException with no detail message.
        /// </summary>
        public BadCommandException() : base() { }

        /// <summary>
        /// Constructs an BadCommandException with the specified detail message.
        /// </summary>
        /// <param name="message">the detail message</param>
        public BadCommandException(string message) : base(message) { }

        /// <summary>
        /// Constructs an BadCommandException with the specified Response.
        /// </summary>
        /// <param name="r">the Respone</param>
        public BadCommandException(Response r) : base(r) { }
    }
}
