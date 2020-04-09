namespace CoffeeBean.Mail.Internet
{
    /**
     * The exception thrown when a wrongly formatted address is encountered.
     *
     * @author Himanshu Manjarawala
     */
    public class AddressException : ParseException
    {
        static readonly long serialVersionUID = 7134583443539323120L;

        /// <summary>
        /// The string being parsed.
        /// </summary>
        protected string reference = null;

        /// <summary>
        /// The index in the string where the error occurred, or -1 if not known.
        /// </summary>
        protected int position = -1;

        /// <summary>
        /// Constructs an AddressException with no detail message.
        /// </summary>
        public AddressException() : base() { }

        /// <summary>
        /// Constructs an AddressException with the specified detail message.
        /// </summary>
        /// <param name="message">the detail message</param>
        public AddressException(string message) : base(message) { }

        /// <summary>
        /// Constructs an AddressException with the specified detail message
        /// and reference info.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="reference">the string being parsed</param>
        public AddressException(string message, string reference) : base(message) { this.reference = reference; }

        /// <summary>
        /// Constructs an AddressException with the specified detail message
        /// and reference info.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="reference">the string being parsed</param>
        /// <param name="position">the position of the error</param>
        public AddressException(string message, string reference, int position) : this(message, reference) { this.position = position; }

        /// <summary>
        /// Get the string that was being parsed when the error was detected
        /// (null if not relevant).
        /// </summary>
        /// <returns>the string that was being parsed</returns>
        public string Reference { get { return reference; } }

        /// <summary>
        /// Get the position with the reference string where the error was
        /// detected (-1 if not relevant).
        /// </summary>
        /// <returns>the position within the string of the error</returns>
        public int Position { get { return position; } }

        public override string ToString()
        {
            string s = base.ToString();
            if (reference == null) return s;
            s = string.Concat(s, string.Format(" in string '{0}'", reference));
            if (position < 0) return s;
            return string.Concat(s, string.Format(" at position {0}", position));
        }
    }
}
