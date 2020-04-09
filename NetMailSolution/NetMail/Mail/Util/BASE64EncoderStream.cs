using CoffeeBean.Mail.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// This class implements a BASE64 encoder.  It is implemented as
    /// a EncoderStream, so one can just wrap this class around
    /// any output stream and write bytes into this filter.  The encoding
    /// is done as the bytes are written out.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class BASE64EncoderStream : OutputStream
    {
        private byte[] buffer;  // cache of bytes that are yet to be encoded
        private int bufsize = 0;    // size of the cache
        private byte[] outbuf;  // line size output buffer
        private int count = 0;  // number of bytes that have been output
        private int bytesPerLine;   // number of bytes per line
        private int lineLimit;	// number of input bytes to output bytesPerLine
        private bool noCRLF = false;

        private static byte[] newLine = new byte[] { (byte)'\r', (byte)'\n' };

        /// <summary>
        /// Create a BASE64 encoder that encodes the specified stream.
        /// </summary>
        /// <param name="s">the output stream</param>
        /// <param name="bytesPerLine">number of bytes per line. The encoder inserts
        /// 		a CRLF sequence after the specified number of bytes,
        /// 		unless bytesPerLine is int.MaxValue, in which
        /// 		case no CRLF is inserted.  bytesPerLine is rounded
        /// 		down to a multiple of 4.</param>
        public BASE64EncoderStream(Stream s, int bytesPerLine) : base(s)
        {
            buffer = new byte[3];
            if (bytesPerLine == int.MaxValue || bytesPerLine < 4)
            {
                noCRLF = true;
                bytesPerLine = 76;
            }
            bytesPerLine = (bytesPerLine / 4) * 4;  // Rounded down to 4n
            this.bytesPerLine = bytesPerLine;   // save it
            lineLimit = bytesPerLine / 4 * 3;

            if (noCRLF)
            {
                outbuf = new byte[bytesPerLine];
            }
            else {
                outbuf = new byte[bytesPerLine + 2];
                outbuf[bytesPerLine] = (byte)'\r';
                outbuf[bytesPerLine + 1] = (byte)'\n';
            }
        }

        /// <summary>
        /// Create a BASE64 encoder that encodes the specified input stream.
        /// Inserts the CRLF sequence after outputting 76 bytes.
        /// </summary>
        /// <param name="s">the output stream</param>
        public BASE64EncoderStream(Stream s) : this(s, 76) { }

        /// <summary>
        /// Encodes <code>len</code> bytes from the specified
        /// <code>byte</code> array starting at offset <code>off</code> to
        /// this output stream.
        /// </summary>
        /// <param name="buff">the data.</param>
        /// <param name="off">the start offset in the data.</param>
        /// <param name="len">the number of bytes to write.</param>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Write(byte[] buff, int off, int len)
        {
            int end = off + len;

            // finish off incomplete coding unit
            while (bufsize != 0 && off < end)
                Write(buff[off++]);

            // finish off line
            int blen = ((bytesPerLine - count) / 4) * 3;
            if (off + blen <= end)
            {
                // number of bytes that will be produced in outbuf
                int outlen = encodedSize(blen);
                if (!noCRLF)
                {
                    outbuf[outlen++] = (byte)'\r';
                    outbuf[outlen++] = (byte)'\n';
                }
	            s.Write(encode(buff, off, blen, outbuf), 0, outlen);
                off += blen;
                count = 0;
            }

            // do bulk encoding a line at a time.
            for (; off + lineLimit <= end; off += lineLimit)
            {
                //int outlen = encodedSize(lineLimit);
                //s.Write(encode(buff, off, lineLimit, outbuf), 0, outlen);
                s.Write(encode(buff, off, lineLimit, outbuf));
            }

            // handle remaining partial line
            if (off + 3 <= end)
            {
                blen = end - off;
                blen = (blen / 3) * 3;  // round down
                                        // number of bytes that will be produced in outbuf
                int outlen = encodedSize(blen);
	            s.Write(encode(buff, off, blen, outbuf), 0, outlen);
                off += blen;
                count += outlen;
            }

            // start next coding unit
            for (; off < end; off++)
                Write(buff[off]);
        }

        /// <summary>
        /// Encodes <code>b.Length</code> bytes to this output stream.
        /// </summary>
        /// <param name="b">the data to be written.</param>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override void Write(byte[] b)
        {
            Write(b, 0, b.Length);
        }

        /// <summary>
        /// Encodes the specified <code>byte</code> to this output stream.
        /// </summary>
        /// <param name="b">the <code>byte</code>.</param>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override void Write(int b)
        {
            buffer[bufsize++] = (byte)b;
            if (bufsize == 3)
            { // Encoding unit = 3 bytes
                encode();
                bufsize = 0;
            }
        }

        /// <summary>
        /// Flushes this output stream and forces any buffered output bytes
        /// to be encoded out to the stream. 
        /// </summary>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Flush()
        {
            if (bufsize > 0)
            { // If there's unencoded characters in the buffer ..
                encode();      // .. encode them
                bufsize = 0;
            }
            s.Flush();
        }

        /// <summary>
        /// Forces any buffered output bytes to be encoded out to the stream
        /// and closes this output stream
        /// </summary>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Close()
        {
            Flush();
            if(count>0 && !noCRLF)
            {
                s.WriteByte(newLine[0]);
                s.WriteByte(newLine[1]);
                s.Flush();
            }
            s.Close();
        }

        /** This array maps the characters to their 6 bit values */
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

        /// <summary>
        /// Encode the data stored in <code>buffer</code>.
        /// Uses <code>outbuf</code> to store the encoded
        /// data before writing.
        /// </summary>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        private void encode()
        {
            int osize = encodedSize(bufsize);
            s.Write(encode(buffer, 0, bufsize, outbuf), 0, osize);
            // increment count
            count += osize;
            // If writing out this encoded unit caused overflow,
            // start a new line.
            if (count >= bytesPerLine)
            {
                if (!noCRLF)
                {
                    s.WriteByte(newLine[0]);
                    s.WriteByte(newLine[1]);
                }
                count = 0;
            }
        }

        /// <summary>
        /// Base64 encode a byte array.  No line breaks are inserted.
        /// This method is suitable for short strings, such as those
        /// in the IMAP AUTHENTICATE protocol, but not to encode the
        /// entire content of a MIME part.
        /// </summary>
        /// <param name="buf">the byte array</param>
        /// <returns>the encoded byte array</returns>
        public static byte[] Encode(byte[] buf)
        {
            return buf.Length == 0? buf : encode(buf, 0, buf.Length, null);
        }

        /// <summary>
        /// Internal use only version of encode.  Allow specifying which
        /// part of the input buffer to encode.  If outbuf is non-null,
        /// it's used as is.  Otherwise, a new output buffer is allocated.
        /// </summary>
        private static byte[] encode(byte[] inbuf, int off, int size,
            byte[] outbuf)
        {
            if (outbuf.IsNull())
                outbuf = new byte[encodedSize(size)];
            int inpos, outpos;
            int val;
            for (inpos = off, outpos = 0; size >= 3; size -= 3, outpos += 4)
            {
                val = inbuf[inpos++] & 0xff;
                val <<= 8;
                val |= inbuf[inpos++] & 0xff;
                val <<= 8;
                val |= inbuf[inpos++] & 0xff;
                outbuf[outpos + 3] = (byte)pem_array[val & 0x3f];
                val >>= 6;
                outbuf[outpos + 2] = (byte)pem_array[val & 0x3f];
                val >>= 6;
                outbuf[outpos + 1] = (byte)pem_array[val & 0x3f];
                val >>= 6;
                outbuf[outpos + 0] = (byte)pem_array[val & 0x3f];
            }
            // done with groups of three, finish up any odd bytes left
            if (size == 1)
            {
                val = inbuf[inpos++] & 0xff;
                val <<= 4;
                outbuf[outpos + 3] = (byte)'='; // pad character;
                outbuf[outpos + 2] = (byte)'='; // pad character;
                outbuf[outpos + 1] = (byte)pem_array[val & 0x3f];
                val >>= 6;
                outbuf[outpos + 0] = (byte)pem_array[val & 0x3f];
            }
            else if (size == 2)
            {
                val = inbuf[inpos++] & 0xff;
                val <<= 8;
                val |= inbuf[inpos++] & 0xff;
                val <<= 2;
                outbuf[outpos + 3] = (byte)'='; // pad character;
                outbuf[outpos + 2] = (byte)pem_array[val & 0x3f];
                val >>= 6;
                outbuf[outpos + 1] = (byte)pem_array[val & 0x3f];
                val >>= 6;
                outbuf[outpos + 0] = (byte)pem_array[val & 0x3f];
            }
            return outbuf;
        }

        /// <summary>
        /// Return the corresponding encoded size for the given number
        /// of bytes, not including any CRLF.
        /// </summary>
        private static int encodedSize(int size)
        {
            return ((size + 2) / 3) * 4;
        }

        protected override void dispose(bool disposable)
        {
            outbuf = null;
            buffer = null;
            if (s.IsNotNull())
            {
                s.Close();
            }
        }

        ~BASE64EncoderStream() { dispose(false); }
    }
}
