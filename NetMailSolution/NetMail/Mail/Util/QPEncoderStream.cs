using System.IO;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// This class implements a Quoted Printable Encoder. It is implemented as
    /// a EncoderStream, so one can just wrap this class around
    /// any output stream and write bytes into this filter. The Encoding
    /// is done as the bytes are written out.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class QPEncoderStream : OutputStream
    {
        private int count = 0;  // number of bytes that have been output
        private int bytesPerLine;   // number of bytes per line
        private bool gotSpace = false;
        private bool gotCR = false;

        /// <summary>
        /// Create a QP encoder that encodes the specified input stream
        /// </summary>
        /// <param name="s">the output stream</param>
        /// <param name="bytesPerLine">the number of bytes per line. The encoder
        ///                  inserts a CRLF sequence after this many number
        ///                  of bytes.</param>
        public QPEncoderStream(Stream s, int bytesPerLine) : base(s)
        {
            // Subtract 1 to account for the '=' in the soft-return 
            // at the end of a line
            this.bytesPerLine = bytesPerLine - 1;
        }

        /// <summary>
        /// Create a QP encoder that encodes the specified input stream.
        /// Inserts the CRLF sequence after outputting 76 bytes.
        /// </summary>
        /// <param name="s">the output stream</param>
        public QPEncoderStream(Stream s) : this(s, 76) { }

        /// <summary>
        /// Encodes <code>len</code> bytes from the specified
        /// <code>byte</code> array starting at offset <code>off</code> to
        /// this output stream.
        /// </summary>
        /// <param name="buff">the data</param>
        /// <param name="off">the start offset in the data.</param>
        /// <param name="len">the number of bytes to write.</param>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override void Write(byte[] buff, int off, int len)
        {
            for (int i = 0; i < len; i++)
                Write(buff[off + i]);
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
            b = b & 0xff; // Turn off the MSB.
            if (gotSpace)
            { // previous character was <SPACE>
                if (b == '\r' || b == '\n')
                    // if CR/LF, we need to encode the <SPACE> char
                    output(' ', true);
                else // no encoding required, just output the char
                    output(' ', false);
                gotSpace = false;
            }

            if (b == '\r')
            {
                gotCR = true;
                outputCRLF();
            }
            else {
                if (b == '\n')
                {
                    if (gotCR)
                        // This is a CRLF sequence, we already output the 
                        // corresponding CRLF when we got the CR, so ignore this
                        ;
                    else
                        outputCRLF();
                }
                else if (b == ' ')
                {
                    gotSpace = true;
                }
                else if (b < 040 || b >= 0177 || b == '=')
                    // Encoding required. 
                    output(b, true);
                else // No encoding required
                    output(b, false);
                // whatever it was, it wasn't a CR
                gotCR = false;
            }
        }

        /// <summary>
        /// Flushes this output stream and forces any buffered output bytes
        /// to be encoded out to the stream.
        /// </summary>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override void Flush()
        {
            if (gotSpace)
            {
                output(' ', true);
                gotSpace = false;
            }
	        s.Flush();
        }

        /// <summary>
        /// Forces any buffered output bytes to be encoded out to the stream
        /// and closes this output stream.
        /// </summary>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public override void Close()
        {
            Flush();
            s.Close();
        }

        private void outputCRLF()
        {
            s.WriteByte((byte)'\r');
            s.WriteByte((byte)'\n');
            count = 0;
        }

        // The encoding table
        private readonly static char[] hex = {
            '0','1', '2', '3', '4', '5', '6', '7',
            '8','9', 'A', 'B', 'C', 'D', 'E', 'F'
            };

        protected void output(int c, bool encode)
        {
            if (encode)
            {
                if ((count += 3) > bytesPerLine)
                {
                    s.WriteByte((byte)'=');
                    s.WriteByte((byte)'\r');
                    s.WriteByte((byte)'\n');
                    count = 3; // set the next line's length
                }
                s.WriteByte((byte)'=');
                s.WriteByte((byte)hex[c >> 4]);
                s.WriteByte((byte)hex[c & 0xf]);
            }
            else
            {
                if (++count > bytesPerLine)
                {
                    s.WriteByte((byte)'=');
                    s.WriteByte((byte)'\r');
                    s.WriteByte((byte)'\n');
                    count = 1; // set the next line's length
                }
                s.WriteByte((byte)c);
            }
        }
    }
}
