using System;
using CoffeeBean.Mail.Extension;
using System.Text;

namespace CoffeeBean.Mail.Internet
{
    /// <summary>
    /// This class tokenizes RFC822 and MIME headers into the basic
    /// symbols specified by RFC822 and MIME.
    /// 
    /// This class handles folded headers (ie headers with embedded
    /// CRLF SPACE sequences). The folds are removed in the returned
    /// tokens. 
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class HeaderTokenizer
    {
        public class Token
        {
            private int type;

            private string value;

            /// <summary>
            /// Token type indicating an ATOM.
            /// </summary>
            public static readonly int ATOM = -1;

            /// <summary>
            /// Token type indicating a quoted string. The value 
            /// field contains the string without the quotes.
            /// </summary>
            public static readonly int QUOTEDSTRING = -2;

            /// <summary>
            /// Token type indicating a comment. The value field 
            /// contains the comment string without the comment 
            /// start and end symbols.
            /// </summary>
            public static readonly int COMMENT = -3;

            /// <summary>
            /// Token type indicating end of input.
            /// </summary>
            public static readonly int EOF = -4;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="type">Token type</param>
            /// <param name="value">Token value</param>
            public Token(int type, string value)
            {
                this.type = type;
                this.value = value;
            }

            /// <summary>
            /// Return the type of the token. If the token represents a
            /// delimiter or a control character, the type is that character
            /// itself, converted to an integer.Otherwise, it's value is 
            /// one of the following:
            /// <ul>
            /// <li><code>ATOM</code> A sequence of ASCII characters 
            /// delimited by either SPACE, CTL, "(", &lt;"&gt; or the 
            /// specified SPECIALS
            /// <li><code>QUOTEDSTRING</code> A sequence of ASCII characters 
            /// within quotes
            /// <li><code>COMMENT</code> A sequence of ASCII characters 
            /// within "(" and ")".
            /// <li><code>EOF</code> End of header
            /// </ul>
            /// </summary>
            /// <returns>Token Type</returns>
            public int Type
            {
                get { return type; }
            }

            /// <summary>
            /// Returns the value of the token just read. When the current
            /// token is a quoted string, this field contains the body of the
            /// string, without the quotes.When the current token is a comment,
            /// this field contains the body of the comment.
            /// </summary>
            /// <returns>Token Value</returns>
            public string Value
            {
                get { return value; }
            }
        }

        private string @string; // the string to be tokenized
        private bool skipComments; // should comments be skipped ?
        private string delimiters; // delimiter string
        private int currentPos; // current parse position
        private int maxPos; // string length
        private int nextPos; // track start of next Token for next()
        private int peekPos; // track start of next Token for peek()

        /// <summary>
        /// RFC822 specials
        /// </summary>
        public static readonly string RFC822 = "()<>@,;:\\\"\t .[]";

        /// <summary>
        /// MIME specials
        /// </summary>
        public static readonly string MIME = "()<>@,;:\\\"\t []/?=";

        /// <summary>
        /// The EOF Token
        /// </summary>
        private static readonly Token EOFToken = new Token(Token.EOF, null);

        /// <summary>
        /// Constructor that takes a rfc822 style header.
        /// </summary>
        /// <param name="header">The rfc822 header to be tokenized</param>
        /// <param name="delimiters">Set of delimiter characters 
        /// 				to be used to delimit ATOMS. These
        /// 				are usually <code>RFC822</code> or 
        /// 				<code>MIME</code>
        /// </param>
        /// <param name="skipComments">If true, comments are skipped and
        /// 				not returned as tokens
        /// </param>
        public HeaderTokenizer(string header, string delimiters, bool skipComments)
        {
            @string = header ?? "";
            this.skipComments = skipComments;
            this.delimiters = delimiters;
            currentPos = nextPos = peekPos = 0;
            maxPos = @string.Length;
        }

        /// <summary>
        /// Constructor. Comments are ignored and not returned as tokens
        /// </summary>
        /// <param name="header">The header that is tokenized</param>
        /// <param name="delimiters">The delimiters to be used</param>
        public HeaderTokenizer(string header, string delimiters) : this(header, delimiters, true) { }

        /// <summary>
        /// Constructor. The RFC822 defined delimiters - RFC822 - are
        /// used to delimit ATOMS.Also comments are skipped and not
        /// returned as tokens
        /// </summary>
        /// <param name="header">the header string</param>
        public HeaderTokenizer(string header) : this(header, RFC822) { }

        /// <summary>
        /// Parses the next token from this String.
        /// 
        /// Clients sit in a loop calling NextToken() to parse successive
        /// tokens until an EOF Token is returned.
        /// </summary>
        /// <returns>the next Token</returns>
        /// <exception cref="ParseException">if the parse fails</exception>
        public Token NextToken()
        {
            return NextToken('\0', false);
        }

        /// <summary>
        /// Parses the next token from this String.
        /// If endOfAtom is not NUL, the token extends until the
        /// endOfAtom character is seen, or to the end of the header.
        /// This method is useful when parsing headers that don't
        /// obey the MIME specification, e.g., by failing to quote
        /// parameter values that contain spaces.
        /// </summary>
        /// <param name="endOfAtom">if not NUL, character marking end of token</param>
        /// <returns>the next Token</returns>
        /// <exception cref="ParseException">if the parse fails</exception>
        public Token NextToken(char endOfAtom)
        {
            return NextToken(endOfAtom, false);
        }

        /// <summary>
        /// Parses the next token from this String.
        /// endOfAtom is handled as above.If keepEscapes is true,
        /// any backslash escapes are preserved in the returned string.
        /// This method is useful when parsing headers that don't
        /// obey the MIME specification, e.g., by failing to escape
        /// backslashes in the filename parameter.
        /// </summary>
        /// <param name="endOfAtom">if not NUL, character marking end of token</param>
        /// <param name="keepEscapes">keep all backslashes in returned string</param>
        /// <returns>the next Token</returns>
        /// <exception cref="ParseException">if the parse fails</exception>
        public Token NextToken(char endOfAtom, bool keepEscapes)
        {
            Token tk;

            currentPos = nextPos; // setup CurrenntPos
            tk = getNext(endOfAtom, keepEscapes);
            nextPos = peekPos = currentPos; // update nextPos and peekPos
            return tk;
        }

        /// <summary>
        /// Peek at the next token, without actually removing the token
        /// from the parse stream.Invoking this method multiple times
        /// will return successive tokens, until<code> NextToken()</code> is
        /// called.
        /// </summary>
        /// <returns>the next Token</returns>
        /// <exception cref="ParseException">if the parse fails</exception>
        public Token PeekToken()
        {
            Token tk;

            currentPos = peekPos; //setup currentPos
            tk = getNext('\0', false);
            peekPos = currentPos; //update peekPos
            return tk;
        }

        /// <summary>
        /// Return the rest of the Header
        /// </summary>
        /// <returns><see cref="string"/>rest of header. null is returned if we are
        ///         already at end of header</returns>
        public string GetReminder()
        {
            if (nextPos >= @string.Length)
                return null;
            return @string.Substring(nextPos);
        }

        private Token getNext(char endOfAtom, bool keepEscapes)
        {
            // If we're already at end of string, return EOF
            if (currentPos >= maxPos) return EOFToken;

            // Skip white-space, position currentPos beyond the space
            if (skipWhiteSpace() == Token.EOF) return EOFToken;

            char c;
            int start;
            bool filter = false;

            c = @string.CharAt(currentPos);

            // Check or Skip comments and position currentPos
            // beyond the comment
            while (c == '(')
            {
                // Parsing comment ..
                int nesting;
                for(start = ++currentPos, nesting=1;
                    nesting>0 && currentPos < maxPos; currentPos++)
                {
                    c = @string.CharAt(currentPos);
                    if (c == '\\') // Escape sequence
                    {
                        currentPos++; // skip the escaped character
                        filter = true;
                    }
                    else if (c == '\r')
                        filter = true;
                    else if (c == '(')
                        nesting++;
                    else if (c == ')')
                        nesting--;
                }
                if (nesting != 0)
                    throw new ParseException("Unbalanced comments");

                if (!skipComments)
                {
                    // Return the comment, if we are asked to.
                    // Note that the comment start & end markers are ignored.
                    string s;

                    if (filter)
                        s = filterToken(@string, start, currentPos - 1, keepEscapes);
                    else
                        s = @string.Substring(start, currentPos - 1);

                    return new Token(Token.COMMENT, s);
                }

                // Skip any whitespace after the comment.
                if (skipWhiteSpace() == Token.EOF)
                    return EOFToken;
                c = @string.CharAt(currentPos);
            }
            // Check for quoted-string and position currentPos 
            //  beyond the terminating quote
            if (c == '"')
            {
                currentPos++; // skip initial quote
                return collectString('"', keepEscapes);
            }

            // Check for SPECIAL or CTL
            if (c < 040 || c >= 0177 || delimiters.IndexOf(c) >= 0)
            {
                if (endOfAtom > 0 && c != endOfAtom)
                {
                    // not expecting a special character here,
                    // pretend it's a quoted string
                    return collectString(endOfAtom, keepEscapes);
                }
                currentPos++; // re-position currentPos
                char[] ch = new char[1];
                ch[0] = c;
                return new Token((int)c, new String(ch));
            }

            // Check for ATOM
            for (start = currentPos; currentPos < maxPos; currentPos++)
            {
                c = @string.CharAt(currentPos);
                // ATOM is delimited by either SPACE, CTL, "(", <"> 
                // or the specified SPECIALS
                if (c < 040 || c >= 0177 || c == '(' || c == ' ' ||
                    c == '"' || delimiters.IndexOf(c) >= 0)
                {
                    if (endOfAtom > 0 && c != endOfAtom)
                    {
                        // not the expected atom after all;
                        // back up and pretend it's a quoted string
                        currentPos = start;
                        return collectString(endOfAtom, keepEscapes);
                    }
                    break;
                }
            }
            return new Token(Token.ATOM, @string.Substring(start, currentPos));
        }

        private Token collectString(char eos, bool keepEscaps)
        {
            int start;
            bool filter = false;

            for (start = currentPos; currentPos < maxPos; currentPos++)
            {
                char c = @string.CharAt(currentPos);
                if (c == '\\')
                {
                    currentPos++;
                    filter = true;
                }
                else if (c == '\r')
                    filter = true;
                else if (c == eos)
                {
                    currentPos++;

                    string s1;

                    if (filter)
                        s1 = filterToken(@string, start, currentPos - 1, keepEscaps);
                    else
                        s1 = @string.Substring(start, currentPos - 1);

                    if (c != '"')
                    {
                        s1 = trimWhiteSpace(s1);
                        currentPos--;
                    }

                    return new Token(Token.QUOTEDSTRING, s1);
                }
            }

            // ran off the end of the string

            // if we're looking for a matching quote, that's an error
            if (eos == '"')
                throw new ParseException("Unbalanced quoted string");

            // otherwise, just return whatever's left
            string s;
            if (filter)
                s = filterToken(@string, start, currentPos, keepEscaps);
            else
                s = @string.Substring(start, currentPos);
            s = trimWhiteSpace(s);
            return new Token(Token.QUOTEDSTRING, s);
        }

        // Skip SPACE, HT, CR and NL
        private int skipWhiteSpace()
        {
            char c;

            for (; currentPos < maxPos; currentPos++)
                if (((c = @string.CharAt(currentPos)) != ' ') && 
                    (c != '\t') && (c != '\r') && (c != '\n'))
                    return currentPos;
            return Token.EOF;
        }

        // Trim SPACE, HT, CR and NL from end of string
        private static string trimWhiteSpace(string s)
        {
            int i;
            char c;

            for (i = s.Length - 1; i >= 0; i--)
            {
                if (((c = s.CharAt(i)) != ' ') &&
                    (c != '\t') && (c != '\r') && (c != '\n'))
                    break;
            }
            if (i <= 0) return string.Empty;
            else
                return s.Substring(0, i + 1);
        }

        private static string filterToken(string s, int start, int end, bool keepEscaps)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            bool gotEscape = false;
            bool gotCR = false;

            for (int i = start; i < end; i++)
            {
                c = s.CharAt(i);
                if(c=='\n' && gotCR)
                {
                    // This LF is part of an unescaped 
                    // CRLF sequence (i.e, LWSP). Skip it.
                    gotCR = false;
                    continue;
                }

                gotCR = false;
                if (!gotEscape)
                {
                    // Previous character was NOT '\'
                    if (c == '\\') // skip this character
                        gotEscape = true;
                    else if (c == '\r') // skip this character
                        gotCR = true;
                    else // append this character
                        sb.Append(c);
                }
                else
                {
                    // Previous character was '\'. So no need to 
                    // bother with any special processing, just 
                    // append this character.  If keepEscapes is
                    // set, keep the backslash.  IE6 fails to escape
                    // backslashes in quoted strings in HTTP headers,
                    // e.g., in the filename parameter.

                    if (keepEscaps) sb.Append('\\');
                    sb.Append(c);
                    gotEscape = false;
                }
            }
            return sb.ToString();
        }
    }
}
