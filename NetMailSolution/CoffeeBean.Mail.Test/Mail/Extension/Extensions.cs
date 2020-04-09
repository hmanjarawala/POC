using System;

namespace CoffeeBean.Mail.Test.Mail.Extension
{
    public static class Extensions
    {
        public static void Fill<T>(this T[] array, int start, int count, T value)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (start + count > array.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            for (var i = start; i < start + count; i++)
            {
                array[i] = value;
            }
        }

        public static void Fill<T>(this T[] array, T value)
        {
            Fill(array, 0, array.Length, value);
        }
    }
}
