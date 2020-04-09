using System.IO;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// A special IOException that indicates a failure to decode data due
    /// to an error in the formatting of the data.  This allows applications
    /// to distinguish decoding errors from other I/O errors.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class DecodingException : IOException
    {
        private static readonly long serialVersionUID = -6913647794421459390L;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">the exception message</param>
        public DecodingException(string message) : base(message) { }
    }
}
