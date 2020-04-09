using CoffeeBean.Mail.Extension;
using System;
using System.IO;

namespace CoffeeBean.Mail.Util
{
    public class PushbackInputStream : InputStream
    {
        /// <summary>
        /// The buffer that contains pushed-back bytes.
        /// </summary>
        protected byte[] buf;

        /// <summary>
        /// The current position within {@code buf}. A value equal to
        /// <code>buf.Length</code> indicates that no bytes are available. A value of 0
        /// indicates that the buffer is full.
        /// </summary>
        protected int pos;

        /// <summary>
        /// Constructs a new <code>PushbackInputStream</code> with the specified input
        /// stream as source. The size of the pushback buffer is set to the default
        /// value of 1 byte.
        /// </summary>
        /// <param name="s">the source stream</param>
        public PushbackInputStream(Stream s) : base(s)
        {
            buf = s.IsNull() ? null : new byte[1];
            pos = 1;
        }

        /// <summary>
        /// Constructs a new <code>PushbackInputStream</code> with the specified input
        /// stream as source. The size of the pushback buffer is set to <code>size</code>.
        /// </summary>
        /// <param name="s">the source stream</param>
        /// <param name="size">the size of the pushback buffer</param>
        /// <exception cref="ArgumentException">if <code>size</code> is negative</exception>
        public PushbackInputStream(Stream s, int size) : base(s)
        {
            if (size <= 0) throw new ArgumentException(string.Concat("Size: ", size));
            buf = s.IsNull() ? null : new byte[size];
            pos = size;
        }

        public override int Available()
        {
            return (int)(buf.Length - pos + (s.Length - s.Position));
        }

        public override void Close()
        {
            if (s.IsNotNull()) s.Close();
            buf = null;
        }

        public override int Read()
        {
            buf.ThrowIfNull(new IOException());
            // Is there a pushback byte available?
            if (pos < buf.Length) return (buf[pos++] & 0xff);
            // Assume read() in the InputStream will return low-order byte or -1
            // if end of stream.
            return s.ReadByte();
        }

        public override int Read(byte[] b, int off, int len)
        {
            buf.ThrowIfNull(new IOException("Stream is closed"));
            // Force buffer null check first!
            if (off > b.Length || off < 0) throw new IndexOutOfRangeException(string.Concat("Offset out of bounds: ", off));
            if (len < 0 || len > b.Length - off) throw new IndexOutOfRangeException(string.Concat("Length out of bounds: ", len));
            int copiedBytes = 0, copyLength = 0, newOffset = off;
            // Are there pushback bytes available?
            if (pos < buf.Length)
            {
                copyLength = (buf.Length - pos >= len) ? len : buf.Length - pos;
                Array.Copy(buf, pos, b, newOffset, copyLength);
                newOffset += copyLength;
                copiedBytes += copyLength;
                // Use up the bytes in the local buffer
                pos += copyLength;
            }
            // Have we copied enough?
            if (copyLength == len) return len;
            int incopied = s.Read(b, newOffset, len - copiedBytes);
            if (incopied > 0) return incopied + copiedBytes;
            if (copiedBytes == 0) return incopied;
            return copiedBytes;
        }

        public override long Skip(long n)
        {
            s.ThrowIfNull(new IOException("Stream is closed"));
            if (n <= 0) return 0;
            long numSkipped = 0;
            if (pos < buf.Length)
            {
                numSkipped += (n < buf.Length - pos) ? n : buf.Length - pos;
                pos += (int)numSkipped;
            }
            if(numSkipped<n)
            {
                if (s.CanSeek) numSkipped += s.Seek(n - numSkipped, SeekOrigin.Current);
            }
            return numSkipped;
        }

        public virtual void UnRead(byte[] buffer)
        {
            UnRead(buffer, 0, buffer.Length);
        }

        public virtual void UnRead(byte[] buffer, int off, int len)
        {
            if (len > pos) throw new IOException("Pushback buffer is full");
            if(off>buffer.Length||off<0) throw new IndexOutOfRangeException(string.Concat("Offset out of bounds: ", off));
            if (len < 0 || len > b.Length - off) throw new IndexOutOfRangeException(string.Concat("Length out of bounds: ", len));
            buf.ThrowIfNull(new IOException("Stream is closed"));
            Array.Copy(buffer, off, buf, pos - len, len);
            pos = pos - len;
        }

        public virtual void UnRead(int b)
        {
            buf.ThrowIfNull(new IOException());
            if (pos == 0) throw new IOException();
            buf[--pos] = (byte)b;
        }
    }
}
