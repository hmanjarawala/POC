using CoffeeBean.Mail.Extension;
using System;
using System.IO;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// This class implements a QP Decoder. It is implemented as
    /// a DecoderStream, so one can just wrap this class around
    /// any input stream and read bytes from this filter. The decoding
    /// is done as the bytes are read out.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class QPDecoderStream : InputStream
    {
        protected byte[] ba = new byte[2];
        protected int spaces = 0;
        PushbackInputStream reader;

        /// <summary>
        /// Create a Quoted Printable decoder that decodes the specified
        /// input stream.
        /// </summary>
        /// <param name="s">the input stream</param>
        public QPDecoderStream(Stream s) : base(s)
        {
            reader = new PushbackInputStream(s, 2);
        }

        /// <summary>
        /// Read the next decoded byte from this input stream. The byte
        /// is returned as an <code>int</code> in the range <code>0</code>
        /// to <code>255</code>. If no byte is available because the end of
        /// the stream has been reached, the value <code>-1</code> is returned.
        /// This method blocks until input data is available, the end of the
        /// stream is detected, or an exception is thrown.
        /// </summary>
        /// <returns>the next byte of data, or <code>-1</code> if the end of the
        ///          stream is reached.</returns>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override int Read()
        {
            if (spaces > 0)
            {
                // We have cached space characters, return one
                spaces--;
                return ' ';
            }
            
            int b = reader.Read();

            if (b == ' ')
            {
                // Got space, keep reading till we get a non-space char
                while ((b = reader.Read()) == ' ')
                {
                    spaces++;
                }

                if (b == '\r' || b == '\n' || b == -1)
                {
                    // If the non-space char is CR/LF/EOF, the spaces we got
                    // so far is junk introduced during transport. Junk 'em.
                    spaces = 0;
                }
                else
                {
                    reader.UnRead(b);
                    // The non-space char is NOT CR/LF, the spaces are valid.
                    b = ' ';
                }
                return b; // return either <SPACE> or <CR/LF>
            }
            else if (b == '=')
            {
                // QP Encoded atom. Decode the next two bytes
                int a = reader.Read();

                if (a == '\n')
                {
                    /* Hmm ... not really confirming QP encoding, but lets
                     * allow this as a LF terminated encoded line .. and
                     * consider this a soft linebreak and recurse to fetch 
                     * the next char.
                     */
                    return Read();
                }
                else if (a == '\r')
                {
                    // Expecting LF. This forms a soft linebreak to be ignored.
                    int c = reader.Read();
                    if(c != '\n')
                    {
                        /* Not really confirming QP encoding, but
		                 * lets allow this as well.
		                 */
                        reader.UnRead(c);
                    }
                    return Read();
                }
                else if (a == -1)
                {
                    // Not valid QP encoding, but we be nice and tolerant here !
                    return -1;
                }
                else
                {
                    ba[0] = (byte)a;
                    ba[1] = (byte)reader.Read();
                    try
                    {
                        return ASCIIUtility.ParseInt(ba, 0, 2, 16);
                    }
                    catch (FormatException) { reader.UnRead(ba); return b; }
                }
            }
            return b;
        }

        /// <summary>
        /// Reads up to <code>len</code> decoded bytes of data from this input stream
        /// into an array of bytes. This method blocks until some input is
        /// available.
        /// </summary>
        /// <param name="b">the buffer into which the data is read.</param>
        /// <param name="off">the start offset of the data.</param>
        /// <param name="len">the maximum number of bytes read.</param>
        /// <returns>the total number of bytes read into the buffer, or
        ///          <code>-1</code> if there is no more data because the end of
        ///          the stream has been reached.</returns>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override int Read(byte[] b, int off, int len)
        {
            int i, c;
            for (i = 0; i < len; i++)
            {
                if ((c = Read()) == -1)
                {
                    if (i == 0) // At end of stream, so we should
                        i = -1; // return -1 , NOT 0.
                    break;
                }
                b[off + i] = (byte)c;
            }
            return i;
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

        /// <summary>
        /// Returns the number of bytes that can be read from this input
        /// stream without blocking. The QP algorithm does not permit
        /// a priori knowledge of the number of bytes after decoding, so
        /// this method just invokes the <code>available</code> method
        /// of the original input stream.
        /// </summary>
        public override int Available()
        {
            return (int)(s.Length - s.Position);
        }

        public override void Close()
        {
            reader.Close();
            if (s.IsNotNull())
            {
                s.Close();
            }
        }

        protected override void dispose(bool disposable)
        {
            ba = null;
            if (disposable)
            {
                if (reader.IsNotNull()) reader.Dispose();
            }
            reader = null;
        }
    }
}
