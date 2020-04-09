using System;

namespace CoffeeBean.Mail.Iap
{
	/// <author>Himanshu Manjarawala</author>
	public class ConnectionException : ProtocolException
	{
		[NonSerialized]
		private Protocol p = null;
		
		private static readonly long serialVersionUID = 5749739604257464727L;
		
		/// <summary>
		/// Constructs an ConnectionException with no detail message.
		/// </summary>
		public ConnectionException():base() { }
		
		/// <summary>
		/// Constructs an ConnectionException with the specified detail message.
		/// </summary>
		/// <param name="message">the detail message</param>
		public ConnectionException(string message):base(message) { }
		
		/// <summary>
		/// Constructs an ConnectionException with the specified Response.
		/// </summary>
		/// <param name="p">the Protocol object</param>
		/// <param name="r">the Response</param>
		public ConnectionException(Protocol p, Response r) : base(r) { this.p = p; }
		
		public Protocol Protocol { get { return p; } }
	}
}