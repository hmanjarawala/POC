using System;

namespace CoffeeBean.Mail.Iap
{
    /// <author>Himanshu Manjarawala</author>
    public class ProtocolException: Exception
    {
        [NonSerialized]
        protected Response response = null;

        private static readonly long serialVersionUID = -4360500807971797439L;

        /// <summary>
        /// Constructs a ProtocolException with no detail message.
        /// </summary>
        public ProtocolException() : base() { }

        /// <summary>
        /// Constructs a ProtocolException with the specified detail message.
        /// </summary>
        /// <param name="message">the detail message</param>
        public ProtocolException(string message) : base(message) { }

        /// <summary>
        /// Constructs a ProtocolException with the specified detail message
        /// and cause.
        /// </summary>
        /// <param name="message">the detail message</param>
        /// <param name="cause">the cause</param>
        public ProtocolException(string message, Exception cause) : base(message, cause) { }

        /// <summary>
        /// Constructs a ProtocolException with the specified Response object.
        /// </summary>
        /// <param name="r">the Response</param>
        public ProtocolException(Response r) : base(r.ToString())
        {
            response = r;
        }

        /// <summary>
        /// Return the offending Response object.
        /// </summary>
        public Response Response { get { return response; } }
    }
}
