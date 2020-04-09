namespace CoffeeBean.Mail.Internet
{
    /**
     * The exception thrown due to an error in parsing RFC822 
     * or MIME headers, including multipart bodies.
     *
     * @author Himanshu Manjarawala
     */
    public class ParseException : MessagingException
    {
        static readonly long serialVersionUID = 2649991205183658089L;

        /// <summary>
        /// Constructs a ParseException with no detail message.
        /// </summary>
        public ParseException() : base() { }

        /// <summary>
        /// Constructs a ParseException with the specified detail message.
        /// </summary>
        /// <param name="message">the detail message</param>
        public ParseException(string message) : base(message) { }
    }
}
