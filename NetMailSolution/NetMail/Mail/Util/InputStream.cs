using CoffeeBean.Mail.Extension;
using System;
using System.IO;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// This abstract class is the superclass of all classes representing various format of bytes.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public abstract class InputStream : IDisposable
    {
        // MAX_SKIP_BUFFER_SIZE is used to determine the maximum buffer skip to
        // use when skipping.
        private static readonly int MAX_SKIP_BUFFER_SIZE = 2048;

        protected volatile Stream s;

        public InputStream(Stream s) { this.s = s; }

        /// <summary>
        /// Reads the next byte of data from the input stream. The value byte is returned as an int in the range 0 to 255. If no byte is available 
        /// because the end of the stream has been reached, the value -1 is returned. This method blocks until input data is available, the end of 
        /// the stream is detected, or an exception is thrown. 
        /// A subclass must provide an implementation of this method.
        /// </summary>
        /// <returns>the next byte of data, or -1 if the end of the stream is reached.</returns>
        /// <exception cref="IOException">if an I/O error occurs.</exception>
        public abstract int Read();

        /// <summary>
        /// Reads some number of bytes from the input stream and stores them into the buffer array b. The number of bytes actually read is returned as 
        /// an integer. This method blocks until input data is available, end of file is detected, or an exception is thrown. If the length of b is zero, 
        /// then no bytes are read and 0 is returned; otherwise, there is an attempt to read at least one byte. If no byte is available because the stream 
        /// is at the end of the file, the value -1 is returned; otherwise, at least one byte is read and stored into b.
        /// 
        /// The first byte read is stored into element b[0], the next one into b[1], and so on.The number of bytes read is, at most, equal to the length of b. 
        /// Let k be the number of bytes actually read; these bytes will be stored in elements b[0] through b[k - 1], leaving elements b[k] through b[b.length - 1] 
        /// unaffected.
        /// </summary>
        /// <param name="b">the buffer into which the data is read.</param>
        /// <returns>the total number of bytes read into the buffer, or -1 is there is no more data because the end of the stream has been reached.</returns>
        /// <exception cref="IOException">If the first byte cannot be read for any reason other than the end of the file, if the input stream has been closed, 
        ///     or if some other I/O error occurs.</exception>
        /// <exception cref="ArgumentNullException">If b is null</exception>
        public virtual int Read(byte[] b) { return Read(b, 0, b.Length); }

        /// <summary>
        /// Reads up to len bytes of data from the input stream into an array of bytes. An attempt is made to read as many as len bytes, but a
        /// smaller number may be read. The number of bytes actually read is returned as an integer.
        /// This method blocks until input data is available, end of file is detected, or an exception is thrown.
        /// If len is zero, then no bytes are read and 0 is returned; otherwise, there is an attempt to read at least one byte. If no byte is 
        /// available because the stream is at end of file, the value -1 is returned; otherwise, at least one byte is read and stored into b.
        /// The first byte read is stored into element b[off], the next one into b[off+1], and so on. The number of bytes read is, at most, equal 
        /// to len. Let k be the number of bytes actually read; these bytes will be stored in elements b[off] through b[off+k-1], leaving 
        /// elements b[off+k] through b[off+len-1] unaffected.
        /// In every case, elements b[0] through b[off] and elements b[off+len] through b[b.length-1] are unaffected.
        /// The Read(b, off, len) method for class InputStream simply calls the method read() repeatedly. If the first such call results in an 
        /// IOException, that exception is returned from the call to the read(b, off, len) method. If any subsequent call to Read() results in a 
        /// IOException, the exception is caught and treated as if it were end of file; the bytes read up to that point are stored into b and the 
        /// number of bytes read before the exception occurred is returned. The default implementation of this method blocks until the 
        /// requested amount of input data len has been read, end of file is detected, or an exception is thrown. Subclasses are encouraged to 
        /// provide a more efficient implementation of this method.
        /// </summary>
        /// <param name="b">the buffer into which the data is read.</param>
        /// <param name="off">the start offset in array b at which the data is written.</param>
        /// <param name="len">the maximum number of bytes to read.</param>
        /// <returns>the total number of bytes read into the buffer, or -1 if there is no more data because the end of the stream has been reached.</returns>
        /// <exception cref="IOException">If the first byte cannot be read for any reason other than end of file, or if the input stream has been closed, or if 
        ///     some other I/O error occurs.</exception>
        /// <exception cref="ArgumentNullException">If b is null</exception>
        /// <exception cref="IndexOutOfRangeException">If off is negative, len is negative, or len is greater than b.length - off</exception>
        public virtual int Read(byte[] b, int off, int len)
        {
            b.ThrowIfNull(new ArgumentNullException());
            if (off < 0 || len < 0 || len > b.Length - off) throw new IndexOutOfRangeException();
            else if (len == 0) return 0;

            int c = Read();
            if (c == -1) return -1;
            b[off] = (byte)c;

            int i = 1;
            try
            {
                for (; i < len; i++)
                {
                    c = Read();
                    if (c == -1) break;
                    b[off + i] = (byte)c;
                }
            }
            catch (IOException) { }
            return i;
        }

        /// <summary>
        /// Skips over and discards n bytes of data from this input stream. The skip method may, for a variety of reasons, end up skipping over 
        /// some smaller number of bytes, possibly 0. This may result from any of a number of conditions; reaching end of file before n bytes 
        /// have been skipped is only one possibility. The actual number of bytes skipped is returned. If n is negative, no bytes are skipped.
        /// 
        /// The skip method of this class creates a byte array and then repeatedly reads into it until n bytes have been read or the end of the 
        /// stream has been reached. Subclasses are encouraged to provide a more efficient implementation of this method. For instance, the 
        /// implementation may depend on the ability to seek.
        /// </summary>
        /// <param name="n">the number of bytes to be skipped.</param>
        /// <returns>the actual number of bytes skipped.</returns>
        /// <exception cref="IOException">if the stream does not support seek, or if some other I/O error occurs.</exception>
        public virtual long Skip(long n)
        {
            long remaining = n;
            int nr;

            int size = (int)Math.Min(MAX_SKIP_BUFFER_SIZE, remaining);
            byte[] skippedbuffer = new byte[size];

            while(remaining > 0)
            {
                nr = Read(skippedbuffer, 0, (int)Math.Min(size, remaining));
                if (nr < 0) break;
                remaining -= nr;
            }
            return n - remaining;
        }

        public virtual int Available() { return 0; }

        public abstract void Close();

        protected virtual void dispose(bool disposable)
        {
            if (s.IsNotNull())
            {
                s.Close();
                if (disposable) s.Dispose();
            }
            s = null;
        }

        public void Dispose()
        {
            Close();
            dispose(true);
            GC.SuppressFinalize(this);
        }

        ~InputStream()
        {
            dispose(false);
        }
    }
}
