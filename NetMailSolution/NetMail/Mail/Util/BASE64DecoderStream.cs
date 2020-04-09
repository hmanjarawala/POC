using CoffeeBean.Mail.Extension;
using System;
using System.IO;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// This class implements a BASE64 Decoder. It is implemented as
    /// a DecoderStream, so one can just wrap this class around
    /// any input stream and read bytes from this filter. The decoding
    /// is done as the bytes are read out.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class BASE64DecoderStream : InputStream
    {
        // buffer of decoded bytes for single byte reads
        private byte[] buffer = new byte[3];
        private int bufsize = 0;    // size of the cache
        private int index = 0;  // index into the cache

        // buffer for almost 8K of typical 76 chars + CRLF lines,
        // used by getByte method.  this buffer contains encoded bytes.
        private byte[] input_buffer = new byte[78 * 105];
        private int input_pos = 0;
        private int input_len = 0;

        private bool ignoreErrors = false;

        /// <summary>
        /// Create a BASE64 decoder that decodes the specified input stream.
        /// The System property<code> mail.mime.base64.ignoreerrors</code>
        /// controls whether errors in the encoded data cause an exception
        /// or are ignored.The default is false (errors cause exception).
        /// </summary>
        /// <param name="s">the input stream</param>
        public BASE64DecoderStream(Stream s) : base(s)
        {
            // default to false
            ignoreErrors = PropertyUtil.GetBooleanSystemPropertyValue(
                "mail.mime.base64.ignoreerrors", false);
        }

        /// <summary>
        /// Create a BASE64 decoder that decodes the specified input stream.
        /// </summary>
        /// <param name="s">the input stream</param>
        /// <param name="ignoreErrors">ignore errors in encoded data?</param>
        public BASE64DecoderStream(Stream s, bool ignoreErrors) : base(s)
        {
            this.ignoreErrors = ignoreErrors;
        }

        /// <summary>
        /// Read the next decoded byte from this input stream. The byte
        /// is returned as an<code> int</code> in the range<code>0</code> 
        /// to<code>255</code>. If no byte is available because the end of
        /// the stream has been reached, the value <code>-1</code> is returned.
        /// This method blocks until input data is available, the end of the 
        /// stream is detected, or an exception is thrown.
        /// </summary>
        /// <returns>next byte of data, or <code>-1</code> if the end of the
        ///            stream is reached.</returns>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override int Read()
        {
            if (index >= bufsize)
            {
                bufsize = decode(buffer, 0, buffer.Length);
                if (bufsize <= 0) // buffer is empty
                    return -1;
                index = 0; // reset index into buffer
            }
            return buffer[index++] & 0xff; // Zero off the MSB
        }

        /// <summary>
        /// Reads up to <code>len</code> decoded bytes of data from this input stream
        /// into an array of bytes. This method blocks until some input is
        /// available.
        /// </summary>
        /// <param name="buf">the buffer into which the data is read.</param>
        /// <param name="off">the start offset of the data.</param>
        /// <param name="len">the maximum number of bytes read.</param>
        /// <returns>the total number of bytes read into the buffer, or
        ///            <code>-1</code> if there is no more data because the end of
        ///            the stream has been reached.</returns>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override int Read(byte[] buf, int off, int len)
        {
            // empty out single byte read buffer
            int off0 = off;
            while (index < bufsize && len > 0)
            {
                buf[off++] = buffer[index++];
                len--;
            }
            if (index >= bufsize)
                bufsize = index = 0;

            int bsize = (len / 3) * 3;  // round down to multiple of 3 bytes
            if (bsize > 0)
            {
                int size = decode(buf, off, bsize);
                off += size;
                len -= size;

                if (size != bsize)
                {   // hit EOF?
                    if (off == off0)    // haven't returned any data
                        return -1;
                    else            // returned some data before hitting EOF
                        return off - off0;
                }
            }

            // finish up with a partial read if necessary
            for (; len > 0; len--)
            {
                int c = Read();
                if (c == -1)    // EOF
                    break;
                buf[off++] = (byte)c;
            }

            if (off == off0)    // haven't returned any data
                return -1;
            else            // returned some data before hitting EOF
                return off - off0;
        }

        /// <summary>
        /// Skips over and discards n bytes of data from this stream.
        /// </summary>
        public override long Skip(long n)
        {
            long skipped = 0;
            while (n-- > 0 && Read() >= 0)
                skipped++;
            return skipped;
        }

        /**
         * This character array provides the character to value map
         * based on RFC1521.
         */
        private readonly static char[] pem_array = {
            'A','B','C','D','E','F','G','H', // 0
	        'I','J','K','L','M','N','O','P', // 1
	        'Q','R','S','T','U','V','W','X', // 2
	        'Y','Z','a','b','c','d','e','f', // 3
	        'g','h','i','j','k','l','m','n', // 4
	        'o','p','q','r','s','t','u','v', // 5
	        'w','x','y','z','0','1','2','3', // 6
	        '4','5','6','7','8','9','+','/'  // 7
            };

        private readonly static sbyte[] pem_convert_array = new sbyte[256];

        static BASE64DecoderStream()
        {
            //unchecked
            //{
                for (int i = 0; i < 255; i++)
                    pem_convert_array[i] = (sbyte)(-1);
            //}            
            for (int i = 0; i < pem_array.Length; i++)
                pem_convert_array[pem_array[i]] = (sbyte)i;
        }

        /// <summary>
        /// The decoder algorithm.  Most of the complexity here is dealing
        /// with error cases.Returns the number of bytes decoded, which
        /// may be zero.Decoding is done by filling an int with 4 6-bit
        /// values by shifting them in from the bottom and then extracting
        /// 3 8-bit bytes from the int by shifting them out from the bottom.
        /// </summary>
        /// <param name="outbuf">the buffer into which to put the decoded bytes</param>
        /// <param name="pos">position in the buffer to start filling</param>
        /// <param name="len">the number of bytes to fill</param>
        /// <returns>the number of bytes filled, always a multiple
        /// 		of three, and may be zero</returns>
        /// <exception cref="IOException">if the data is incorrectly formatted</exception>
        private int decode(byte[] outbuf, int pos, int len)
        {

            int pos0 = pos;
            while (len >= 3)
            {
                /*
                 * We need 4 valid base64 characters before we start decoding.
                 * We skip anything that's not a valid base64 character (usually
                 * just CRLF).
                 */
                int got = 0;
                int val = 0;
                while (got < 4)
                {
                    int i = getByte();
                    if (i == -1 || i == -2)
                    {
                        bool atEOF;
                        if (i == -1)
                        {
                            if (got == 0)
                                return pos - pos0;
                            if (!ignoreErrors)
                                throw new DecodingException(
                                "BASE64Decoder: Error in encoded stream: " +
                                "needed 4 valid base64 characters " +
                                "but only got " + got + " before EOF" +
                                recentChars());
                            atEOF = true;   // don't read any more
                        }
                        else {  // i == -2
                                // found a padding character, we're at EOF
                                // XXX - should do something to make EOF "sticky"
                            if (got < 2 && !ignoreErrors)
                                throw new DecodingException(
                                "BASE64Decoder: Error in encoded stream: " +
                                "needed at least 2 valid base64 characters," +
                                " but only got " + got +
                                " before padding character (=)" +
                                recentChars());

                            // didn't get any characters before padding character?
                            if (got == 0)
                                return pos - pos0;
                            atEOF = false;  // need to keep reading
                        }

                        // pad partial result with zeroes

                        // how many bytes will we produce on output?
                        // (got always < 4, so size always < 3)
                        int size = got - 1;
                        if (size == 0)
                            size = 1;

                        // handle the one padding character we've seen
                        got++;
                        val <<= 6;

                        while (got < 4)
                        {
                            if (!atEOF)
                            {
                                // consume the rest of the padding characters,
                                // filling with zeroes
                                i = getByte();
                                if (i == -1)
                                {
                                    if (!ignoreErrors)
                                        throw new DecodingException(
                                        "BASE64Decoder: Error in encoded " +
                                        "stream: hit EOF while looking for " +
                                        "padding characters (=)" +
                                        recentChars());
                                }
                                else if (i != -2)
                                {
                                    if (!ignoreErrors)
                                        throw new DecodingException(
                                        "BASE64Decoder: Error in encoded " +
                                        "stream: found valid base64 " +
                                        "character after a padding character " +
                                        "(=)" + recentChars());
                                }
                            }
                            val <<= 6;
                            got++;
                        }

                        // now pull out however many valid bytes we got
                        val >>= 8;      // always skip first one
                        if (size == 2)
                            outbuf[pos + 1] = (byte)(val & 0xff);
                        val >>= 8;
                        outbuf[pos] = (byte)(val & 0xff);
                        // len -= size;	// not needed, return below
                        pos += size;
                        return pos - pos0;
                    }
                    else {
                        // got a valid byte
                        val <<= 6;
                        got++;
                        val |= i;
                    }
                }

                // read 4 valid characters, now extract 3 bytes
                outbuf[pos + 2] = (byte)(val & 0xff);
                val >>= 8;
                outbuf[pos + 1] = (byte)(val & 0xff);
                val >>= 8;
                outbuf[pos] = (byte)(val & 0xff);
                len -= 3;
                pos += 3;
            }
            return pos - pos0;
        }

        /// <summary>
        /// Read the next valid byte from the input stream.
        /// Buffer lots of data from underlying stream in input_buffer,
        /// for efficiency.
        /// </summary>
        /// <returns>the next byte, -1 on EOF, or -2 if next byte is '='
        /// 	(padding at end of encoded data)</returns>
        private int getByte()
        {
            int c;
            do
            {
                if (input_pos >= input_len)
                {
                    try
                    {
                        input_len = s.Read(input_buffer);
                    }
                    catch(Exception ex) { return -1; }
                    if (input_len <= 0)
                        return -1;
                    input_pos = 0;
                }
                // get the next byte in the buffer
                c = input_buffer[input_pos++] & 0xff;
                // is it a padding byte?
                if (c == '=')
                    return -2;
                // no, convert it
                c = pem_convert_array[c];
                // loop until we get a legitimate byte
            } while (c == -1);
            return c;
        }

        /// <summary>
        /// Return the most recent characters, for use in an error message.
        /// </summary>
        private string recentChars()
        {
            // reach into the input buffer and extract up to 10
            // recent characters, to help in debugging.
            String errstr = "";
            int nc = input_pos > 10 ? 10 : input_pos;
            if (nc > 0)
            {
                errstr += ", the " + nc +
                        " most recent characters were: \"";
                for (int k = input_pos - nc; k < input_pos; k++)
                {
                    char c = (char)(input_buffer[k] & 0xff);
                    switch (c)
                    {
                        case '\r':
                            errstr += "\\r";
                            break;
                        case '\n':
                            errstr += "\\n";
                            break;
                        case '\t':
                            errstr += "\\t";
                            break;
                        default:
                            if (c >= ' ' && c < 0177)
                                errstr += c;
                            else
                                errstr += ("\\" + (int)c);
                            break;
                    }
                }
                errstr += "\"";
            }
            return errstr;
        }

        /// <summary>
        /// Base64 decode a byte array.  No line breaks are allowed.
        /// This method is suitable for short strings, such as those
        /// in the IMAP AUTHENTICATE protocol, but not to decode the
        /// entire content of a MIME part.
        /// 
        /// NOTE: inbuf may only contain valid base64 characters.
        ///       Whitespace is not ignored.
        /// </summary>
        /// <param name="inbuf">the byte array</param>
        /// <returns>the decode byte array</returns>
        public static byte[] Decode(byte[] inbuf)
        {
            int size = (inbuf.Length / 4) * 3;
            if (size == 0)
                return inbuf;

            if (inbuf[inbuf.Length - 1] == '=')
            {
                size--;
                if (inbuf[inbuf.Length - 2] == '=')
                    size--;
            }
            byte[] outbuf = new byte[size];

            int inpos = 0, outpos = 0;
            size = inbuf.Length;
            while (size > 0)
            {
                int val;
                int osize = 3;
                val = pem_convert_array[inbuf[inpos++] & 0xff];
                val <<= 6;
                val |= pem_convert_array[inbuf[inpos++] & 0xff];
                val <<= 6;
                if (inbuf[inpos] != '=') // End of this BASE64 encoding
                    val |= pem_convert_array[inbuf[inpos++] & 0xff];
                else
                    osize--;
                val <<= 6;
                if (inbuf[inpos] != '=') // End of this BASE64 encoding
                    val |= pem_convert_array[inbuf[inpos++] & 0xff];
                else
                    osize--;
                if (osize > 2)
                    outbuf[outpos + 2] = (byte)(val & 0xff);
                val >>= 8;
                if (osize > 1)
                    outbuf[outpos + 1] = (byte)(val & 0xff);
                val >>= 8;
                outbuf[outpos] = (byte)(val & 0xff);
                outpos += osize;
                size -= 4;
            }
            return outbuf;
        }

        /// <summary>
        /// Returns the number of bytes that can be read from this input
        /// stream without blocking.However, this figure is only
        /// a close approximation in case the original encoded stream
        /// contains embedded CRLFs; since the CRLFs are discarded, not decoded
        /// </summary>
        public override int Available()
        {
            return (int)(((s.Length - s.Position) * 3) / 4 + (bufsize - index));
        }

        public override void Close()
        {
            if (s.IsNotNull())
            {
                s.Close();
            }
            input_buffer = null;
        }

        protected override void dispose(bool disposable)
        {
            buffer = null;
            Close();
        }
    }
}
