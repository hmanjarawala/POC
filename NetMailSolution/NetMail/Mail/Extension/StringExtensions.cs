using System;
using System.Text;

namespace CoffeeBean.Mail.Extension
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string @string1, string @string2)
        {
            return @string1.Equals(@string2, StringComparison.OrdinalIgnoreCase);
        }

        public static char CharAt(this string @string, int index)
        {
            if (index < 0 || index > @string.Length) return '\0';
            char[] charArray = @string.ToCharArray();
            return charArray[index];
        }

        public static bool IsNull(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }

        public static bool IsNotNull(this string s)
        {
            return !s.IsNull();
        }

        public static byte[] GetBytes(this string s, string charset)
        {
            Encoding e;
            try
            {
                e = Encoding.GetEncoding(charset);
            }
            catch (ArgumentException) { throw; }
            return e.GetBytes(s);
        }

        public static byte[] GetBytes(this string s)
        {
            return GetBytes(Encoding.Default.WebName);
        }
    }
}
