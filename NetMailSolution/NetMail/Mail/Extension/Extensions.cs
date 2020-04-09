using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CoffeeBean.Mail.Extension
{
    public static class Extensions
    {
        public static void ThrowIfNull<T>(this object @object, T exception) where T: Exception, new()
        {
            if (@object == null)
                throw new T();
        }

        public static bool IsNull<T>(this T @object) where T : class
        {
            return @object == null;
        }

        public static bool IsNotNull<T>(this T @object) where T : class
        {
            return @object != null;
        }

        public static bool IsEmpty(this ICollection collection)
        {
            return collection.IsNull() || collection.Count == 0;
        }

        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        public static void Write(this Stream s, byte[] b) { s.Write(b, 0, b.Length); }

        public static int Read(this Stream s, byte[] b) { return s.Read(b, 0, b.Length); }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis(this DateTime d)
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
    }
}
