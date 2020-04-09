using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAUtility.Extensions
{
    static class DateTimeExtension
    {
        public static DateTime GetNthDayOfWeek(this DateTime currentDate, int occurance, DayOfWeek day)
        {
            var fDay = new DateTime(currentDate.Year, currentDate.Month, 1);
            var fDC = fDay.DayOfWeek == day ? fDay : fDay.AddDays(day - fDay.DayOfWeek);
            if (fDC.Month < currentDate.Month) occurance++;
            return fDC.AddDays(7 * (occurance - 1));
        }
    }
}
