using CoffeeBean.Mail.Extension;
using CoffeeBean.Mail.Util;
using System.Collections;
using System.IO;
using System.Text;

namespace CoffeeBean.Mail.Iap
{
    /// <author>Himanshu Manjarawala</author>
	public class Argument
	{
		protected ArrayList items;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public Argument() { items = new ArrayList(1); }
		
		/// <summary>
		/// Append the given Argument to this Argument. All items
		/// from the source argument are copied into this destination
		/// argument.
		/// </summary>
		/// <param name="arg">the Argument to append</param>
		/// <returns>this</returns>
		public Argument Append(Argument arg)
		{
			items.AddRange(arg.items);
			return this;
		}
		
		/// <summary>
		/// Write out given string as an ASTRING, depending on the type
		/// of the characters inside the string. The string should
		/// contain only ASCII characters.
		/// </summary>
		/// <param name="s">String to write out</param>
		/// <returns>this</returns>
		public Argument WriteString(string s)
		{
			items.Add(new AString(ASCIIUtility.GetBytes(s)));
			return this;
		}
		
		/// <summary>
		/// Convert the given string into bytes in the specified
		/// charset, and write the bytes out as an ASTRING.
		/// </summary>
		/// <param name="s">String to write out</param>
		/// <param name="encoding">the encoding</param>
		/// <exception cref="ArgumentException">for bad encoding</exception>
		/// <returns>this</returns>
		public Argument WriteString(string s, string encoding)
		{
			if(encoding == null)
				WriteString(s);
			else
			{
				Encoding e = Encoding.GetEncoding(encoding);
				items.Add(new AString(e.GetBytes(s)));
			}
			return this;
		}
		
		/// <summary>
		/// Convert the given string into bytes in the specified
		/// charset, and write the bytes out as an ASTRING.
		/// </summary>
		/// <param name="s">String to write out</param>
		/// <param name="encoding">the encoding</param>
		/// <returns>this</returns>
		public Argument WriteString(string s, Encoding encoding)
		{
			if(encoding.IsNull())
				WriteString(s);
			else
				items.Add(new AString(encoding.GetBytes(s)));
			return this;
		}
		
		/// <summary>
		/// Write out given string as an NSTRING, depending on the type
		/// of the characters inside the string. The string should
		/// contain only ASCII characters.
		/// </summary>
		/// <param name="s">String to write out</param>
		/// <returns>this</returns>
		public Argument WriteNString(string s)
		{
			if(s == null)
				items.Add(new NString(null));
			else
				items.Add(new NString(ASCIIUtility.GetBytes(s)));
			return this;
		}
		
		/// <summary>
		/// Convert the given string into bytes in the specified
		/// charset, and write the bytes out as an NSTRING.
		/// </summary>
		/// <param name="s">String to write out</param>
		/// <param name="encoding">the encoding</param>
		/// <exception cref="ArgumentException">for bad encoding</exception>
		/// <returns>this</returns>
		public Argument WriteNString(string s, string encoding)
		{
			if(s == null)
				items.Add(new NString(null));
			else if(encoding == null)
				WriteString(s);
			else
			{
				Encoding e = Encoding.GetEncoding(encoding);
				items.Add(new AString(e.GetBytes(s)));
			}
			return this;
		}
		
		/// <summary>
		/// Convert the given string into bytes in the specified
		/// charset, and write the bytes out as an NSTRING.
		/// </summary>
		/// <param name="s">String to write out</param>
		/// <param name="encoding">the encoding</param>
		/// <returns>this</returns>
		public Argument WriteNString(string s, Encoding encoding)
		{
			if(s == null)
				items.Add(new NString(null));
			else if(encoding.IsNull())
				WriteString(s);
			else
				items.Add(new AString(encoding.GetBytes(s)));
			return this;
		}
		
		/// <summary>
		/// Write out given byte[] as a Literal.
		/// </summary>
		/// <param name="b">byte[] to write out</param>
		/// <returns>this</returns>
		public Argument WriteBytes(byte[] b)
		{
			items.Add(b);
			return this;
		}
		
		/// <summary>
		/// Write out given MemoryStream as a Literal.
		/// <param name="b">StreamWriter to write out</param>
		/// </summary>
		/// <returns>this</returns>
		public Argument WriteBytes(StreamWriter b)
		{
			items.Add(b);
			return this;
		}
		
		/// <summary>
		/// Write out given data as a Literal.
		/// <param name="b">Literal representing data to write out</param>
		/// </summary>
		/// <returns>this</returns>
		public Argument WriteBytes(ILiteral b)
		{
			items.Add(b);
			return this;
		}
		
		/// <summary>
		/// Write out given string as an Atom. Note that an Atom can contain only
		/// certain US-ASCII characters.  No validation is done on the characters 
		/// in the string.
		/// <param name="s">String</param>
		/// </summary>
		/// <returns>this</returns>
		public Argument WriteAtom(string s)
		{
			items.Add(new Atom(s));
			return this;
		}

        /// <summary>
        /// Write out number.
        /// </summary>
        /// <param name="i">number</param>
        /// <returns>this</returns>
        public Argument WriteNumber(int i)
        {
            items.Add(i);
            return this;
        }

        /// <summary>
        /// Write out number.
        /// </summary>
        /// <param name="i">number</param>
        /// <returns>this</returns>
        public Argument WriteNumber(long i)
        {
            items.Add(i);
            return this;
        }

        /// <summary>
        /// Write out as parenthesised list.
        /// </summary>
        /// <param name="c">the Argument</param>
        /// <returns>this</returns>
        public Argument WriteArgument(Argument c)
        {
            items.Add(c);
            return this;
        }

