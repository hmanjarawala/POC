using System;
using System.Globalization;

namespace AAUtility.Extensions
{
    static class StringExtension
    {
        public static DateTime ToDateTime(this string s, string format, string cultureName)
        {
            try
            {
                return DateTime.ParseExact(s, format, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { throw; }
            catch (CultureNotFoundException) { throw; }
        }

        public static int GetWeekOfTheYear(this DateTime date)
        {
            try
            {
                var _gc = new GregorianCalendar();
                var dayOfWeek = _gc.GetDayOfWeek(date);
                if (dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Wednesday) date = date.AddDays(3);
                return _gc.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }
            catch (Exception e) { return 0; }
        }

        public static void ThrowIfNullOrEmpty(this string input, string @param)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(param);
        }
    }
}
