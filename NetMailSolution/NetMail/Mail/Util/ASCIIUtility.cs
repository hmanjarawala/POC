using CoffeeBean.Mail.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CoffeeBean.Mail.Util
{
    public class ASCIIUtility
    {
        // Private constructor so that this class is not instantiated
        private ASCIIUtility() { }

        /// <summary>
        /// Convert the bytes within the specified range of the given byte 
        /// array into a signed integer in the given radix . The range extends 
        /// from <code>start</code> till, but not including <code>end</code>.
        /// 
        /// Based on System.Convert.ToInt32()
        /// 
        /// </summary>
        /// <param name="b">the bytes</param>
        /// <param name="start">the first byte offset</param>
        /// <param name="end">the last byte offset</param>
        /// <param name="radix">the radix</param>
        /// <returns>the integer value</returns>
        /// <exception cref="FormatException">for conversion errors</exception>
        public static int ParseInt(byte[] b, int start, int end, int radix)
        {
            if (b.IsNull())
                throw new FormatException("null");
            int result = 0;
            bool negative = false;
            int i = start;
            int limit;
            int multmin;
            int digit;

            if (end > start)
            {
                if (b[i] == '-')
                {
                    negative = true;
                    limit = int.MinValue;
                    i++;
                }
                else
                {
                    limit = -int.MaxValue;
                }
                multmin = limit / radix;
                if (i < end)
                {
                    try
                    {
                        digit = Convert.ToInt32(((char)b[i++]).ToString(), radix);
                    }
                    catch { digit = -1; }
                    if (digit < 0)
                    {
                        throw new FormatException(
                        string.Concat("illegal number: ", ToString(b, start, end))
                        );
                    }
                    else {
                        result = -digit;
                    }
                }
                while (i < end)
                {
                    // Accumulating negatively avoids surprises near MAX_VALUE
                    try
                    {
                        digit = Convert.ToInt32(((char)b[i++]).ToString(), radix);
                    }
                    catch { digit = -1; }
                    if (digit < 0)
                    {
                        throw new FormatException("illegal number");
                    }
                    if (result < multmin)
                    {
                        throw new FormatException("illegal number");
                    }
                    result *= radix;
                    if (result < limit + digit)
                    {
                        throw new FormatException("illegal number");
                    }
                    result -= digit;
                }
            }
            else
            {
                throw new FormatException("illegal number");
            }
            if (negative)
            {
                if (i > start + 1)
                {
                    return result;
                }
                else {  /* Only got "-" */
                    throw new FormatException("illegal number");
                }
            }
            else
            {
                return -result;
            }
        }

        /// <summary>
        /// Convert the bytes within the specified range of the given byte 
        /// array into a signed integer . The range extends from 
        /// <code>start</code> till, but not including <code>end</code>.
        /// 
        /// </summary>
        /// <param name="b">the bytes</param>
        /// <param name="start">the first byte offset</param>
        /// <param name="end">the last byte offset</param>
        /// <returns>the integer value</returns>
        /// <exception cref="FormatException">for conversion errors</exception>
        public static int ParseInt(byte[] b, int start, int end)
        {
            return ParseInt(b, start, end, 10);
        }

        /// <summary>
        /// Convert the bytes within the specified range of the given byte
        /// array into a String. The range extends from <code>start</code>
        /// till, but not including <code>end</code>.
        /// </summary>
        /// <param name="b">the bytes</param>
        /// <param name="start">the first byte offset</param>
        /// <param name="end">the last byte offset</param>
        /// <returns>the String</returns>
        public static string ToString(byte[] b, int start, int end)
        {
            int size = end - start;
            char[] theChars = new char[size];

            for (int i = 0, j = start; i < size;)
                theChars[i++] = (char)(b[j++] & 0xff);

            return new string(theChars);
        }

        /// <summary>
        /// Convert the bytes into a String.
        /// </summary>
        /// <param name="b">the bytes</param>
        /// <returns>the String</returns>
        public static string ToString(byte[] b)
        {
            return ToString(b, 0, b.Length);
        }

        /// <summary>
        /// Convert the bytes within the specified range of the given byte 
        /// array into a signed long in the given radix . The range extends 
        /// from <code>start</code> till, but not including <code>end</code>.
        /// 
        /// Based on System.Convert.ToInt64()
        /// 
        /// </summary>
        /// <param name="b">the bytes</param>
        /// <param name="start">the first byte offset</param>
        /// <param name="end">the last byte offset</param>
        /// <param name="radix">the radix</param>
        /// <returns>the long value</returns>
        /// <exception cref="FormatException">for conversion errors</exception>
        public static long ParseLong(byte[] b, int start, int end, int radix)
        {
            if (b == null)
                throw new FormatException("null");

            long result = 0;
            bool negative = false;
            int i = start;
            long limit;
            long multmin;
            int digit;

            if (end > start)
            {
                if (b[i] == '-')
                {
                    negative = true;
                    limit = long.MinValue;
                    i++;
                }
                else
                {
                    limit = -long.MaxValue;
                }
                multmin = limit / radix;
                if (i < end)
                {
                    try
                    {
                        digit = Convert.ToInt32(((char)b[i++]).ToString(), radix);
                    }
                    catch { digit = -1; }
                    if (digit < 0)
                    {
                        throw new FormatException(
                        string.Concat("illegal number: ", ToString(b, start, end))
                        );
                    }
                    else
                    {
                        result = -digit;
                    }
                }
                while (i < end)
                {
                    // Accumulating negatively avoids surprises near MAX_VALUE
                    try
                    {
                        digit = Convert.ToInt32(((char)b[i++]).ToString(), radix);
                    }
                    catch { digit = -1; }
                    if (digit < 0)
                    {
                        throw new FormatException("illegal number");
                    }
                    if (result < multmin)
                    {
                        throw new FormatException("illegal number");
                    }
                    result *= radix;
                    if (result < limit + digit)
                    {
                        throw new FormatException("illegal number");
                    }
                    result -= digit;
                }
            }
            else
            {
                throw new FormatException("illegal number");
            }
            if (negative)
            {
                if (i > start + 1)
                {
                    return result;
                }
                else
                {  /* Only got "-" */
                    throw new FormatException("illegal number");
                }
            }
            else
            {
                return -result;
            }
        }

        /// <summary>
        /// Convert the bytes within the specified range of the given byte 
        /// array into a signed long . The range extends from 
        /// <code>start</code> till, but not including <code>end</code>.
        /// 
        /// </summary>
        /// <param name="b">the bytes</param>
        /// <param name="start">the first byte offset</param>
        /// <param name="end">the last byte offset</param>
        /// <returns>the long value</returns>
        /// <exception cref="FormatException">for conversion errors</exception>
        public static long ParseLong(byte[] b, int start, int end)
        {
            return ParseLong(b, start, end, 10);
        }

        public static byte[] GetBytes(string s)
        {
            char[] chars = s.ToCharArray();
            int size = chars.Length;
            byte[] bytes = new byte[size];

            for (int i = 0; i < size;)
                bytes[i] = (byte)chars[i++];
            return bytes;
        }
    }
}
