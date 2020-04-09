using System.IO;

namespace CoffeeBean.Mail.Iap
{
	/// <summary>
	/// An interface for objects that provide data dynamically for use in
	/// a literal protocol element.
	/// </summary>
	/// <author>Himanshu Manjarawala</author>
	public interface ILiteral
	{
		/// <summary>
		/// Return the size of the data.
		/// </summary>
		int Size { get; }
		
		/// <summary>
		/// Write the data to the Stream.
		/// </summary>
		/// <param name="s">the Stream</param>
		/// <exception cref="IOException">for I/O errors</exception>
		void WriteTo(StreamWriter s);
	}
}