        /// <summary>
        /// Write out all the buffered items into the stream.
        /// </summary>
        /// <param name="protocol"></param>
        /// <exception cref="IOException">the I/O error</exception>
        /// <exception cref="ProtocolException">the Protocol error</exception>
        public void Write(Protocol protocol)
        {
            int size = items.IsNull() ? 0 : items.Count;
            StreamWriter writer = getStreamWriter(protocol);

            for (int i = 0; i < size; i++)
            {
                if (i > 0)  // write delimiter if not the first item
                    writer.Write(' ');

                object o = items[i];

                if (o is Atom)
                {
                    writer.Write(((Atom)o).String);
                }
                else if (o is int)
                {
                    writer.Write(((int)o));
                }
                else if (o is long)
                {
                    writer.Write(((long)o));
                }
                else if (o is AString)
                {
                    astring(((AString)o).Bytes, protocol);
                }
                else if (o is NString)
                {
                    nstring(((NString)o).Bytes, protocol);
                }
                else if (o is byte[])
                {
                    literal((byte[])o, protocol);
                }
                else if (o is StreamWriter)
                {
                    literal((StreamWriter)o, protocol);
                }
                else if (o is ILiteral)
                {
                    literal((ILiteral)o, protocol);
                }
                else if (o is Argument)
                {
                    writer.Write('('); // open parans
                    ((Argument)o).Write(protocol);
                    writer.Write(')'); // close parans
                }
            }
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private void astring(byte[] bytes, Protocol p)
        {
            nstring(bytes, p, false);
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private void nstring(byte[] bytes, Protocol p)
        {
            if (bytes.IsNull())
            {
                StreamWriter writer = getStreamWriter(p);
                writer.Write("NIL");
            }
            else nstring(bytes, p, true);
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private void nstring(byte[] bytes, Protocol p, bool doQuote)
        {
            StreamWriter writer = getStreamWriter(p);
            int len = bytes.Length;

            // If length is greater than 1024 bytes, send as literal
            if (len > 1024)
            {
                literal(bytes, p);
                return;
            }

            // if 0 length, send as quoted-string
            bool quote = len == 0 ? true : doQuote;
            bool escape = false;
            bool utf8 = p.SupportsUtf8();

            byte b;
            for(int i=0;i< len; i++)
            {
                b = bytes[i];

                if(b=='\0' || b=='\r' || b== '\n' || 
                    (!utf8 && ((b & 0xff) > 0177)))
                {
                    // NUL, CR or LF means the bytes need to be sent as literals
                    literal(bytes, p);
                    return;
                }
                if (b == '*' || b == '%' || b == '(' || b == ')' || b == '{' ||
                        b == '"' || b == '\\' ||
                        ((b & 0xff) <= ' ') || ((b & 0xff) > 0177))
                {
                    quote = true;
                    if (b == '"' || b == '\\') // need to escape these characters
                        escape = true;
                }
            }

            /*
	         * Make sure the (case-independent) string "NIL" is always quoted,
	         * so as not to be confused with a real NIL (handled above in nstring).
	         * This is more than is necessary, but it's rare to begin with and
	         * this makes it safer than doing the test in nstring above in case
	         * some code calls writeString when it should call writeNString.
	         */
            if (!quote && bytes.Length == 3 &&
                   (bytes[0] == 'N' || bytes[0] == 'n') &&
                   (bytes[1] == 'I' || bytes[1] == 'i') &&
                   (bytes[2] == 'L' || bytes[2] == 'l'))
                quote = true;

            if (quote) // start quote
                writer.Write('"');

            if (escape)
            {
                // already quoted
                for (int i = 0; i < len; i++)
                {
                    b = bytes[i];
                    if (b == '"' || b == '\\')
                        writer.Write('\\');
                    writer.Write(b);
                }
            }
            else
                writer.Write(bytes);

            if (quote) // end quote
                writer.Write('"');
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private void literal(byte[] b, Protocol p)
        {
            startLiteral(p, b.Length).Write(b);
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private void literal(StreamWriter b, Protocol p)
        {
            StreamReader reader = new StreamReader(startLiteral(p, (int)b.BaseStream.Length).BaseStream);
            b.Write(reader.ReadToEnd());
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private void literal(ILiteral b, Protocol p)
        {
            b.WriteTo(startLiteral(p, b.Size));
        }

        /// <exception cref="IOException"></exception>
        /// <exception cref="ProtocolException"></exception>
        private StreamWriter startLiteral(Protocol p, int size)
        {
            StreamWriter writer = getStreamWriter(p);
            bool nonSync = p.SupportsNonSyncLiterals();

            writer.Write('{');
            writer.Write(size);
            if (nonSync) // server supports non-sync literals
                writer.WriteLine("+}");
            else
                writer.WriteLine("}");
            writer.Flush();

            // If we are using synchronized literals, wait for the server's
            // continuation signal
            if (!nonSync)
            {
                for (;;)
                {
                    Response r = p.ReadResponse();
                    if (r.IsContinuation) break;
                    if (r.IsTagged) throw new LiteralException(r);
                }
            }
            return writer;
        }

        private StreamWriter getStreamWriter(Protocol p)
        {
            return p.GetWriter();
        }
    }

	class Atom
	{
		string s;
		public Atom(string s) { this.s = s; }

        public string String { get { return s; } }
	}
	
	class AString
	{
		byte[] b;
		public AString(byte[] b) { this.b = b; }

        public byte[] Bytes { get { return b; } }
    }
	
	class NString
	{
		byte[] b;
		public NString(byte[] b) { this.b = b; }

        public byte[] Bytes { get { return b; } }
	}
}