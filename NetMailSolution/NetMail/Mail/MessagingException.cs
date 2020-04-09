using System;
using System.Runtime.Serialization;
using System.Text;

namespace CoffeeBean.Mail
{
    /**
     * The base class for all exceptions thrown by the Messaging classes
     *
     * @author Himanshu Manjarawala
     */ 
    public class MessagingException : Exception
    {
        private Exception _innerException;

        static readonly long serialVersionUID = -2569192289819959253L;

        /// <summary>
        /// Constructs a MessagingException with no detail message.
        /// </summary>
        public MessagingException() : base() { }

        /// <summary>
        /// Constructs a MessagingException with the specified detail message.
        /// </summary>
        /// <param name="message">the detail message</param>
        public MessagingException(string message) : base(message) { }

        /// <summary>
        /// Constructs a MessagingException with the specified 
        /// Exception and detail message. The specified exception is chained
        /// to this exception.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="innerException">the embedded exception</param>
        public MessagingException(string message, Exception innerException) : base(message, innerException) { _innerException = innerException; }

        /// <summary>
        /// Constructs a MessagingException with serialized data.
        /// </summary>
        /// <param name="info">the serialized object data about the exception being thrown</param>
        /// <param name="context">the StreamingContext that contains contextual information about the source or destination</param>
        public MessagingException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Override toString method to provide information on
        /// nested exceptions.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            Exception e = _innerException;
            if (e == null) return s;
            StringBuilder builder = new StringBuilder(s ?? "");
            while (e != null)
            {
                builder.Append(";\n Nested exception is: \r\t");
                if(e is MessagingException)
                {
                    MessagingException mex = (MessagingException)e;
                    builder.Append(mex.BaseToString());
                    e = mex._innerException;
                }
                else
                {
                    builder.Append(e.ToString());
                    e = null;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the "ToString" information for this exception, 
        /// without any information on nested exceptions.
        /// </summary>
        private string BaseToString() { return base.ToString(); }
    }
}
