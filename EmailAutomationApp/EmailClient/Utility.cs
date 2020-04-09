using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.IO;

namespace EmailAutomationLibrary
{
    /// <summary>
    /// This class contains 3 static methods to parse headers, message parts, and to convert UTC date/time string to DateTime object.
    /// </summary>
    public static class Utility
    {
        static readonly Regex BoundaryRegex = new Regex("Content-Type: multipart(?:/\\S+;)" +
         "\\s+[^\r\n]*boundary=\"?(?<boundary>" +
         "[^\"\r\n]+)\"?\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static readonly Regex UtcDateTimeRegex = new Regex(
         @"^(?:\w+,\s+)?(?<day>\d+)\s+(?<month>\w+)\s+(?<year>\d+)\s+(?<hour>\d{1,2})" +
         @":(?<minute>\d{1,2}):(?<second>\d{1,2})\s+(?<offsetsign>\-|\+)(?<offsethours>" +
         @"\d{2,2})(?<offsetminutes>\d{2,2})(?:.*)$",
         RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static NameValueCollection ParseHeaders(string headerText)
        {
            NameValueCollection headers = new NameValueCollection();
            StringReader reader = new StringReader(headerText);

            string line;
            string headerName = null;

            var exclude = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "From", "To", "Cc", "Bcc" };

            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    //Himanshu Manjarawala on 2013/02/20 - [Bug# 16034 & 16147] - Start
                    if (line == string.Empty)
                        continue;

                    if (line[0] == ' ' || line[0] == '\t')
                    {
                        if (headerName != null)
                            headers[headerName] += line.Trim();
                        continue;
                    }

                    int colonIndx = line.IndexOf(':');

                    if (colonIndx < 0)
                        continue;

                    headerName = line.Substring(0, colonIndx);

                    //if (exclude.Contains(headerName))
                    //    continue;

                    string headerValue = line.Substring(colonIndx + 1).Trim();

                    headers.Add(headerName, headerValue);
                    //Himanshu Manjarawala on 2013/02/20 - [Bug# 16034 & 16147] - End
                } // end of while loop
            }
            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "ParseHeaders", string.Empty, ex);

            }
            return headers;
        }

        public static List<MIMEPart> ParseMessageParts(string emailText)
        {
            List<MIMEPart> messageParts = new List<MIMEPart>();
            int newLinesIndx = emailText.IndexOf("\r\n\r\n");

            Match m = BoundaryRegex.Match(emailText);

            try
            {
                if (m.Index < emailText.IndexOf("\r\n\r\n") && m.Success)
                {
                    string boundary = m.Groups["boundary"].Value;
                    string startingBoundary = "\r\n--" + boundary;

                    int startingBoundaryIndx = -1;

                    while (true)
                    {
                        if (startingBoundaryIndx == -1)
                            startingBoundaryIndx = emailText.IndexOf(startingBoundary);

                        if (startingBoundaryIndx != -1)
                        {
                            int nextBoundaryIndx = emailText.IndexOf(startingBoundary,
                                                                     startingBoundaryIndx + startingBoundary.Length);

                            if (nextBoundaryIndx != -1 && nextBoundaryIndx != startingBoundaryIndx)
                            {
                                string multipartMsg = emailText.Substring(startingBoundaryIndx +
                                                                          startingBoundary.Length,
                                                                          (nextBoundaryIndx - startingBoundaryIndx -
                                                                           startingBoundary.Length));

                                int headersIndx = multipartMsg.IndexOf("\r\n\r\n");

                                if (headersIndx == -1)
                                {
                                    //   throw new FormatException("Incompatible multipart message format");
                                }
                                //Himanshu Manjarawala on 2013/02/28 - [Bug# 16034 & 16147] - Start
                                string headerText = multipartMsg.Substring(0, headersIndx);
                                //Himanshu Manjarawala on 2013/02/28 - [Bug# 16034 & 16147] - End
                                string bodyText = multipartMsg.Substring(headersIndx).Trim();

                                //Himanshu Manjarawala on 2013/02/28 - [Bug# 16034 & 16147] - Start
                                //NameValueCollection headers = ParseHeaders(multipartMsg.Trim());
                                NameValueCollection headers = ParseHeaders(headerText.Trim());
                                //Himanshu Manjarawala on 2013/02/28 - [Bug# 16034 & 16147] - End
                                messageParts.Add(new MIMEPart(headers, bodyText));
                            }
                            else
                            {
                                break;
                            }
                            startingBoundaryIndx = nextBoundaryIndx;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (newLinesIndx != -1)
                    {
                        string emailBodyText = emailText.Substring(newLinesIndx + 1);

                        //if (messageParts == null)  // this become true when mail is unable to parse.
                        //{
                        //    string emailBodyText = emailText.Substring(newLinesIndx + 1);

                        //}
                    }
                }
                else
                {
                    //Himanshu Manjarawala on 2013/02/20 - [Bug# 16034 & 16147] - Start
                    int startIndex = 0;
                    int headersIndx = 0;
                    //int headersIndx = emailText.IndexOf("\r\n\r\n");

                    NameValueCollection tempHeader = null;
                    do
                    {
                        tempHeader = null;
                        headersIndx = emailText.IndexOf("\r\n\r\n", startIndex);

                        if (headersIndx == -1)
                        {

                            //throw new FormatException("Incompatible multipart message format");
                            break;
                        }
                        startIndex = headersIndx + 1;
                        string headerText = emailText.Substring(0, headersIndx);
                        tempHeader = ParseHeaders(headerText);

                    } while (tempHeader["Content-Type"] == null);
                    //Himanshu Manjarawala on 2013/02/20 - [Bug# 16034 & 16147] - End
                    string bodyText = emailText.Substring(headersIndx).Trim();

                    //Himanshu Manjarawala on 2013/02/20 - [Bug# 16034 & 16147] - Start
                    //NameValueCollection headers = ParseHeaders(emailText);
                    //Himanshu Manjarawala on 2013/02/20 - [Bug# 16034 & 16147] - End
                    messageParts.Add(new MIMEPart(tempHeader, bodyText));
                }
            }

            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "ParseMessageParts", string.Empty, ex);

                messageParts = null;
            }

            return messageParts;
        }

        public static DateTime ConvertStrToDateTime(string str)
        {
            DateTime dt = ConvertStrToUtcDateTime(str);

            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

            dt = dt.AddHours(offset.Hours);
            dt = dt.AddMinutes(offset.Minutes);

            return dt;
        }

        public static DateTime ConvertStrToUtcDateTime(string str)
        {
            Match m = UtcDateTimeRegex.Match(str);

            DateTime dt = DateTime.MinValue;  // default dateTime

            if (m.Success)
            {
                int day = Convert.ToInt32(m.Groups["day"].Value);
                int year = Convert.ToInt32(m.Groups["year"].Value);
                int hour = Convert.ToInt32(m.Groups["hour"].Value);
                int minute = Convert.ToInt32(m.Groups["minute"].Value);
                int second = Convert.ToInt32(m.Groups["second"].Value);

                int month = 0;
                switch (m.Groups["month"].Value)
                {
                    case "Jan":
                        month = 1;
                        break;
                    case "Feb":
                        month = 2;
                        break;
                    case "Mar":
                        month = 3;
                        break;
                    case "Apr":
                        month = 4;
                        break;
                    case "May":
                        month = 5;
                        break;
                    case "Jun":
                        month = 6;
                        break;
                    case "Jul":
                        month = 7;
                        break;
                    case "Aug":
                        month = 8;
                        break;
                    case "Sep":
                        month = 9;
                        break;
                    case "Oct":
                        month = 10;
                        break;
                    case "Nov":
                        month = 11;
                        break;
                    case "Dec":
                        month = 12;
                        break;
                    default:
                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "ConvertStrToUtcDateTime", "Invalid date conversion");
                        break;
                }

                string offsetSign = m.Groups["offsetsign"].Value;
                int offsetHours = Convert.ToInt32(m.Groups["offsethours"].Value);
                int offsetMinutes = Convert.ToInt32(m.Groups["offsetminutes"].Value);

                dt = new DateTime(year, month, day, hour, minute, second);

                if (offsetSign == "+")
                {
                    dt = dt.AddHours(offsetHours * -1);
                    dt = dt.AddMinutes(offsetMinutes * -1);
                }
                else if (offsetSign == "-")
                {
                    dt = dt.AddHours(-offsetHours);
                    dt = dt.AddMinutes(-offsetMinutes);
                }


            }
            else
            {
                //
            }
            // throw new FormatException("Incompatible date/time string format");

            return dt;
        }
    }
}
