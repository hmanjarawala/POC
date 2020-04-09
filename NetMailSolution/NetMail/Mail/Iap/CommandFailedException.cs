namespace CoffeeBean.Mail.Iap
{
	/// <author>Himanshu Manjarawala</author>
	public class CommandFailedException : ProtocolException
	{
		private static readonly long serialVersionUID = 793932807880443631L;
		
		/// <summary>
		/// Constructs an CommandFailedException with no detail message.
		/// </summary>
		public CommandFailedException():base() { }
		
		/// <summary>
		/// Constructs an CommandFailedException with the specified detail message.
		/// </summary>
		/// <param name="message">the detail message</param>
		public CommandFailedException(string message):base(message) { }
		
		/// <summary>
		/// Constructs an CommandFailedException with the specified detail message.
		/// </summary>
		/// <param name="r">the Response</param>
		public CommandFailedException(Response r):base(r) { }
	}
}