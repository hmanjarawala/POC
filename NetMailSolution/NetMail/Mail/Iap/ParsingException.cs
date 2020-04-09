namespace CoffeeBean.Mail.Iap
{
	/// <author>Himanshu Manjarawala</author>
	public class ParsingException : ProtocolException
	{
		private static readonly long serialVersionUID = 7756119840142724839L;
		
		/// <summary>
		/// Constructs an ParsingException with no detail message.
		/// </summary>
		public ParsingException():base() { }
		
		/// <summary>
		/// Constructs an ParsingException with the specified detail message.
		/// </summary>
		/// <param name="message">the detail message</param>
		public ParsingException(string message):base(message) { }
		
		/// <summary>
		/// Constructs an ParsingException with the specified detail message.
		/// </summary>
		/// <param name="r">the Response</param>
		public ParsingException(Response r):base(r) { }
	}
}