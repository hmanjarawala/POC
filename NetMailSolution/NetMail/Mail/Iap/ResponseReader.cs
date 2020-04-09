using CoffeeBean.Mail.Extension;
using CoffeeBean.Mail.Util;
using System;
using System.IO;

namespace CoffeeBean.Mail.Iap
{
    /// <summary>
    /// Reader that is used to read a Response.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class ResponseReader
    {
        private static readonly int minIncrement = 256;
        private static readonly int maxIncrement = 256 * 1024;
        private static readonly int incrementSlop = 16;

        // where we read from
        private BufferedStream bin;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">input stream to wrap</param>
        public ResponseReader(Stream stream)
        {
            bin = new BufferedStream(stream, 2 * 1024);
        }

        /// <summary>
        /// Read a Response from the Stream.
        /// </summary>
        /// <returns>ByteArray that contains the Response</returns>
        /// <exception cref="IOException">for I/O errors</exception>
        public ByteArray ReadResponse()
        {
            return ReadResponse(null);
        }

        /// <summary>
        /// Read a Response from the Stream.
        /// </summary>
        /// <param name="ba">the ByteArray in which to store the response, or null</param>
        /// <returns>ByteArray that contains the Response</returns>
        /// <exception cref="IOException">for I/O errors</exception>
        public ByteArray ReadResponse(ByteArray ba)
        {
            if (ba.IsNull())
                ba = new ByteArray(128);

            byte[] buffer = ba.Bytes;
            int idx = 0;
            for (;;)
            {// read until CRLF with no preceeding literal
             // XXX - b needs to be an int, to handle bytes with value 0xff
                int b = 0;
                bool gotCRLF = false;

                // Read a CRLF terminated line from the InputStream
                while (!gotCRLF &&
                    (b = bin.ReadByte()) != -1)
                {
                    if (b == '\n')
                    {
                        if (idx > 0 && buffer[idx - 1] == '\r')
                            gotCRLF = true;
                    }
                    if (idx >= buffer.Length)
                    {
                        int incr = buffer.Length;
                        if (incr > maxIncrement) incr = maxIncrement;
                        ba.Increase(incr);
                        buffer = ba.Bytes;
                    }
                    buffer[idx++] = (byte)b;
                }

                if (b == -1)
                    throw new IOException("Connection dropped by server?");

                // Now lets check for literals : {<digits>}CRLF
                // Note: index needs to >= 5 for the above sequence to occur
                if (idx < 5 || buffer[idx - 3] != '}')
                    break;

                int i;
                // look for left curly
                for (i = idx - 4; i >= 0; i--)
                    if (buffer[i] == '{')
                        break;

                if (i < 0) // Nope, not a literal ?
                    break;

                int count = 0;
                // OK, handle the literal ..
                try
                {
                    count = ASCIIUtility.ParseInt(buffer, i + 1, idx - 3);
                }
                catch (FormatException)
                {
                    break;
                }

                // Now read 'count' bytes. (Note: count could be 0)
                if (count > 0)
                {
                    int avail = buffer.Length - idx; // available space in buffer
                    if (count + incrementSlop > avail)
                    {
                        // need count-avail more bytes
                        ba.Increase(minIncrement > count + incrementSlop - avail ?
                            minIncrement : count + incrementSlop - avail);
                        buffer = ba.Bytes;
                    }

                    /*
                     * read() might not return all the bytes in one shot,
                     * so call repeatedly till we are done
                     */
                    int actual;
                    while (count > 0)
                    {
                        actual = bin.Read(buffer, idx, count);
                        if (actual == -1)
                            throw new IOException("Connection dropped by server?");
                        count -= actual;
                        idx += actual;
                    }
                }
                // back to top of loop to read until CRLF
            }
            ba.Length = idx;
            return ba;
        }

        /// <summary>
        /// ow much buffered data do we have?
        /// </summary>
        /// <returns>number of bytes available</returns>
        /// <exception cref="IOException">if the stream has been closed</exception>
        public long Available()
        {
            try
            {
                return bin.Length - bin.Position;
            }
            catch (Exception e)
            {
                throw new IOException(e.Message, e);
            }
        }
    }
}
