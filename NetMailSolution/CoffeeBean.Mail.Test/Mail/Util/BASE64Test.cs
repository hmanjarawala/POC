using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using CoffeeBean.Mail.Util;
using System.Linq;
using CoffeeBean.Mail.Test.Mail.Extension;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace CoffeeBean.Mail.Test.Mail.Util
{
    [ExcludeFromCodeCoverage]
    public class BASE64Test
    {
        [Fact]
        public void TestMethod1()
        {
            // test a range of buffer sizes
            for (int bufsize = 1; bufsize < 100; bufsize++)
            {
                //System.out.println("Buffer size: " + bufsize);
                byte[] buf = new byte[bufsize];

                // test a set of patterns

                // first, all zeroes
                buf.Fill((byte)0);
                test("Zeroes", buf);

                // now, all ones
                buf.Fill((byte)0xff);
                test("Ones", buf);

                // now, small integers
                for (int i = 0; i < bufsize; i++)
                    buf[i] = (byte)i;
                test("Ints", buf);

                // finally, random numbers
                Random rnd = new Random();
                rnd.NextBytes(buf);
                test("Random", buf);
            }
        }

        /// <summary>
        /// Encode and decode the buffer and check that we get back the
        /// same data.  Encoding is done both with the static encode
        /// method and using the encoding stream.  Likewise, decoding
        /// is done both with the static decode method and using the
        /// decoding stream.  Check all combinations.
        /// </summary>
        private static void test(string name, byte[] buf)
        {
            // first encode and decode with method
            byte[] encoded = BASE64EncoderStream.Encode(buf);
            byte[] nbuf = BASE64DecoderStream.Decode(encoded);
            compare(name, "method", buf, nbuf);

            // encode with stream, compare with method encoded version
            MemoryStream ms = new MemoryStream();
            BASE64EncoderStream os = new BASE64EncoderStream(ms, int.MaxValue);
            os.Write(buf);
            os.Flush();
            os.Close();
            byte[] sbuf = ms.ToArray();
            compare(name, "encoded", encoded, sbuf);

            // encode with stream, decode with method
            nbuf = BASE64DecoderStream.Decode(sbuf);
            compare(name, "stream->method", buf, nbuf);

            // encode with stream, decode with stream
            MemoryStream ms1 = new MemoryStream(sbuf);
            BASE64DecoderStream os1 = new BASE64DecoderStream(ms1);
            readAll(os1, nbuf, nbuf.Length);
            compare(name, "stream", buf, nbuf);

            // encode with method, decode with stream
            for (int i = 1; i <= nbuf.Length; i++)
            {
                ms1 = new MemoryStream(encoded);
                os1 = new BASE64DecoderStream(ms1);
                readAll(os1, nbuf, i);
                compare(name, "method->stream " + i, buf, nbuf);
            }

            // encode with stream, decode with stream, many buffers

            // first, fill the output with multiple buffers, up to the limit
            int limit = 10000;		// more than 8K
            ms = new MemoryStream();
            os = new BASE64EncoderStream(ms);
            for (int size = 0, blen = buf.Length; size < limit; size += blen)
            {
                if (size + blen > limit)
                {
                    blen = limit - size;
                    // write out partial buffer, starting at non-zero offset
                    os.Write(buf, buf.Length - blen, blen);
                }
                else
                    os.Write(buf);
            }
            os.Flush();
            os.Close();

            // read the encoded output and check the line length
            string type = "big stream";		// for error messages below
            sbuf = ms.ToArray();
            ms1 = new MemoryStream(sbuf);
            byte[] inbuf = new byte[78];
            for (int size = 0, blen = 76; size < limit; size += blen)
            {
                if (size + blen > limit)
                {
                    blen = limit - size;
                    int n = ms1.Read(inbuf, 0, blen);
                    Assert.Equal(blen, n); //, string.Concat(name, ": ", type, " read wrong size at offset ", (size + blen)));
                }
                else {
                    int n = ms1.Read(inbuf, 0, blen + 2);
                    Assert.Equal(blen + 2, n); //string.Concat(name, ": ", type, " read wrong size at offset ", (size + blen)));
                    Assert.True(inbuf[blen] == (byte)'\r' && inbuf[blen + 1] == (byte)'\n', 
                        string.Concat(name, ": ", type, " no CRLF: at offset ", (size + blen)));                    
                }
            }

            // decode the output and check the data
            ms1 = new MemoryStream(sbuf);
            os1 = new BASE64DecoderStream(ms1);
            inbuf = new byte[buf.Length];
            for (int size = 0, blen = buf.Length; size < limit; size += blen)
            {
                if (size + blen > limit)
                    blen = limit - size;
                int n = os1.Read(nbuf, 0, blen);
                Assert.Equal(blen, n); //, string.Concat(name, ": ", type, " read wrong size at offset ", (size + blen)));
                if (blen != buf.Length)
                {
                    // have to compare with end of original buffer
                    byte[] cbuf = new byte[blen];
                    Array.Copy(buf, buf.Length - blen, cbuf, 0, blen);
                    // need a version of the read buffer that's the right size
                    byte[] cnbuf = new byte[blen];
                    Array.Copy(nbuf, 0, cnbuf, 0, blen);
                    compare(name, type, cbuf, cnbuf);
                }
                else {
                    compare(name, type, buf, nbuf);
                }
            }
        }

        private static byte[] origLine;
        private static byte[] encodedLine;

        static BASE64Test()
        {
            try
            {
                origLine = Encoding.ASCII.GetBytes("000000000000000000000000000000000000000000000000000000000");
                encodedLine = Encoding.ASCII.GetBytes(string.Concat("MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAw",
                    "MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAw", Environment.NewLine));
            }
            catch { }
        }

        /// <summary>
        /// Test that CRLF is inserted at the right place.
        /// Test combinations of array writes of different sizes
        /// and single byte writes.
        /// </summary>
        [Fact]
        public void TestLineLength()
        {
            for (int i = 0; i < origLine.Length; i++)
            {
                MemoryStream mos = new MemoryStream();

                OutputStream os = new BASE64EncoderStream(mos);
                os.Write(origLine, 0, i);
                os.Write(origLine, i, origLine.Length - i);
                os.Write((byte)'0');
                os.Flush();
                os.Close();

                byte[] line = new byte[encodedLine.Length];
                Array.Copy(mos.ToArray(), 0, line, 0, line.Length);
                Assert.True(encodedLine.SequenceEqual(line), string.Concat("encoded line ", i));
            }

            for (int i = 0; i < origLine.Length; i++)
            {
                MemoryStream mos = new MemoryStream();

                OutputStream os = new BASE64EncoderStream(mos);
                os.Write(origLine, 0, i);
                os.Write(origLine, i, origLine.Length - i);
                os.Write(origLine);
                os.Flush();
                os.Close();

                byte[] line = new byte[encodedLine.Length];
                Array.Copy(mos.ToArray(), 0, line, 0, line.Length);
                Assert.True(encodedLine.SequenceEqual(line), string.Concat("all arrays, encoded line ", i));
            }

            for (int i = 1; i < 5; i++)
            {
                MemoryStream mos = new MemoryStream();

                OutputStream os = new BASE64EncoderStream(mos);
                for (int j = 0; j < i; j++)
                    os.Write((byte)'0');
                os.Write(origLine, i, origLine.Length - i);
                os.Write((byte)'0');
                os.Flush();
                os.Close();

                byte[] line = new byte[encodedLine.Length];
                Array.Copy(mos.ToArray(), 0, line, 0, line.Length);
                Assert.True(encodedLine.SequenceEqual(line), string.Concat("single byte first encoded line ", i));
            }

            for (int i = origLine.Length - 5; i < origLine.Length; i++)
            {
                MemoryStream mos = new MemoryStream();

                OutputStream os = new BASE64EncoderStream(mos);
                os.Write(origLine, 0, i);
                for (int j = 0; j < origLine.Length - i; j++)
                    os.Write((byte)'0');
                os.Write((byte)'0');
                os.Flush();
                os.Close();

                byte[] line = new byte[encodedLine.Length];
                Array.Copy(mos.ToArray(), 0, line, 0, line.Length);
                Assert.True(encodedLine.SequenceEqual(line), string.Concat("single byte last encoded line ", i));
            }
        }

        /// <summary>
        /// Fill the buffer from the stream.
        /// </summary>
        private static void readAll(InputStream s, byte[] buf, int readsize)
        {
            int need = buf.Length;
            int off = 0;
            int got;
            while (need > 0)
            {
                got = s.Read(buf, off, need > readsize ? readsize : need);
                if (got <= 0) break;
                off += got;
                need -= got;
            }
            if (need != 0) Debug.WriteLine("couldn't read all bytes");
        }

        /// <summary>
        /// Compare the two buffers.
        /// </summary>
        private static void compare(string name, string type, 
            byte[] buf, byte[] nbuf)
        {
            Assert.Equal(buf.Length, nbuf.Length); //, string.Concat(name, ": ", type, " decoded array size wrong"));
            for (int i = 0; i < buf.Length; i++)
                Assert.Equal(buf[i], nbuf[i]); //, string.Concat(name, ": ", type, " data wrong: index ", i));
        }

        /// <summary>
        /// Dump the contents of the buffer.
        /// </summary>
        private static void dump(string name, byte[] buf)
        {
            Debug.WriteLine(name);
            for (int i = 0; i < buf.Length; i++)
                Debug.WriteLine(buf[i]);
        }
    }
}
