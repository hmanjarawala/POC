using CoffeeBean.Mail.Extension;
using CoffeeBean.Mail.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CoffeeBean.Mail.Internet
{
    public class MimeUtility
    {
        // This class cannot be instantiated
        private MimeUtility() { }

        public static readonly int ALL = -1;

        // cached map of whether a charset is compatible with ASCII
        // Dictionary<string,bool>
        private static readonly IDictionary<string, bool> nonAsciiEncodingDictionary =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        private static readonly bool decodeStrict = 
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.decodetext.strict", true);
        private static readonly bool encodeEolStrict =
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.encodeeol.strict", false);
        private static readonly bool ignoreUnknownEncoding =
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.ignoreunknownencoding", false);
        private static readonly bool allowUtf8 =
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.allowutf8", false);
        /*
         * The following two properties allow disabling the Fold()
         * and Unfold() methods and reverting to the previous behavior.
         * They should never need to be changed and are here only because
         * of my paranoid concern with compatibility.
         */
        private static readonly bool foldEncodedWords =
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.foldencodedwords", false);
        private static readonly bool foldText =
            PropertyUtil.GetBooleanSystemPropertyValue("mail.mime.foldtext", true);

        private static IDictionary<string, string> mime2net;
        private static IDictionary<string, string> net2mime;

        private static string defaultNETEncoding;
        private static string defaultMIMEEncoding;

        static MimeUtility()
        {
            net2mime = new Dictionary<string, string>(40);
            mime2net = new Dictionary<string, string>(14);

            // Use this class's assembly to load the mapping file
            using (Stream s = typeof(MimeUtility).Assembly.GetManifestResourceStream("CoffeeBean.Resources.CoffeeBean.Encoding.map"))
            {
                if (s.IsNotNull())
                {
                    using (var reader = new StreamReader(s))
                    {
                        // Load the Net-to-MIME charset mapping table
                        loadMappings(reader, net2mime);

                        // Load the MIME-to-Net charset mapping table
                        loadMappings(reader, mime2net);
                    }
                }
            }

            // If we didn't load the tables, e.g., because we didn't have
            // permission, load them manually.  The entries here should be
            // the same as the default CoffeeBean.Encoding.map.
            if (net2mime.IsEmpty())
            {
                net2mime.Add("8859_1", "ISO-8859-1");
                net2mime.Add("iso8859_1", "ISO-8859-1");
                net2mime.Add("iso8859-1", "ISO-8859-1");

                net2mime.Add("8859_2", "ISO-8859-2");
                net2mime.Add("iso8859_2", "ISO-8859-2");
                net2mime.Add("iso8859-2", "ISO-8859-2");

                net2mime.Add("8859_3", "ISO-8859-3");
                net2mime.Add("iso8859_3", "ISO-8859-3");
                net2mime.Add("iso8859-3", "ISO-8859-3");

                net2mime.Add("8859_4", "ISO-8859-4");
                net2mime.Add("iso8859_4", "ISO-8859-4");
                net2mime.Add("iso8859-4", "ISO-8859-4");

                net2mime.Add("8859_5", "ISO-8859-5");
                net2mime.Add("iso8859_5", "ISO-8859-5");
                net2mime.Add("iso8859-5", "ISO-8859-5");

                net2mime.Add("8859_6", "ISO-8859-6");
                net2mime.Add("iso8859_6", "ISO-8859-6");
                net2mime.Add("iso8859-6", "ISO-8859-6");

                net2mime.Add("8859_7", "ISO-8859-7");
                net2mime.Add("iso8859_7", "ISO-8859-7");
                net2mime.Add("iso8859-7", "ISO-8859-7");

                net2mime.Add("8859_8", "ISO-8859-8");
                net2mime.Add("iso8859_8", "ISO-8859-8");
                net2mime.Add("iso8859-8", "ISO-8859-8");

                net2mime.Add("8859_9", "ISO-8859-9");
                net2mime.Add("iso8859_9", "ISO-8859-9");
                net2mime.Add("iso8859-9", "ISO-8859-9");

                net2mime.Add("sjis", "Shift_JIS");
                net2mime.Add("jis", "ISO-2022-JP");
                net2mime.Add("iso2022jp", "ISO-2022-JP");
                net2mime.Add("euc_jp", "euc-jp");
                net2mime.Add("koi8_r", "koi8-r");
                net2mime.Add("euc_cn", "euc-cn");
                net2mime.Add("euc_tw", "euc-tw");
                net2mime.Add("euc_kr", "euc-kr");
            }
            if (mime2net.IsEmpty())
            {
                net2mime.Add("iso-2022-cn", "ISO2022CN");
                net2mime.Add("iso-2022-kr", "ISO2022KR");
                net2mime.Add("utf-8", "UTF8");
                net2mime.Add("utf8", "UTF8");
                net2mime.Add("ja_jp.iso2022-7", "ISO2022JP");
                net2mime.Add("ja_jp.eucjp", "EUCJIS");
                net2mime.Add("euc-kr", "KSC5601");
                net2mime.Add("euckr", "KSC5601");
                net2mime.Add("us-ascii", "ISO-8859-1");
                net2mime.Add("x-us-ascii", "ISO-8859-1");
                net2mime.Add("gb2312", "GB18030");
                net2mime.Add("cp936", "GB18030");
                net2mime.Add("ms936", "GB18030");
                net2mime.Add("gbk", "GB18030");
            }
        }

        /// <summary>
        /// A utility method to quote a word, if the word contains any
        /// characters from the specified 'specials' list.
        /// 
        /// The <code>HeaderTokenizer</code> class defines two special
        /// sets of delimiters - MIME and RFC 822.
        /// 
        /// This method is typically used during the generation of 
        /// RFC 822 and MIME header fields.
        /// </summary>
        /// <param name="word">word to be quoted</param>
        /// <param name="specials">the set of special characters</param>
        /// <returns>the possibly quoted word</returns>
        /// <see cref="HeaderTokenizer#MIME"/>
        /// <see cref="HeaderTokenizer#RFC822"/>
        public static string Quote(string word, string specials)
        {
            int len = word.IsNull() ? 0 : word.Length;
            if (len == 0) return "\"\""; // an empty string is handled specially

            /*
	         * Look for any "bad" characters, Escape and
	         *  quote the entire string if necessary.
	         */
            bool needQuoting = false;
            for (int i = 0; i < len; i++)
            {
                char c = word.CharAt(i);
                if (c == '"' || c == '\\' || c == '\r' || c == '\n')
                {
                    // need to escape them and then quote the whole string
                    StringBuilder sb = new StringBuilder(len + 3);
                    sb.Append('"');
                    sb.Append(word.Substring(0, i));
                    int lastc = 0;

                    for (int j = i; j < len; j++)
                    {
                        char cc = word.CharAt(j);
                        if ((cc == '"') || (cc == '\\') ||
                        (cc == '\r') || (cc == '\n'))
                            if (cc == '\n' && lastc == '\r')
                                ;   // do nothing, CR was already escaped
                            else
                                sb.Append('\\');    // Escape the character
                        sb.Append(cc);
                        lastc = cc;
                    }
                    sb.Append('"');
                    return sb.ToString();
                }
                else if (c < 040 || (c >= 0177 && !allowUtf8) || 
                    specials.IndexOf(c) >= 0)
                    // These characters cause the string to be quoted
                    needQuoting = true;
            }

            if (needQuoting)
            {
                StringBuilder sb = new StringBuilder(len + 2);
                sb.Append('"').Append(word).Append('"');
                return sb.ToString();
            }
            else
                return word;
        }

        /// <summary>
        /// Fold a string at linear whitespace so that each line is no longer
        /// than 76 characters, if possible.  If there are more than 76
        /// non-whitespace characters consecutively, the string is folded at
        /// the first whitespace after that sequence.  The parameter
        /// <code>used</code> indicates how many characters have been used in
        /// the current line; it is usually the length of the header name.
        /// 
        /// Note that line breaks in the string aren't escaped; they probably
        /// should be.
        /// </summary>
        /// <param name="used">characters used in line so far</param>
        /// <param name="s">the string to fold</param>
        /// <returns>the folded string</returns>
        public static string Fold(int used, string s)
        {
            if (!foldText) return s;

            int end;
            char c;
            // Strip trailing spaces and newlines
            for (end = s.Length - 1; end >= 0; end--)
            {
                c = s.CharAt(end);
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    break;
            }
            if (end != s.Length - 1) s = s.Substring(0, end + 1);

            // if the string fits now, just return it
            if (used + s.Length <= 76)
                return makesafe(s);

            // have to actually fold the string
            StringBuilder sb = new StringBuilder(s.Length + 4);
            char lastc = (char)0;
            while(used + s.Length > 76)
            {
                int lastspace = -1;
                for(int i = 0; i < s.Length; i++)
                {
                    if (lastspace != -1 && used + i > 76)
                        break;
                    c = s.CharAt(i);
                    if (c == ' ' || c == '\t')
                        if (!(lastc == ' ' || lastc == '\t'))
                            lastspace = i;
                    lastc = c;
                }
                if (lastspace == -1)
                {
                    // no space, use the whole thing
                    sb.Append(s);
                    s = "";
                    used = 0;
                    break;
                }
                sb.Append(s.Substring(0, lastspace));
                sb.Append("\r\n");
                lastc = s.CharAt(lastspace);
                sb.Append(lastc);
                s = s.Substring(lastspace + 1);
                used = 1;
            }
            sb.Append(s);
            return makesafe(sb.ToString());
        }

        /// <summary>
        /// If the String or StringBuilder has any embedded newlines,
        /// make sure they're followed by whitespace, to prevent header
        /// injection errors.
        /// </summary>
        private static string makesafe(string s)
        {
            int i;
            for (i = 0; i < s.Length; i++)
            {
                char c = s.CharAt(i);
                if (c == '\r' || c == '\n')
                    break;
            }
            if (i == s.Length)    // went through whole string with no CR or LF
                return s;

            // read the lines in the string and reassemble them,
            // eliminating blank lines and inserting whitespace as necessary
            StringBuilder sb = new StringBuilder(s.Length + 1);
            string line;
            using (StringReader r = new StringReader(s))
            {
                while((line = r.ReadLine()).IsNotNull())
                {
                    if(line.Trim().Length == 0)
                        continue;	// ignore empty lines
                    if (sb.Length > 0)
                    {
                        sb.Append("\r\n");
                        Debug.Assert(line.Length > 0); // proven above
                        char c = line.CharAt(0);
                        if (c != ' ' && c != '\t') sb.Append(' ');
                    }
                    sb.Append(line);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Unfold a folded header.  Any line breaks that aren't escaped and
        /// are followed by whitespace are removed.
        /// </summary>
        /// <param name="s">the string to unfold</param>
        /// <returns>the unfolded string</returns>
        public static string Unfold(string s)
        {
            if (!foldText) return s;

            StringBuilder sb = null;
            int i;
            while((i = s.IndexOfAny("\r\n".ToCharArray())) >= 0)
            {
                int start = i;
                int slen = s.Length;
                i++;		// skip CR or NL
                if (i < slen && s.CharAt(i - 1) == '\r' && s.CharAt(i) == '\n')
                    i++;	// skip LF
                if (start > 0 && s.CharAt(start - 1) == '\\')
                {
                    // there's a backslash before the line break
                    // strip it out, but leave in the line break
                    if (sb == null)
                        sb = new StringBuilder(s.Length);
                    sb.Append(s.Substring(0, start - 1));
                    sb.Append(s.Substring(start, i));
                    s = s.Substring(i);
                }
                else
                {
                    char c;
                    // if next line starts with whitespace,
                    // or at the end of the string, remove the line break
                    // XXX - next line should always start with whitespace
                    if (i >= slen || (c = s.CharAt(i)) == ' ' || c == '\t')
                    {
                        if (sb == null)
                            sb = new StringBuilder(s.Length);
                        sb.Append(s.Substring(0, start));
                        s = s.Substring(i);
                    }
                    else {
                        // it's not a continuation line, just leave in the newline
                        if (sb == null)
                            sb = new StringBuilder(s.Length);
                        sb.Append(s.Substring(0, i));
                        s = s.Substring(i);
                    }
                }
            }
            if (sb != null)
            {
                sb.Append(s);
                return sb.ToString();
            }
            else
                return s;
        }

        /// <summary>
        /// Convert a MIME charset name into a valid .Net charset name.
        /// </summary>
        /// <param name="encoding">the MIME charset name</param>
        /// <returns>the .Net charset equivalent. If a suitable mapping is
        /// 	not available, the passed in charset is itself returned.</returns>
        public static string NetEncoding(string encoding)
        {
            if (mime2net.IsNull() || encoding.IsNull())
                // no mapping table, or charset parameter is null
                return encoding;
            string alias = mime2net[encoding.ToLowerInvariant()];
            if (alias.IsNotNull())
            {
                // verify that the mapped name is valid before trying to use it
                try
                {
                    Encoding.GetEncoding(alias);
                }
                catch { alias = null; }
            }
            return alias.IsNull() ? encoding : alias;
        }

        /// <summary>
        /// Convert a .net charset into its MIME charset name.
        /// </summary>
        /// <param name="encoding">the .Net charset</param>
        /// <returns>the MIME/IANA equivalent. If a mapping
        /// 		is not possible, the passed in charset itself
        /// 		is returned.</returns>
        public static string MimeEncoding(string encoding)
        {
            if (net2mime.IsNull() || encoding.IsNull())
                // no mapping table, or charset parameter is null
                return encoding;
            string alias = net2mime[encoding.ToLowerInvariant()];
            return alias.IsNull() ? encoding : alias;
        }

        /// <summary>
        /// Get the default charset corresponding to the system's current 
        /// default locale.  If the System property <code>mail.mime.charset</code>
        /// is set, a system charset corresponding to this MIME charset will be
        /// returned.
        /// </summary>
        /// <returns>the default charset of the system's default locale, as a .Net charset</returns>
        public static string GetDefaultNetEncoding()
        {
            if (defaultNETEncoding.IsNull())
            {
                /*
                 * If mail.mime.charset is set, it controls the default
                 * .Net charset as well.
                 */
                string mimecs = null;
                mimecs = Encoding.Default.EncodingName;
                if (mimecs.IsNotNull() || mimecs.Length > 0)
                {
                    defaultNETEncoding = NetEncoding(mimecs);
                    return defaultNETEncoding;
                }
            }
            return defaultNETEncoding;
        }

        /// <summary>
        /// Get the default MIME charset for this locale.
        /// </summary>
        static string GetDefaultMimeEncoding()
        {
            if (defaultMIMEEncoding.IsNull())
            {
                defaultMIMEEncoding = Encoding.Default.WebName;
            }
            if (defaultMIMEEncoding.IsNull())
                defaultMIMEEncoding = MimeEncoding(GetDefaultNetEncoding());
            return defaultMIMEEncoding;
        }

        private static void loadMappings(StreamReader reader, IDictionary<string, string> table)
        {
            string currentLine;

            while (true)
            {
                try {
                    currentLine = reader.ReadLine();
                }
                catch { break; }
                if (currentLine.IsNull()) break;  // end of file, stop
                if (currentLine.StartsWith("--") && currentLine.EndsWith("--"))
                    // end of this table
                    break;

                // ignore empty lines and comments
                if (currentLine.Trim().Length == 0 || currentLine.StartsWith("#"))
                    continue;

                // A valid entry is of the form <key><separator><value>
                // where, <separator> := SPACE | HT. Parse this
                var tk = currentLine.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    string key = tk[0];
                    string value = tk[1];
                    table.Add(key.ToLowerInvariant(), value);
                }
                catch { }
            }
        }

        static readonly int ALL_ASCII = 1;
        static readonly int MOSTLY_ASCII = 2;
        static readonly int MOSTLY_NONASCII = 3;

        /// <summary>
        /// Check if the given string contains non US-ASCII characters.
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>ALL_ASCII if all characters in the string 
        ///         belong to the US-ASCII charset. MOSTLY_ASCII
        ///         if more than half of the available characters
        ///         are US-ASCII characters. Else MOSTLY_NONASCII.</returns>
        static int checkAscii(string s)
        {
            int ascii = 0, non_ascii = 0;
            int l = s.Length;

            for (int i = 0; i < l; i++)
            {
                if (nonascii((int)s.CharAt(i))) // non-ascii
                    non_ascii++;
                else
                    ascii++;
            }

            if (non_ascii == 0)
                return ALL_ASCII;
            if (ascii > non_ascii)
                return MOSTLY_ASCII;

            return MOSTLY_NONASCII;
        }
        [Obsolete]
        /// <summary>
        /// Check if the given byte array contains non US-ASCII characters.
        /// </summary>
        /// <param name="b">byte array</param>
        /// <returns>ALL_ASCII if all characters in the string 
        ///         belong to the US-ASCII charset. MOSTLY_ASCII
        ///         if more than half of the available characters
        ///         are US-ASCII characters. Else MOSTLY_NONASCII.</returns>
        static int checkAscii(byte[] b)
        {
            int ascii = 0, non_ascii = 0;

            for (int i = 0; i < b.Length; i++)
            {
                // The '&' operator automatically causes b[i] to be promoted
                // to an int, and we mask out the higher bytes in the int 
                // so that the resulting value is not a negative integer.
                if (nonascii(b[i] & 0xff)) // non-ascii
                    non_ascii++;
                else
                    ascii++;
            }

            if (non_ascii == 0)
                return ALL_ASCII;
            if (ascii > non_ascii)
                return MOSTLY_ASCII;

            return MOSTLY_NONASCII;
        }
        /// <summary>
        /// Check if the given input stream contains non US-ASCII characters.
        /// Upto <code>max</code> bytes are checked. If <code>max</code> is
        /// set to <code>ALL</code>, then all the bytes available in this
        /// stream are checked. If <code>breakOnNonAscii</code> is true
        /// the check terminates when the first non-US-ASCII character is
        /// found and MOSTLY_NONASCII is returned. Else, the check continues
        /// till <code>max</code> bytes or till the end of stream.
        /// </summary>
        /// <param name="stream">the strem</param>
        /// <param name="max">maximum bytes to check for. The special value
        /// 		ALL indicates that all the bytes in this input
        /// 		stream must be checked.</param>
        /// <param name="breakOnNonAscii">if <code>true</code>, then terminate the
        ///         the check when the first non-US-ASCII character
        ///         is found.</param>
        /// <returns>ALL_ASCII if all characters in the string 
        ///         belong to the US-ASCII charset. MOSTLY_ASCII
        ///         if more than half of the available characters
        ///         are US-ASCII characters. Else MOSTLY_NONASCII.</returns>
        static int checkAscii(Stream stream, int max, bool breakOnNonAscii)
        {
            int ascii = 0, non_ascii = 0;
            int len;
            int block = 4096;
            int linelen = 0;
            bool longLine = false, badEOL = false;
            bool checkEOL = encodeEolStrict && breakOnNonAscii;
            byte[] buf = null;
            if (max != 0)
            {
                block = (max == ALL) ? 4096 : Math.Min(max, 4096);
                buf = new byte[block];
            }
            while (max != 0)
            {
                try
                {
                    if ((len = stream.Read(buf, 0, block)) == -1) break;
                    int lastb = 0;
                    for (int i = 0; i < len; i++)
                    {
                        // The '&' operator automatically causes b[i] to 
                        // be promoted to an int, and we mask out the higher
                        // bytes in the int so that the resulting value is 
                        // not a negative integer.
                        int b = buf[i] & 0xff;
                        if (checkEOL &&
                            ((lastb == '\r' && b != '\n') ||
                            (lastb != '\r' && b == '\n')))
                            badEOL = true;
                        if (b == '\r' || b == '\n')
                            linelen = 0;
                        else {
                            linelen++;
                            if (linelen > 998)  // 1000 - CRLF
                                longLine = true;
                        }
                        if (nonascii(b))
                        {   // non-ascii
                            if (breakOnNonAscii) // we are done
                                return MOSTLY_NONASCII;
                            else
                                non_ascii++;
                        }
                        else
                            ascii++;
                        lastb = b;
                    }
                }
                catch (IOException) { break; }
                if (max != ALL) max -= len;
            }

            if (max == 0 && breakOnNonAscii)
                // We have been told to break on the first non-ascii character.
                // We haven't got any non-ascii character yet, but then we
                // have not checked all of the available bytes either. So we
                // cannot say for sure that this input stream is ALL_ASCII,
                // and hence we must play safe and return MOSTLY_NONASCII

                return MOSTLY_NONASCII;

            if(non_ascii == 0)
            {
                // no non-us-ascii characters so far
                // If we're looking at non-text data, and we saw CR without LF
                // or vice versa, consider this mostly non-ASCII so that it
                // will be base64 encoded (since the quoted-printable encoder
                // doesn't encode this case properly).
                if (badEOL) return MOSTLY_NONASCII;
                // if we've seen a long line, we degrade to mostly ascii
                else if (longLine) return MOSTLY_ASCII;
                else return ALL_ASCII;
            }
            if (ascii > non_ascii) // mostly ascii
                return MOSTLY_ASCII;
            return MOSTLY_NONASCII;
        }

        static bool nonascii(int b)
        {
            return b >= 0177 || (b < 040 && b != '\r' && b != '\n' && b != '\t');
        }
    }
}
