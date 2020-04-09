using AAUtility.Extensions;
using System;
using System.Globalization;

namespace AAUtility
{
    partial class UtilityFasad
    {
        public string GetCurrentDateTime(string format)
        { 
            return DateTime.Now.ToString(format); 
        } 

        public string GetCurrentUtcDateTime(string format)
        { 
            return DateTime.UtcNow.ToString(format); 
        } 

        public string GetDateTimeFromDateTime(string strDate, string inFormat, string cultureName, string targetFormat)
        { 
            try 
            { 
                var dt = strDate.ToDateTime(inFormat, cultureName); 
                return dt.ToString(targetFormat); 
            }catch(Exception e) { return e.Message; } 
        } 

        public int GetWeekOfTheYear(string strDate, string format, string cultureName)
        { 
            try 
            { 
                var dt = strDate.ToDateTime(format, cultureName);
                return dt.GetWeekOfTheYear();
            } 
            catch (Exception e) { return 0; } 
        } 

        public int GetWeekOfTheMonth(string strDate, string format, string cultureName)
        {
            try
            {
                var dt = strDate.ToDateTime(format, cultureName);
                int weekOfTheYear = dt.GetWeekOfTheYear();
                var dtFirst = new DateTime(dt.Year, dt.Month, 1);
                int weekOfTheYear1 = dtFirst.GetWeekOfTheYear();
                return weekOfTheYear - weekOfTheYear1 + 1;
            }
            catch { return 0; }
        }

        public string GetElapsedTime(string strFromDate, string strToDate, string format, string cultureName)
        { 
            try 
            { 
                var dtFrom = strFromDate.ToDateTime(format, cultureName); 
                var dtTo = strToDate.ToDateTime(format, cultureName); 
                var span = dtTo - dtFrom; 
                return span.ToString(); 
            }catch(Exception e) { return e.Message; } 
        } 

        public string GetWeekDayTextFromDateTime(string strDate, string format, string cultureName)
        { 
            try 
            { 
                var dt = strDate.ToDateTime(format, cultureName); 
                return dt.DayOfWeek.ToString(); 
            }catch(Exception e) { return e.Message; } 
        } 

        public string GetNextWorkingDay(string strDate, string format, string cultureName)
        { 
            try 
            { 
                var dt = strDate.ToDateTime(format, cultureName); 
                dt = dt.AddDays(1); 
                var dayOfWeek = CultureInfo.GetCultureInfo(cultureName).Calendar.GetDayOfWeek(dt); 
                if (dayOfWeek == DayOfWeek.Saturday) dt = dt.AddDays(2); 
                else if (dayOfWeek == DayOfWeek.Sunday) dt = dt.AddDays(1); 
                return dt.ToString(format); 
            } 
            catch (Exception e) { return e.Message; } 
        } 

        public string GetPriorWorkingDay(string strDate, string format, string cultureName)
        { 
            try 
            { 
                var dt = strDate.ToDateTime(format, cultureName); 
                dt = dt.AddDays(-1); 
                var dayOfWeek = CultureInfo.GetCultureInfo(cultureName).Calendar.GetDayOfWeek(dt); 
                if (dayOfWeek == DayOfWeek.Saturday) dt = dt.AddDays(-1); 
                else if (dayOfWeek == DayOfWeek.Sunday) dt = dt.AddDays(-2); 
                return dt.ToString(format); 
            } 
            catch (Exception e) { return e.Message; } 
        } 

        public string GetLastDayOfMonth(int year, int month, string format)
        {
            try
            {
                var dt = new DateTime(year, month, 1);
                dt = dt.AddMonths(1).AddDays(-1);
                return dt.ToString(format);
            }
            catch (Exception e) { return e.Message; }
        }

        public string GetFutureDate(string strDate, string format, int noOfDaysInFuture, string workingDaysOnly, string cultureName)
        {
            try
            {
                var dt = strDate.ToDateTime(format, cultureName);
                dt = dt.AddDays(noOfDaysInFuture);
                bool blnWorkingDaysOnly = Convert.ToBoolean(workingDaysOnly);
                if (blnWorkingDaysOnly)
                {
                    var dayOfWeek = CultureInfo.GetCultureInfo(cultureName).Calendar.GetDayOfWeek(dt);
                    if (dayOfWeek == DayOfWeek.Saturday) dt = dt.AddDays((noOfDaysInFuture / noOfDaysInFuture));
                    else if (dayOfWeek == DayOfWeek.Sunday) dt = dt.AddDays((noOfDaysInFuture / noOfDaysInFuture) * 2);
                }
                return dt.ToString(format);
            }
            catch (Exception e) { return e.Message; }
        }

        public string GetNthDayOfWeek(string strDate, string format, string cultureName, string targetFormat, int occurance, string dayOfWeek)
        {
            try
            {
                var dt = strDate.ToDateTime(format, cultureName);
                DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayOfWeek);
                return dt.GetNthDayOfWeek(occurance, day).ToString(targetFormat);
            }
            catch (Exception e) { return e.Message; }
        }

    }
}
