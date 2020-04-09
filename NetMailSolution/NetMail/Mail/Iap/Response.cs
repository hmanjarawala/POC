using CoffeeBean.Mail.Extension;
using CoffeeBean.Mail.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoffeeBean.Mail.Iap
{
    /// <summary>
    /// This class represents a response obtained from the input stream
    /// of an IMAP server.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class Response
    {
        protected int index;  // internal index (updated during the parse)
        protected int pindex; // index after parse, for reset
        protected int size;   // number of valid bytes in our buffer
        protected byte[] buffer = null;
        protected int type = 0;
        protected string tag = null;
        protected Exception ex;
        protected bool utf8;

        private static readonly int increment = 100;

        // The first and second bits indicate whether this response
        // is a Continuation, Tagged or Untagged
        public readonly static int TAG_MASK = 0x03;
        public readonly static int CONTINUATION = 0x01;
        public readonly static int TAGGED = 0x02;
        public readonly static int UNTAGGED = 0x03;

        // The third, fourth and fifth bits indicate whether this response
        // is an OK, NO, BAD or BYE response
        public readonly static int TYPE_MASK = 0x1C;
        public readonly static int OK = 0x04;
        public readonly static int NO = 0x08;
        public readonly static int BAD = 0x0C;
        public readonly static int BYE = 0x10;

        // The sixth bit indicates whether a BYE response is synthetic or real
        public readonly static int SYNTHETIC = 0x20;

        /**
         * An ATOM is any CHAR delimited by:
         * SPACE | CTL | '(' | ')' | '{' | '%' | '*' | '"' | '\' | ']'
         * (CTL is handled in readDelimString.)
         */
        private static string ATOM_CHAR_DELIM = " (){%*\"\\]";

        /**
         * An ASTRING_CHAR is any CHAR delimited by:
         * SPACE | CTL | '(' | ')' | '{' | '%' | '*' | '"' | '\'
         * (CTL is handled in readDelimString.)
         */
        private static string ASTRING_CHAR_DELIM = " (){%*\"\\";

        public Response(string s) : this(s, true) { }

        /// <summary>
        /// Constructor for testing.
        /// </summary>
        public Response(string s, bool supportUtf8)
        {
            if (supportUtf8)
                buffer = Encoding.UTF8.GetBytes(s);
            else
                buffer = Encoding.ASCII.GetBytes(s);
            size = buffer.Length;
            utf8 = supportUtf8;
            parse();
        }

        /// <summary>
        /// Read a new Response from the given Protocol
        /// </summary>
        /// <param name="p">the Protocol object</param>
        /// <exception cref="IOException">for I/O errors</exception>
        /// <exception cref="ProtocolException">for Protocol failures</exception>
        public Response(Protocol p)
        {
            // read one response into 'buffer'
            ByteArray ba = p.GetResponseBuffer();
            ByteArray response = p.GetReader().ReadResponse(ba);
            buffer = response.Bytes;
            size = buffer.Length - 2; // Skip the terminating CRLF
            utf8 = p.SupportsUtf8();
            parse();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="r">the Response to copy</param>
        public Response(Response r)
        {
            index = r.index;
            pindex = r.pindex;
            size = r.size;
            buffer = r.buffer;
            type = r.type;
            tag = r.tag;
            ex = r.ex;
            utf8 = r.utf8;
        }

        /// <summary>
        /// Return a Response object that looks like a BYE protocol response.
        /// Include the details of the exception in the response string.
        /// </summary>
        /// <param name="ex">the exception</param>
        /// <returns>the synthetic Response object</returns>
        public static Response ByeResponse(Exception ex)
        {
            string err = "* BYE CoffeeBeanMail Exception: " + ex.ToString();
            err = err.Replace('\r', ' ').Replace('\n', ' ');
            Response r = new Response(err);
            r.type |= SYNTHETIC;
            r.ex = ex;
            return r;
        }

        /// <summary>
        /// Does the server support UTF-8?
        /// </summary>
        public bool SupportUtf8 { get { return utf8; } }

        private void parse()
        {
            index = 0; // position internal index at start

            if (size == 0)  // empty line
                return;
            if (buffer[index] == '+')
            { // Continuation statement
                type |= CONTINUATION;
                index += 1; // Position beyond the '+'
                return; // return
            }
            else if (buffer[index] == '*')
            { // Untagged statement
                type |= UNTAGGED;
                index += 1; // Position beyond the '*'
            }
            else
            {  // Tagged statement
                type |= TAGGED;
                tag = ReadAtom();   // read the TAG, index positioned beyond tag
                if (tag == null)
                    tag = "";   // avoid possible NPE
            }

            int mark = index; // mark
            String s = ReadAtom();  // updates index
            if (s == null)
                s = "";     // avoid possible NPE
            if (s.EqualsIgnoreCase("OK"))
                type |= OK;
            else if (s.EqualsIgnoreCase("NO"))
                type |= NO;
            else if (s.EqualsIgnoreCase("BAD"))
                type |= BAD;
            else if (s.EqualsIgnoreCase("BYE"))
                type |= BYE;
            else
                index = mark; // reset

            pindex = index;
            return;
        }

        public void SkipSpaces()
        {
            while (index < size && buffer[index] == ' ')
                index++;
        }

        /// <summary>
        /// Skip past any spaces.  If the next non-space character is c,
        /// consume it and return true.  Otherwise stop at that point
        /// and return false.
        /// </summary>
        public bool IsNextNonSpace(char c)
        {
            SkipSpaces();
            if (index < size && buffer[index] == (byte)c)
            {
                index++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Skip to the next space, for use in error recovery while parsing.
        /// </summary>
        public void SkipToken()
        {
            while (index < size && buffer[index] != ' ')
                index++;
        }

        public void Skip(int count)
        {
            index += count;
        }

        public byte PeekByte()
        {
            if (index < size)
                return buffer[index];
            else
                return 0;       // XXX - how else to signal error?
        }

        /// <summary>
        /// Return the next byte from this Statement.
        /// </summary>
        /// <returns>the next byte</returns>
        public byte ReadByte()
        {
            if (index < size)
                return buffer[index++];
            else
                return 0;       // XXX - how else to signal error?
        }

        /// <summary>
        /// Extract an ATOM, starting at the current position. Updates
        /// the internal index to beyond the Atom.
        /// </summary>
        /// <returns>an Atom</returns>
        public string ReadAtom()
        {
            return readDelimString(ATOM_CHAR_DELIM);
        }

        /// <summary>
        /// Extract a string stopping at control characters or any
        /// character in delim.
        /// </summary>
        private string readDelimString(string delim)
        {
            SkipSpaces();

            if (index >= size) // already at end of response
                return null;

            int b;
            int start = index;
            while (index < size && ((b = (((int)buffer[index]) & 0xff)) >= ' ') &&
                   delim.IndexOf((char)b) < 0 && b != 0x7f)
                index++;
            return toString(buffer, start, index);
        }

        private string toString(byte[] buffer, int start, int end)
        {
            return utf8 ? UTF8Encoding.UTF8.GetString(buffer, start, end - start) : ASCIIUtility.ToString(buffer, start, end - start);
        }

        /// <summary>
        /// Read a string as an arbitrary sequence of characters,
        /// stopping at the delimiter  Used to read part of a
        /// response code inside [].
        /// </summary>
        /// <param name="delim">the delimiter character</param>
        /// <returns>the string</returns>
        public string ReadString(char delim)
        {
            SkipSpaces();

            if (index >= size) // already at end of response
                return null;

            int start = index;
            while (index < size && buffer[index] != delim)
                index++;

            return toString(buffer, start, index);
        }

        public string[] ReadStringList()
        {
            return readStringList(false);
        }

        public string[] ReadAtomStringList()
        {
            return readStringList(true);
        }

        private string[] readStringList(bool atom)
        {
            SkipSpaces();

            if (buffer[index] != '(')
            { // not what we expected
                return null;
            }
            index++; // skip '('

            // to handle buggy IMAP servers, we tolerate multiple spaces as
            // well as spaces after the left paren or before the right paren
            List<string> result = new List<string>();
            while (!IsNextNonSpace(')'))
                result.Add(atom ? ReadAtomString() : ReadString());

            return result.ToArray();
        }

        /// <summary>
        /// Extract a NSTRING, starting at the current position. Return it as
        /// a String. The sequence 'NIL' is returned as null
        /// 
        /// NSTRING := QuotedString | Literal | "NIL"
        /// </summary>
        /// <returns>a String</returns>
        public string ReadString()
        {
            return Convert.ToString(parseString(false, true));
        }

        /// <summary>
        /// Extract an ASTRING, starting at the current position
        /// and return as a String.An ASTRING can be a QuotedString, a
        /// Literal or an Atom (plus ']').
        /// 
        /// Any errors in parsing returns null
        /// 
        /// ASTRING := QuotedString | Literal | 1*ASTRING_CHAR
        /// </summary>
        /// <returns>a String</returns>
        public string ReadAtomString()
        {
            return Convert.ToString(parseString(true, true));
        }

        /// <summary>
        /// Extract a NSTRING, starting at the current position. Return it as
        /// a MemoryStream. The sequence 'NIL' is returned as null
        /// 
        /// NSTRING := QuotedString | Literal | "NIL"
        /// </summary>
        /// <returns>a MemoryStream</returns>
        public MemoryStream ReadBytes()
        {
            ByteArray ba = ReadByteArray();
            if (ba.IsNotNull())
                return ba.ToStream();
            else
                return null;
        }

        /// <summary>
        /// Extract a NSTRING, starting at the current position. Return it as
        /// a ByteArray. The sequence 'NIL' is returned as null
        /// 
        /// NSTRING := QuotedString | Literal | "NIL"
        /// </summary>
        /// <returns>a ByteArray</returns>
        public ByteArray ReadByteArray()
        {
            /*
	         * Special case, return the data after the continuation uninterpreted.
	         * It's usually a challenge for an AUTHENTICATE command.
	         */
            if (IsContinuation)
            {
                SkipSpaces();
                return new ByteArray(buffer, index, size - index);
            }
            return (ByteArray)parseString(false, false);
        }

        /// <summary>
        /// Generic parsing routine that can parse out a Quoted-String,
        /// Literal or Atom and return the parsed token as a String
        /// or a ByteArray. Errors or NIL data will return null.
        /// </summary>
        private object parseString(bool parseAtoms, bool returnString)
        {
            byte b;
            // Skip leading spaces
            SkipSpaces();

            b = buffer[index];

            if (b == '"')
            {// QuotedString
                index++; // skip the quote
                int start = index;
                int copyto = index;

                while (index < size && (b = buffer[index]) != '"')
                {
                    if (b == '\\') // skip escaped byte
                        index++;
                    if (index != copyto)
                    { // only copy if we need to
                      // Beware: this is a destructive copy. I'm 
                      // pretty sure this is OK, but ... ;>
                        buffer[copyto] = buffer[index];
                    }
                    copyto++;
                    index++;
                }
                if (index >= size)
                {
                    // didn't find terminating quote, something is seriously wrong
                    //throw new ArrayIndexOutOfBoundsException(
                    //		    "index = " + index + ", size = " + size);
                    return null;
                }
                else
                    index++; // skip past the terminating quote

                if (returnString)
                    return toString(buffer, start, copyto);
                else
                    return new ByteArray(buffer, start, copyto - start);
            }
            else if (b == '{')
            {// Literal
                int start = ++index; // note the start position

                while (buffer[index] != '}')
                    index++;

                int count = 0;
                try
                {
                    count = ASCIIUtility.ParseInt(buffer, start, index);
                }
                catch (FormatException nex)
                {
                    // throw new ParsingException();
                    return null;
                }

                start = index + 3; // skip "}\r\n"
                index = start + count; // position index to beyond the literal

                if (returnString) // return as String
                    return toString(buffer, start, start + count);
                else
                    return new ByteArray(buffer, start, count);
            }
            else if (parseAtoms)
            { // parse as ASTRING-CHARs
                int start = index;  // track this, so that we can use to
                                    // creating ByteArrayInputStream below.
                String s = readDelimString(ASTRING_CHAR_DELIM);
                if (returnString)
                    return s;
                else  // *very* unlikely
                    return new ByteArray(buffer, start, index);
            }
            else if (b == 'N' || b == 'n')
            { // the only valid value is 'NIL'
                index += 3; // skip past NIL
                return null;
            }
            return null; // Error
        }

        /// <summary>
        /// Extract an integer, starting at the current position. Updates the
        /// internal index to beyond the number. Returns -1 if  a number was 
        /// not found.
        /// </summary>
        /// <returns>a number</returns>
        public int ReadNumber()
        {
            // Skip leading spaces
            SkipSpaces();

            int start = index;
            while (index < size && char.IsDigit((char)buffer[index]))
                index++;

            if (index > start)
            {
                try
                {
                    return ASCIIUtility.ParseInt(buffer, start, index);
                }
                catch (FormatException) { }
            }

            return -1;
        }

        /// <summary>
        /// Extract a long number, starting at the current position. Updates the
        /// internal index to beyond the number. Returns -1 if a long number
        /// was not found.
        /// </summary>
        /// <returns>a long</returns>
        public long ReadLong()
        {
            // Skip leading spaces
            SkipSpaces();

            int start = index;
            while (index < size && char.IsDigit((char)buffer[index]))
                index++;

            if (index > start)
            {
                try
                {
                    return ASCIIUtility.ParseLong(buffer, start, index);
                }
                catch (FormatException) { }
            }

            return -1;
        }

        public int Type { get { return type; } }

        public bool IsContinuation
        {
            get
            {
                return ((type & TAG_MASK) == CONTINUATION);
            }
        }

        public bool IsTagged
        {
            get
            {
                return ((type & TAG_MASK) == TAGGED);
            }
        }

        public bool IsUnTagged
        {
            get
            {
                return ((type & TAG_MASK) == UNTAGGED);
            }
        }

        public bool IsOK
        {
            get
            {
                return ((type & TYPE_MASK) == OK);
            }
        }

        public bool IsNO
        {
            get
            {
                return ((type & TYPE_MASK) == NO);
            }
        }

        public bool IsBAD
        {
            get
            {
                return ((type & TYPE_MASK) == BAD);
            }
        }

        public bool IsBYE
        {
            get
            {
                return ((type & TYPE_MASK) == BYE);
            }
        }

        public bool IsSynthetic
        {
            get
            {
                return ((type & SYNTHETIC) == SYNTHETIC);
            }
        }

        /// <summary>
        /// Return the tag, if this is a tagged statement.
        /// </summary>
        public string Tag { get { return tag; } }

        /// <summary>
        /// Return the rest of the response as a string, usually used to
        /// return the arbitrary message text after a NO response.
        /// </summary>
        public string Rest
        {
            get
            {
                SkipSpaces();
                return toString(buffer, index, size);
            }
        }

        /// <summary>
        /// Return the exception for a synthetic BYE response.
        /// </summary>
        public Exception Exception { get { return ex; } }

        /// <summary>
        /// Reset pointer to beginning of response.
        /// </summary>
        public void Reset()
        {
            index = pindex;
        }

        public override string ToString()
        {
            return toString(buffer, 0, size);
        }
    }
}
