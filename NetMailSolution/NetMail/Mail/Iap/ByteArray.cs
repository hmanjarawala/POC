using System;
using System.IO;

namespace CoffeeBean.Mail.Iap
{
    /// <summary>
    /// A simple wrapper around a byte array, with a start position and
    /// count of bytes.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class ByteArray
    {
        private byte[] bytes; // the byte array
        private int start;    // start position
        private int count;    // count of bytes

        /// <summary>
        /// Constructor
        /// 
        /// </summary>
        /// <param name="b">the byte array to wrap</param>
        /// <param name="start">start position in byte array</param>
        /// <param name="count">number of bytes in byte array</param>
        public ByteArray(byte[] b, int start, int count)
        {
            bytes = b;
            this.start = start;
            this.count = count;
        }

        /// <summary>
        /// Constructor that creates a byte array of the specified size.
        /// </summary>
        /// <param name="capacity">the size of the ByteArray</param>
        public ByteArray(int capacity) : this(new byte[capacity], 0, capacity) { }

        /// <summary>
        /// Returns the internal byte array. Note that this is a live
        /// reference to the actual data, not a copy.
        /// </summary>
        public byte[] Bytes { get { return bytes; } }

        /// <summary>
        /// Returns a new byte array that is a copy of the data.
        /// </summary>
        public byte[] CopyBytes
        {
            get
            {
                byte[] b = new byte[count];
                Array.Copy(bytes, start, b, 0, count);
                return b;
            }
        }

        /// <summary>
        /// Returns the start position
        /// </summary>
        public int Start { get { return start; } }

        /// <summary>
        /// Gets or sets the count of bytes
        /// </summary>
        public int Length
        {
            get { return count; }
            set { count = value; }
        }

        /// <summary>
        /// Returns a MemoryStream.
        /// </summary>
        /// <returns>the <see cref="MemoryStream"/></returns>
        public MemoryStream ToStream()
        {
            return new MemoryStream(bytes, start, count, false, false);
        }

        /// <summary>
        /// Increase the byte array by incr bytes.
        /// </summary>
        /// <param name="incr">how much to increase</param>
        public void Increase(int incr)
        {
            byte[] buffer = new byte[bytes.Length + incr];
            Array.Copy(bytes, 0, buffer, 0, bytes.Length);
            bytes = buffer;
        }
    }
}
