namespace CoffeeBean.Mail.Iap
{
	/// <author>Himanshu Manjarawala</author>
	public class LiteralException : ProtocolException
	{
		private static readonly long serialVersionUID = -6919179828339609913L;
		
		/// <summary>
		/// Constructs a LiteralException with the specified Response object.
		/// </summary>
		/// <param name="r">the response object</param>
		public LiteralException(Response r):base(r) { }
	}
}