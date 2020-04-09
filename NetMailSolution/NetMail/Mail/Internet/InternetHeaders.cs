using CoffeeBean.Mail.Extension;
using CoffeeBean.Mail.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoffeeBean.Mail.Internet
{
    public class InternetHeaders
    {
        private static readonly bool ignoreWhiteSpaceLine = 
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.ignorewhitespacelines", false);

        protected sealed class InternetHeader : Header
        {
            /*
	         * Note that the value field from the superclass
	         * isn't used in this class.  We extract the value
	         * from the line field as needed.  We store the line
	         * rather than just the value to ensure that we can
	         * get back the exact original line, with the original
	         * whitespace, etc.
	         */
            string line;   // the entire RFC822 header "line",
                           // or null if placeholder

            /// <summary>
            /// Constructor that takes a line and splits out
            /// the header name.
            /// </summary>
            /// <param name="l">the header line</param>
            public InternetHeader(string l) : base("", "")
            {
                int i = l.IndexOf(':');
                if (i < 0)
                    name = l.Trim();
                else
                    name = l.Substring(0, i - 1).Trim();
                line = l;
            }

            /// <summary>
            /// Constructor that takes a header name and value.
            /// </summary>
            /// <param name="n">the name of the header</param>
            /// <param name="v">the value of the header</param>
            public InternetHeader(string n, string v) : base(n, "")
            {
                if (!string.IsNullOrEmpty(v))
                    line = string.Concat(n, ":", v);
                else
                    line = null;
            }

            /// <summary>
            /// Return the "value" part of the header line.
            /// </summary>
            public override string Value
            {
                get
                {
                    int i = line.IndexOf(':');
                    if (i < 0) return line;
                    //Skip whitespace after ":"
                    int j;
                    for (j = i + 1; j < line.Length; j++)
                    {
                        char c = line.CharAt(j);
                        if (!(c == ' ' || c == '\r' || c == '\n' || c == '\t'))
                            break;
                    }
                    return line.Substring(j);
                }
            }

            public string Line { get { return line; } set { line = value; } }
        }

        /*
         * The enumeration object used to enumerate an
         * InternetHeaders object.  Can return
         * either a String or a Header object.
         */
        class MatchEnumerator
        {
            private IEnumerator<InternetHeader> e;
            private string[] names; // names to match, or not
            private bool match;  // return matching headers?
            private bool want_line;  // return header lines?
            private InternetHeader next_Header;  // the next header to be returned

            /// <summary>
            /// Constructor.  Initialize the enumeration for the entire
            /// List of headers, the set of headers, whether to return
            /// matching or non-matching headers, and whether to return
            /// header lines or Header objects.
            /// </summary>
            public MatchEnumerator(IEnumerable<InternetHeader> v, string[] n, bool m, bool l)
            {
                e = v.GetEnumerator();
                names = n;
                match = m;
                want_line = l;
                next_Header = null;
            }

            /// <summary>
            /// Advances the enumerator to the next element in this enumeration?
            /// </summary>
            public bool MoveNext()
            {
                if (next_Header.IsNull())
                    next_Header = nextMatch();
                return next_Header != null;
            }

            /// <summary>
            /// Returns Current element.
            /// </summary>
            public object Current
            {
                get
                {
                    if (next_Header.IsNull())
                        next_Header = nextMatch();

                    next_Header.ThrowIfNull(new Exception("No more headers"));

                    InternetHeader h = next_Header;
                    next_Header = null;
                    if (want_line)
                        return h.Line;
                    else
                        return new Header(h.Name, h.Value);
                }
            }

            private InternetHeader nextMatch()
            {
                next:
                while (e.MoveNext())
                {
                    InternetHeader h = e.Current;
                    // skip "place holder" headers
                    if (string.IsNullOrEmpty(h.Line)) continue;

                    // if no names to match against, return appropriately
                    if (names.IsNull() || names.Length == 0)
                        return match ? null : h;

                    // check whether this header matches any of the names
                    for(int i = 0; i < names.Length; i++)
                    {
                        if (names[i].EqualsIgnoreCase(h.Name))
                        {
                            if (match) return h;
                            else
                                // found a match, but we're
                                // looking for non-matches.
                                // try next header.
                                goto next;
                        }
                    }

                    // found no matches.  if that's what we wanted, return it.
                    if (!match) return h;
                }
                return null;
            }
        }

        class MatchStringEnumerator : MatchEnumerator, IEnumerator<string>
        {
            public MatchStringEnumerator(IEnumerable<InternetHeader> v, string[] n, bool m) : base(v, n, m, true) { }

            public new string Current
            {
                get
                {
                    return (string)base.Current;
                }
            }

            public void Dispose() { }
            public void Reset() { }
        }

        class MatchHeaderEnumerator : MatchEnumerator, IEnumerator<Header>
        {
            public MatchHeaderEnumerator(IEnumerable<InternetHeader> v, string[] n, bool m) : base(v, n, m, false) { }

            public new Header Current
            {
                get
                {
                    return (Header)base.Current;
                }
            }

            public void Dispose() { }

            public void Reset() { }
        }

        private IList<InternetHeader> headers;

        public InternetHeaders()
        {
            headers = new List<InternetHeader>(40);
            headers.Add(new InternetHeader("Return-Path", null));
            headers.Add(new InternetHeader("Received", null));
            headers.Add(new InternetHeader("Resent-Date", null));
            headers.Add(new InternetHeader("Resent-From", null));
            headers.Add(new InternetHeader("Resent-Sender", null));
            headers.Add(new InternetHeader("Resent-To", null));
            headers.Add(new InternetHeader("Resent-Cc", null));
            headers.Add(new InternetHeader("Resent-Bcc", null));
            headers.Add(new InternetHeader("Resent-Message-Id", null));
            headers.Add(new InternetHeader("Date", null));
            headers.Add(new InternetHeader("From", null));
            headers.Add(new InternetHeader("Sender", null));
            headers.Add(new InternetHeader("Reply-To", null));
            headers.Add(new InternetHeader("To", null));
            headers.Add(new InternetHeader("Cc", null));
            headers.Add(new InternetHeader("Bcc", null));
            headers.Add(new InternetHeader("Message-Id", null));
            headers.Add(new InternetHeader("In-Reply-To", null));
            headers.Add(new InternetHeader("References", null));
            headers.Add(new InternetHeader("Subject", null));
            headers.Add(new InternetHeader("Comments", null));
            headers.Add(new InternetHeader("Keywords", null));
            headers.Add(new InternetHeader("Errors-To", null));
            headers.Add(new InternetHeader("MIME-Version", null));
            headers.Add(new InternetHeader("Content-Type", null));
            headers.Add(new InternetHeader("Content-Transfer-Encoding", null));
            headers.Add(new InternetHeader("Content-MD5", null));
            headers.Add(new InternetHeader(":", null));
            headers.Add(new InternetHeader("Content-Length", null));
            headers.Add(new InternetHeader("Status", null));
        }

        /// <summary>
        /// Read and parse the given RFC822 message stream till the 
        /// blank line separating the header from the body. The input 
        /// stream is left positioned at the start of the body. The 
        /// header lines are stored internally.
        /// 
        /// No placeholder entries are inserted; the original order of
        /// the headers is preserved.
        /// 
        /// </summary>
        /// <param name="inputStream">RFC822 input stream</param>
        /// <exception cref="MessagingException">for any I/O error reading the stream</exception>
        public InternetHeaders(Stream inputStream) : this(inputStream, false) { }

        /// <summary>
        /// Read and parse the given RFC822 message stream till the 
        /// blank line separating the header from the body. The input 
        /// stream is left positioned at the start of the body. The 
        /// header lines are stored internally.
        /// 
        /// No placeholder entries are inserted; the original order of
        /// the headers is preserved.
        /// 
        /// </summary>
        /// <param name="inputStream">RFC822 input stream</param>
        /// <param name="allowutf8">if UTF-8 encoded headers are allowed</param>
        /// <exception cref="MessagingException">for any I/O error reading the stream</exception>
        public InternetHeaders(Stream inputStream, bool allowutf8)
        {
            headers = new List<InternetHeader>(40);
            LoadFromStream(inputStream, allowutf8);
        }

        /// <summary>
        /// Read and parse the given RFC822 message stream till the
        /// blank line separating the header from the body.Store the
        /// header lines inside this InternetHeaders object. The order
        /// of header lines is preserved.
        /// 
        /// Note that the header lines are added into this InternetHeaders
        /// object, so any existing headers in this object will not be
        /// affected.Headers are added to the end of the existing list
        /// of headers, in order.
        /// </summary>
        /// <param name="inputStream">RFC822 input stream</param>
        /// <exception cref="MessagingException">for any I/O error reading the stream</exception>
        public void LoadFromStream(Stream inputStream)
        {
            LoadFromStream(inputStream, false);
        }

        /// <summary>
        /// Read and parse the given RFC822 message stream till the
        /// blank line separating the header from the body.Store the
        /// header lines inside this InternetHeaders object. The order
        /// of header lines is preserved.
        /// 
        /// Note that the header lines are added into this InternetHeaders
        /// object, so any existing headers in this object will not be
        /// affected.Headers are added to the end of the existing list
        /// of headers, in order.
        /// </summary>
        /// <param name="inputStream">RFC822 input stream</param>
        /// <param name="allowutf8">if UTF-8 encoded headers are allowed</param>
        /// <exception cref="MessagingException">for any I/O error reading the stream</exception>
        public void LoadFromStream(Stream inputStream, bool allowutf8)
        {
            // Read header lines until a blank line. It is valid
            // to have BodyParts with no header lines.
            string line;
            StreamReader reader;

            reader = (allowutf8) ? new StreamReader(inputStream, Encoding.UTF8) : new StreamReader(inputStream, true);

            string prevline = null; // the previous header line, as a string
            // a buffer to accumulate the header in, when we know it's needed
            StringBuilder builder = new StringBuilder();

            try
            {
                // if the first line being read is a continuation line,
                // we ignore it if it's otherwise empty or we treat it as
                // a non-continuation line if it has non-whitespace content
                bool first = true;
                do
                {
                    line = reader.ReadLine();
                    if(!string.IsNullOrEmpty(line) && 
                        (line.StartsWith(" ") || line.StartsWith("\t")))
                    {
                        // continuation of header
                        if (!string.IsNullOrEmpty(prevline))
                        {
                            builder.Append(prevline);
                            prevline = null;
                        }
                        if (first)
                        {
                            string lt = line.Trim();
                            if (lt.Length > 0)
                                builder.Append(lt);
                        }
                        else
                        {
                            if (builder.Length > 0)
                                builder.Append(Environment.NewLine);
                            builder.Append(line);
                        }
                    }
                    else
                    {
                        // new header
                        if (!string.IsNullOrEmpty(prevline))
                            AddHeaderLine(prevline);
                        else if(builder.Length > 0)
                        {
                            // store previous header first
                            AddHeaderLine(builder.ToString());
                            builder.Length = 0;
                        }
                        prevline = line;
                    }
                    first = false;
                } while (!string.IsNullOrEmpty(line) && !isEmpty(line));
            }
            catch (IOException iox)
            {
                throw new MessagingException("Error in input stream", iox);
            }
        }

        /// <summary>
        /// Return all the values for the specified header. The
        /// values are String objects.  Returns <code>null</code>
        /// if no headers with the specified name exist.
        /// 
        /// </summary>
        /// <param name="name">header name</param>
        /// <returns>array of header values, or null if none</returns>
        public string[] GetHeader(string name)
        {
            IEnumerator<InternetHeader> e = headers.GetEnumerator();

            IList<string> v = new List<string>();

            while (e.MoveNext())
            {
                InternetHeader h = e.Current;
                if (name.EqualsIgnoreCase(h.Name) && !string.IsNullOrEmpty(h.Line))
                    v.Add(h.Value);
            }

            if (v.Count == 0)
                return null;
            return v.ToArray();
        }

        /// <summary>
        /// Is this line an empty (blank) line?
        /// </summary>
        private static bool isEmpty(string line)
        {
            return (line.Length == 0 || (ignoreWhiteSpaceLine && line.Trim().Length == 0));
        }

        /// <summary>
        /// Get all the headers for this header name, returned as a single
        /// String, with headers separated by the delimiter. If the
        /// delimiter is <code>null</code>, only the first header is 
        /// returned.  Returns <code>null</code>
        /// if no headers with the specified name exist.
        /// 
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="delimiter">delimiter</param>
        /// <returns>            the value fields for all headers with
        /// 			this name, or null if none</returns>
        public string GetHeader(string name, string delimiter)
        {
            string[] s = GetHeader(name);

            if (s.IsNull()) return null;

            if (s.Length == 1 || string.IsNullOrEmpty(delimiter))
                return s[0];
            StringBuilder builder = new StringBuilder(s[0]);
            for(int i = 1; i < s.Length; i++)
            {
                builder.Append(delimiter);
                builder.Append(s[i]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Change the first header line that matches name
        /// to have value, adding a new header if no existing header
        /// matches. Remove all matching headers but the first.
        /// 
        /// Note that RFC822 headers can only contain US-ASCII characters
        /// 
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">header value</param>
        public void SetHeader(string name, string value)
        {
            bool found = false;

            for (int i = 0; i < headers.Count; i++)
            {
                InternetHeader h = headers[i];
                if (name.EqualsIgnoreCase(h.Name))
                {
                    if (!found)
                    {
                        int j;
                        if (!string.IsNullOrEmpty(h.Line) && (j = h.Line.IndexOf(':')) >= 0)
                        {
                            h.Line = string.Concat(h.Line.Substring(0, j + 1), " ", value);
                            // preserves capitalization, spacing
                        }
                        else
                        {
                            h.Line = string.Concat(name, ": ", value);
                        }
                        found = true;
                    }
                    else
                    {
                        headers.RemoveAt(i);
                        i--;
                    }
                }
            }

            if (!found)
                AddHeader(name, value);
        }

        /// <summary>
        /// Add a header with the specified name and value to the header list. 
        /// 
        /// The current implementation knows about the preferred order of most
        /// well-known headers and will insert headers in that order.In
        /// addition, it knows that<code> Received</code> headers should be
        /// inserted in reverse order (newest before oldest), and that they
        /// should appear at the beginning of the headers, preceeded only by
        /// a possible<code> Return-Path</code> header.  
        /// 
        /// Note that RFC822 headers can only contain US-ASCII characters.
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">header value</param>
        public void AddHeader(string name, string value)
        {
            int pos = headers.Count;
            bool addReverse = name.EqualsIgnoreCase("Received") || name.EqualsIgnoreCase("Return-Path");

            if (addReverse) pos = 0;

            for(int i = headers.Count - 1; i >= 0; i--)
            {
                InternetHeader h = headers[i];
                if (name.EqualsIgnoreCase(h.Name))
                {
                    if (addReverse)
                        pos = i;
                    else
                    {
                        headers.Insert(i + 1, new InternetHeader(name, value));
                        return;
                    }
                }

                // marker for default place to add new headers
                if (!addReverse && h.Name.Equals(":"))
                    pos = i;
            }

            headers.Insert(pos, new InternetHeader(name, value));
        }

        /// <summary>
        /// Remove all header entries that match the given name
        /// </summary>
        /// <param name="name">header name</param>
        public void RemoveHeader(string name)
        {
            for(int i = 0; i < headers.Count; i++)
            {
                InternetHeader h = headers[i];
                if (name.EqualsIgnoreCase(h.Name))
                {
                    h.Line = null;
                }
            }
        }

        /// <summary>
        /// Return all the headers as an Enumeration of 
        /// <seealso cref="CoffeeBean.Mail.Header"/> objects.
        /// </summary>
        /// <returns>Enumeration of Header objects</returns>
        public IEnumerator<Header> GetAllHeaders()
        {
            return GetNonMatchingHeaders(null);
        }

        /// <summary>
        /// Return all matching <seealso cref="CoffeeBean.Mail.Header"/> objects.
        /// </summary>
        /// <param name="names">the headers to return</param>
        /// <returns>Enumeration of matching Header objects</returns>
        public IEnumerator<Header> GetMatchingHeaders(string[] names)
        {
            return new MatchHeaderEnumerator(headers, names, true);
        }

        /// <summary>
        /// Return all non-matching <seealso cref="CoffeeBean.Mail.Header"/> objects.
        /// </summary>
        /// <param name="names">the headers to not return</param>
        /// <returns>Enumeration of non-matching Header objects</returns>
        public IEnumerator<Header> GetNonMatchingHeaders(string[] names)
        {
            return new MatchHeaderEnumerator(headers, names, false);
        }

        /// <summary>
        /// Add an RFC822 header line to the header store.
        /// If the line starts with a space or tab (a continuation line),
        /// add it to the last header line in the list.  Otherwise,
        /// append the new header line to the list.
        /// 
        /// Note that RFC822 headers can only contain US-ASCII characters
        /// 
        /// </summary>
        /// <param name="line">raw RFC822 header line</param>
        public void AddHeaderLine(string line)
        {
            try
            {
                char c = line.CharAt(0);
                if (c == ' ' || c == '\t')
                {
                    InternetHeader h = headers[headers.Count - 1];
                    h.Line = string.Concat(h.Line, Environment.NewLine, line);
                }
                else
                    headers.Add(new InternetHeader(line));
            }
            catch { }
        }

        /// <summary>
        /// Return all the header lines as an Enumeration of Strings.
        /// </summary>
        /// <returns>Enumeration of Strings of all header lines</returns>
        public IEnumerator<string> GetAllHeaderLines()
        {
            return GetNonMatchingHeaderLines(null);
        }

        /// <summary>
        /// Return all matching header lines as an Enumeration of Strings.
        /// </summary>
        /// <param name="names">the headers to return</param>
        /// <returns>Enumeration of Strings of all matching header lines</returns>
        public IEnumerator<string> GetMatchingHeaderLines(string[] names)
        {
            return new MatchStringEnumerator(headers, names, true);
        }

        /// <summary>
        /// Return all non-matching header lines
        /// </summary>
        /// <param name="names">the headers to not return</param>
        /// <returns>Enumeration of Strings of all non-matching header lines</returns>
        public IEnumerator<string> GetNonMatchingHeaderLines(string[] names)
        {
            return new MatchStringEnumerator(headers, names, false);
        }
    }
}
