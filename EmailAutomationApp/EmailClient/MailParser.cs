using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace EmailAutomationLibrary
{
    /// <summary>
    /// It represents an Email message on the server. 
    /// It provides you access to the "From", "To", "Subject", "Date" and "Content-Type" headers. 
    /// It does not let you access the body of the email. 
    /// </summary>
    public class MailParser : ITraceable
    {
        public NameValueCollection Headers { get; set; }

        public string ContentType { get; set; }
        public DateTime UtcDateTime { get; set; }
        public string ReceivedDate { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string ReceivedDateTime { get; set; }

        static readonly Regex QuotedPrintableRegex = new Regex(@"(\=([0-9A-F][0-9A-F]))+", RegexOptions.IgnoreCase);

        static readonly Regex CharsetRegex = new Regex("charset=\"?(?<charset>[^\\s\"]+)\"?", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        const string SubjectPattern = @"(?:=\?)([^\?]+)(?:\?[QqBb]\?)([^\?]*)(?:\?=)";

        private bool _isContentFound;

        public event TraceEventHandler LogTrace;

        public static MIMEPart FindMessagePart(List<MIMEPart> msgParts, string contentType)
        {

            //Trace(new TraceEventArg(() => "EmailParser: FindMessagePart > Entry"));
            string info = "@contentType: " + contentType;
            //Trace(new TraceEventArg(() => $"EmailParser: FindMessagePart > {info}"));

            return msgParts.FirstOrDefault(p => p.ContentType != null && p.ContentType.ToLower().IndexOf(contentType) != -1);
        }

        public MailParser(string emailText)
        {

            Trace(new TraceEventArg(() => "MailParser: MailParser > Entry"));
            string info = "@emailText: " + emailText;
            Trace(new TraceEventArg(() => $"MailParser: MailParser > {info}"));

            try
            {
                Headers = Utility.ParseHeaders(emailText);

                ContentType = Headers["Content-Type"];
                From = Headers["From"];
                To = Headers["To"];
                Cc = (string.IsNullOrEmpty(Headers["CC"])) ? string.Empty : Headers["CC"];
                Bcc = (string.IsNullOrEmpty(Headers["BCC"])) ? string.Empty : Headers["BCC"];
                Subject = Headers["Subject"];

                if (Headers["Date"] != null)
                    try
                    {
                        ReceivedDateTime = Headers["Date"]; //Bug:17820 - Ankita - 11/11/2013
                        UtcDateTime = Utility.ConvertStrToUtcDateTime(Headers["Date"]);
                        ReceivedDate = Headers["Date"]; //Bug:17820 - Ankita - 11/11/2013
                    }
                    catch (FormatException ex)
                    {

                        Trace(new TraceEventArg(() => $"MailParser > Invalid Date {ex}"));
                        UtcDateTime = DateTime.MinValue;
                    }
                else
                    UtcDateTime = DateTime.MinValue;
            }
            catch (Exception ex)
            {
                Trace(new TraceEventArg(() => $"MailParser > {ex}"));

                Headers = null;
            }

            Trace(new TraceEventArg(() => "MailParser: MailParser > Exit"));

        }

        private static int GetDivider(string frontByte)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: GetDivider", "Entry");
            string info = "@frontByte: " + frontByte;
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: GetDivider", info);

            int firstByte = Convert.ToInt32(frontByte, 16);

            int hex00 = Convert.ToInt32("00", 16);
            int hex7F = Convert.ToInt32("7F", 16);
            int hexC0 = Convert.ToInt32("C0", 16);
            int hexDf = Convert.ToInt32("DF", 16);
            int hexE0 = Convert.ToInt32("E0", 16);
            int hexEf = Convert.ToInt32("EF", 16);
            int hexF0 = Convert.ToInt32("F0", 16);
            int hexF7 = Convert.ToInt32("F7", 16);


            int divider = 1;

            if ((firstByte >= hex00) && (firstByte <= hex7F))
            {
                divider = 1;
            }
            else if ((firstByte >= hexC0) && (firstByte <= hexDf))
            {
                divider = 2;
            }
            else if ((firstByte >= hexE0) && (firstByte <= hexEf))
            {
                divider = 3;
            }
            else if ((firstByte >= hexF0) && (firstByte <= hexF7))
            {
                divider = 4;
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: GetDivider", "Exit");

            return divider;
        }

        /// <summary>

        /// =?us-ascii?Q?How_to_reset_your_Yahoo!_password?=
        /// encoded-word = "=?" charset "?" encoding "?" encoded-text "?="
        /// =?iso-8859-1?Q?Verification E-Mail?=
        /// =?windows-1252?B?TGliZXJ0eSBVcGRhdGU6IFRoZSBUZWEgUGFydHkncyBSb2xlIGluIA==?= =?windows-1252?B?dGhlIDIwMTIgRWxlY3Rpb25z?=
        /// ?windows-1252?Q?ankitdsi@hotmail.com=20wants=20to=20add=20you?=
        /// =?UTF-8?B?4KSa4KWA4K4KWC4KSw4KWN4KS1?=
        /// /// =?UTF-8?Q?Partner_News:_A_Bigger_Window_Th?=
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public static string DecodeSubject(string subject)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeSubject", "Entry");
            string info = "@subject: " + subject;
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeSubject", info);

            StringBuilder sb;

            try
            {
                MatchCollection utfBase64 = Regex.Matches(subject, SubjectPattern, RegexOptions.Multiline | RegexOptions.Compiled);

                if (utfBase64.Count != 0)
                {
                    string charset = utfBase64[0].Groups[1].Value;
                    sb = new StringBuilder();

                    string decodeString;

                    if (subject.Contains("?B?"))
                    {
                        for (int i = 0; i < utfBase64.Count; i++)
                        {
                            decodeString = DecodeBase64String(charset, utfBase64[i].Groups[2].Value);
                            sb.Append(decodeString);
                        }

                        subject = sb.ToString();
                    }
                    else if (subject.Contains("?Q?"))
                    {
                        for (int i = 0; i < utfBase64.Count; i++)
                        {
                            decodeString = DecodeQuotedPrintableSubject(charset, utfBase64[i].Groups[2].Value);
                            sb.Append(decodeString);
                        }
                        subject = sb.ToString();
                    }

                }
            }


            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeSubject", string.Empty, ex);


            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeSubject", "Exit");

            return subject;
        }

        public static string ProcessMailBody(List<MIMEPart> message, string mailTypeText, string attchmentPath)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Entry");
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "The new library");
            string info = string.Empty;
            if (mailTypeText != null)
                info = "@mailTypeText: " + mailTypeText;
            if (attchmentPath != null)
                info = info + " @attchmentPath: " + attchmentPath;
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", info);

            string[] contentTypeCharset;
            string contentType;
            string charset;
            string decodeString = string.Empty;
            bool isContentFound = false;

            if (message.Count >= 2)   // either contain attachment or html/text (both) field
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Mail count is > 2");
                for (int i = 0; i < message.Count; i++)
                {
                    contentTypeCharset = message[i].ContentType.Split(';');
                    contentType = contentTypeCharset[0];

                    if (string.IsNullOrEmpty(message[i].ContentType) == false)
                    {
                        if (contentType.ToLower().Contains("multipart") && (isContentFound == false || string.IsNullOrEmpty(decodeString)))
                        {
                            bool isMailContentFound;
                            decodeString = ReadMultipartMessage(message[i].MessageText, mailTypeText,
                                                                out isMailContentFound);
                            isContentFound = isMailContentFound;
                        }
                        else if (mailTypeText.ToLower().Equals(contentType) && (isContentFound == false || string.IsNullOrEmpty(decodeString)))
                        {
                            charset = (contentTypeCharset.Length == 1) ? "7bit" : contentTypeCharset[1].Split('=')[1];
                            charset = charset.Replace("\"", String.Empty);

                            string contentEncoding = message[i].Headers["Content-Transfer-Encoding"];
                            decodeString = DecodeContent(contentEncoding, message[i].MessageText, charset);
                            isContentFound = true;
                        }
                        else if (contentType.ToLower().Equals("text/html") && (isContentFound == false || string.IsNullOrEmpty(decodeString)))
                        {
                            charset = (contentTypeCharset.Length == 1) ? "7bit" : contentTypeCharset[1].Split('=')[1];
                            charset = charset.Replace("\"", String.Empty);

                            string contentEncoding = message[i].Headers["Content-Transfer-Encoding"];
                            decodeString = DecodeContent(contentEncoding, message[i].MessageText, charset);
                            isContentFound = true;
                        }

                        else if (i >= 1) // scan for attachment
                        {
                            string fileExtension = string.Empty;

                            if (message[i].ContentType.IndexOf("name=") > 0)
                            {
                                if (string.IsNullOrEmpty(attchmentPath) == false) // if attachment path is not specify
                                {
                                    //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "1");

                                    string fileName = message[i].ContentType.Substring(message[i].ContentType.IndexOf("name=") + 5);

                                    fileName = extractAttachmentName(fileName, "\"");

                                    string messageBase64String = message[i].MessageText.Trim();

                                    string contentEncoding = message[i].Headers["Content-Transfer-Encoding"];

                                    if (contentEncoding == null) // used in case of forward mail
                                    {
                                        if (message[i].Headers["Content-Disposition"].Contains("filename="))
                                        {
                                            string[] fullfileName = message[i].Headers["Content-Disposition"].Split(';');
                                            fileName = fullfileName[1].Substring(fullfileName[i].IndexOf("="));
                                            fileName = fileName.Replace("\"", string.Empty);

                                        }
                                    }
                                    SaveFile(attchmentPath + "\\" + fileName, messageBase64String, contentEncoding);

                                }
                            }
                            else if (message[i].ContentType.Contains("multipart"))
                            {
                                ProcessAttachementFromOwnSend(message[i].MessageText, attchmentPath);

                            }
                            else if (!string.IsNullOrEmpty(message[i].Headers["Content-Disposition"]))
                            {
                                if ((message[i].Headers["Content-Disposition"].Contains("filename=")))
                                {
                                    //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "2");

                                    string[] fullfileName = message[i].Headers["Content-Disposition"].Split(';');

                                    string fileName = fullfileName[1].Substring(fullfileName[1].IndexOf("filename=") + 9);

                                    fileName = extractAttachmentName(fileName, "\"");

                                    string contentEncoding = message[i].Headers["Content-Transfer-Encoding"];
                                    string fileContent = message[i].MessageText.Trim();
                                    if (fileContent.Contains("FLAGS (\\Seen)"))
                                    {
                                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Removing FLAGS (SEEN)");
                                        fileContent = fileContent.Replace("FLAGS (\\Seen)", string.Empty);
                                    }
                                    SaveFile(attchmentPath + "\\" + fileName, fileContent, contentEncoding);
                                }
                                //Note that whenever email is attached, it's type is rfc822 and format is .eml
                                else if ((message[i].Headers["Content-Disposition"].ToLower().Contains("attachment"))
                                    && (message[i].ContentType.ToLower().Contains("rfc822")))
                                {
                                    //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "3");

                                    string contentEncoding = message[i].Headers["Content-Transfer-Encoding"];
                                    string fileContent = message[i].MessageText.Trim();

                                    string fileNameString = fileContent.Substring(fileContent.IndexOf("Subject:") + 8);
                                    string[] subNameString = fileNameString.Split('\r');
                                    string fileName = subNameString[0].Trim();//the email attachment fileName is the subject line

                                    //if the fileName contains a semi-colon (:), it is not supported (refer to bug - 19319)
                                    fileName = extractAttachmentName(fileName, ":");

                                    string fullEmlFileName = attchmentPath + fileName + ".eml";

                                    SaveFile(fullEmlFileName, fileContent, contentEncoding);
                                }
                            }
                        } // end of attachment else

                    }
                    else
                    {
                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "ProcessMailBody", "Unable to process message text");

                    }
                } // end of html or attchemnt part
            }
            else
            {
                MIMEPart preferredMsgPart = FindMessagePart(message, mailTypeText);

                if (preferredMsgPart == null)
                {
                    if (mailTypeText == "text/html")
                    {
                        preferredMsgPart = FindMessagePart(message, "text/plain");

                        if (preferredMsgPart == null) // only used when multipart/alternative
                        {

                            preferredMsgPart = FindMessagePart(message, "text/xml");

                            if (preferredMsgPart == null)
                            {
                                contentTypeCharset = message[0].ContentType.Split(';');
                                contentType = contentTypeCharset[0];
                                if (contentType.Contains("multipart"))
                                {
                                    bool isMailContentFound;
                                    decodeString = ReadMultipartMessage(message[0].MessageText, mailTypeText, out isMailContentFound);

                                    ProcessAttachementFromOwnSend(message[0].MessageText, attchmentPath);
                                    isContentFound = isMailContentFound;
                                }

                                else if (!string.IsNullOrEmpty(message[0].Headers["Content-Disposition"]))
                                {
                                    if ((message[0].Headers["Content-Disposition"].Contains("filename="))) // from mail.india.com (server) to mail.india.com
                                    {
                                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "4");

                                        string[] fullfileName = message[0].Headers["Content-Disposition"].Split(';');

                                        string fileName = fullfileName[1].Substring(fullfileName[1].IndexOf("filename=") + 9);

                                        fileName = extractAttachmentName(fileName, "\"");

                                        string contentEncoding = message[0].Headers["Content-Transfer-Encoding"];
                                        string fileContent = message[0].MessageText.Trim();
                                        if (fileContent.Contains("FLAGS (\\Seen)"))
                                        {
                                            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Removing FLAGS (SEEN)");
                                            fileContent = fileContent.Replace("FLAGS (\\Seen)", string.Empty);
                                        }
                                        SaveFile(attchmentPath + "\\" + fileName, fileContent, contentEncoding);
                                    }
                                }
                                else if (string.IsNullOrEmpty(message[0].Headers["Content-Disposition"]) && contentType.Contains("application")
                                && contentTypeCharset[1].Contains("name="))
                                {
                                    //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "5");

                                    string[] fullfileName = message[0].Headers["Content-Type"].Split(';');

                                    string fileName = fullfileName[1].Substring(fullfileName[1].IndexOf("name=") + 5);

                                    fileName = extractAttachmentName(fileName, "\"");

                                    string contentEncoding = message[0].Headers["Content-Transfer-Encoding"];
                                    string fileContent = message[0].MessageText.Trim();
                                    if (fileContent.Contains("FLAGS (\\Seen)"))
                                    {
                                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Removing FLAGS (SEEN)");
                                        fileContent = fileContent.Replace("FLAGS (\\Seen)", string.Empty);
                                    }
                                    SaveFile(attchmentPath + "\\" + fileName, fileContent, contentEncoding);
                                }

                            }
                            else
                            {
                                decodeString = DecodeMessage(preferredMsgPart);
                                isContentFound = true;
                            }

                        }
                        else
                        {
                            decodeString = DecodeMessage(preferredMsgPart);
                            isContentFound = true;
                        }
                    }
                    else if (mailTypeText == "text/plain")
                    {
                        preferredMsgPart = FindMessagePart(message, "text/html");

                        if (preferredMsgPart == null) // only used when multipart/alternative
                        {
                            preferredMsgPart = FindMessagePart(message, "text/xml");

                            if (preferredMsgPart == null)
                            {
                                contentTypeCharset = message[0].ContentType.Split(';');
                                contentType = contentTypeCharset[0];
                                if (contentType.Contains("multipart"))
                                {
                                    bool isMailContentFound;
                                    decodeString = ReadMultipartMessage(message[0].MessageText, mailTypeText, out isMailContentFound);
                                    ProcessAttachementFromOwnSend(message[0].MessageText, attchmentPath);
                                    isContentFound = isMailContentFound;
                                }
                                else if (!string.IsNullOrEmpty(message[0].Headers["Content-Disposition"]))
                                {
                                    if ((message[0].Headers["Content-Disposition"].Contains("filename="))) // from mail.india.com (server) to mail.india.com
                                    {
                                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "6");

                                        string[] fullfileName = message[0].Headers["Content-Disposition"].Split(';');

                                        string fileName = fullfileName[1].Substring(fullfileName[1].IndexOf("filename=") + 9);

                                        fileName = extractAttachmentName(fileName, "\"");

                                        string contentEncoding = message[0].Headers["Content-Transfer-Encoding"];
                                        string fileContent = message[0].MessageText.Trim();

                                        if (fileContent.Contains("FLAGS (\\Seen)"))
                                        {
                                            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Removing FLAGS (SEEN)");
                                            fileContent = fileContent.Replace("FLAGS (\\Seen)", string.Empty);
                                        }

                                        SaveFile(attchmentPath + "\\" + fileName, fileContent, contentEncoding);
                                    }
                                }
                                else if (string.IsNullOrEmpty(message[0].Headers["Content-Disposition"]) && contentType.Contains("application")
                                && contentTypeCharset[1].Contains("name="))
                                {
                                    //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "7");

                                    string[] fullfileName = message[0].Headers["Content-Type"].Split(';');

                                    string fileName = fullfileName[1].Substring(fullfileName[1].IndexOf("name=") + 5);

                                    fileName = extractAttachmentName(fileName, "\"");

                                    string contentEncoding = message[0].Headers["Content-Transfer-Encoding"];
                                    string fileContent = message[0].MessageText.Trim();

                                    if (fileContent.Contains("FLAGS (\\Seen)"))
                                    {
                                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Removing FLAGS (SEEN)");
                                        fileContent = fileContent.Replace("FLAGS (\\Seen)", string.Empty);
                                    }
                                    SaveFile(attchmentPath + "\\" + fileName, fileContent, contentEncoding);
                                }
                            }
                            else
                            {
                                decodeString = DecodeMessage(preferredMsgPart);
                                isContentFound = true;
                            }
                        }
                        else
                        {
                            decodeString = DecodeMessage(preferredMsgPart);
                            isContentFound = true;
                        }
                    }
                }
                else
                {
                    decodeString = DecodeMessage(preferredMsgPart);
                    isContentFound = true;

                }
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessMailBody", "Exit");

            return decodeString;
        }

        private static void ProcessAttachementFromOwnSend(string messagePart, string attchmentPath)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessAttachementFromOwnSend", "Entry");
            string info = "@messagePart: " + messagePart + " @attchmentPath: " + attchmentPath.ToString();
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessAttachementFromOwnSend", info);

            var reader = new StringReader(messagePart);

            try
            {
                string[] allLines;

                while (true) // end of while loop
                {
                    string readLine = reader.ReadLine();

                    if (string.IsNullOrEmpty(readLine) == false && readLine.StartsWith("--"))
                    {
                        allLines = Regex.Split(messagePart, readLine, RegexOptions.Multiline | RegexOptions.Compiled);

                        readLine = allLines[allLines.Length - 1].Replace("\r\n", string.Empty);

                        if (readLine.EndsWith("--") || readLine.Trim().StartsWith("FLAGS (\\"))//Bug# 17326 - Ankita - 20/11/2013
                        {
                            break;
                        }
                    }
                }

                for (int i = 1; i < allLines.Length - 1; i++)
                {
                    if (allLines[i].IndexOf("\r\n") >= 0)
                    {
                        allLines[i] = allLines[i].Remove(0, 2);
                    }

                    List<MIMEPart> message = Utility.ParseMessageParts(allLines[i]);

                    foreach (MIMEPart msgPart in message)
                    {
                        if (msgPart.ContentType.IndexOf("name=") > 0)
                        {
                            string[] contentTypeCharset = msgPart.ContentType.Split(';');

                            string fileName = isUtfOrIsoEncodedFileName(contentTypeCharset[1]) ? contentTypeCharset[1] : contentTypeCharset[1].Split(new char[] { '=' })[1];

                            fileName = extractAttachmentName(fileName, "\"");

                            string contentEncoding = msgPart.Headers["Content-Transfer-Encoding"];

                            SaveFile(attchmentPath + "\\" + fileName, msgPart.MessageText, contentEncoding);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "Process Attachment From SendMail", ex.Message, ex);
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ProcessAttachementFromOwnSend", "Exit");

        }

        public static string DecodeContent(string contentEncoding, string message, string charset)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeContent", "Entry");
            //string info = "@contentEncoding: " + contentEncoding + " @message: " + message.ToString() + " @charset: " + charset.ToString();
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeContent", info);

            string decodedText = string.Empty;

            if (string.IsNullOrEmpty(contentEncoding) == false)
            {
                switch (contentEncoding.ToLower())
                {
                    case "base64":
                        decodedText = DecodeBase64String(charset, message);
                        break;
                    case "quoted-printable":
                        decodedText = QuotedPrintable.DecodeEncodedMessage(message, charset);
                        break;
                    case "8bit":
                    case "7bit":
                    case "binary":
                        decodedText = message;
                        break;
                }
            }
            else
            {
                decodedText = message;
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeContent", "Exit");

            return decodedText;
        }

        private static string ReadMultipartMessage(string messagePart, string mailTypeText, out bool isContentFound)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ReadMultipartMessage", "Entry");
            //string info = "@messagePart: " + messagePart + " @mailTypeText: " + mailTypeText.ToString();
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ReadMultipartMessage", info);

            StringReader reader = new StringReader(messagePart);

            string decodeString = string.Empty;

            isContentFound = false;

            string[] allLines;

            string readLine;

            while (true) // end of while loop
            {
                readLine = reader.ReadLine();

                if (string.IsNullOrEmpty(readLine) == false && readLine.StartsWith("--"))
                {
                    allLines = Regex.Split(messagePart, readLine, RegexOptions.Multiline | RegexOptions.Compiled);

                    readLine = allLines[allLines.Length - 1].Replace("\r\n", string.Empty);

                    if (readLine.EndsWith("--") || readLine.Trim().StartsWith("FLAGS (\\"))//Bug# 17326 - Ankita - 20/11/2013
                    {
                        break;
                    }
                }

            }

            for (int i = 1; i < allLines.Length - 1; i++)
            {
                if (isContentFound == false)
                {
                    if (allLines[i].IndexOf("\r\n") >= 0)
                    {
                        allLines[i] = allLines[i].Remove(0, 2);
                    }

                    List<MIMEPart> message = Utility.ParseMessageParts(allLines[i]);

                    foreach (MIMEPart t in message)
                    {
                        string[] contentTypeCharset = t.ContentType.Split(';');
                        string contentType = contentTypeCharset[0];

                        if (contentType.Contains("multipart"))
                        {
                            bool isMailContentFound;
                            decodeString = ReadMultipartMessage(message[0].MessageText, mailTypeText, out isMailContentFound);
                            isContentFound = isMailContentFound;
                        }
                        else
                        {
                            string charset = (contentTypeCharset.Length == 1)
                                                 ? "7bit"
                                                 : contentTypeCharset[1].Split('=')[1];

                            charset = charset.Replace("\"", String.Empty);

                            string contentEncoding = t.Headers["Content-Transfer-Encoding"];

                            if (mailTypeText.Equals(contentType))
                            {
                                decodeString = DecodeContent(contentEncoding, t.MessageText, charset);
                                isContentFound = true;
                                break;
                            }
                        }
                    }

                }
            }

            if (string.IsNullOrEmpty(decodeString) && mailTypeText.Contains("text/plain"))
            {
                return ReadMultipartMessage(messagePart, "text/html", out isContentFound);
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: ReadMultipartMessage", "Exit");

            return decodeString;
        }

        private static void SaveFile(string savePath, string messageBase64String, string contentEncoding)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: SaveFile", "Entry");

            string info = string.Empty;

            if (savePath != null)
                info = "@savePath: " + savePath; //note that if the savePath is empty, we want that info as well;
            if (messageBase64String != null)
                info = info + " @messageBase64String: " + messageBase64String;
            if (contentEncoding != null)
                info = info + " @contentEncoding: " + contentEncoding;
            //string info = "@savePath: " + savePath + " @messageBase64String: " + messageBase64String.ToString() + " @contentEncoding: " + contentEncoding.ToString();
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: SaveFile", info);

            try
            {
                if (string.IsNullOrEmpty(contentEncoding) == false)
                {
                    contentEncoding = contentEncoding.ToLower().Trim();

                    if (contentEncoding.Equals("base64"))
                    {
                        byte[] encodedDataAsBytes = Convert.FromBase64String(messageBase64String);

                        using (var stream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                        {
                            stream.Write(encodedDataAsBytes, 0, encodedDataAsBytes.Length);
                        }
                    }
                    else if (contentEncoding.Equals("quoted-printable"))
                    {
                        byte[] encodeArray = new UTF8Encoding().GetBytes(messageBase64String);
                        File.WriteAllBytes(savePath, encodeArray);
                    }
                    else if (contentEncoding.Equals("7bit"))
                    {
                        byte[] encodeArray = new ASCIIEncoding().GetBytes(messageBase64String);
                        File.WriteAllBytes(savePath, encodeArray);

                    }
                    else
                    {
                        //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "UnSupported encoding = ", contentEncoding);
                    }
                }
                else
                {
                    byte[] encodeArray = new ASCIIEncoding().GetBytes(messageBase64String);
                    if (encodeArray != null)
                        File.WriteAllBytes(savePath, encodeArray);
                    //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "Null encoding", savePath);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "From SaveFile", ex.Message, ex);

            }
            catch (IOException ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "From SaveFile", ex.Message, ex);

            }

            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "From SaveFile", ex.Message, ex);

            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: SaveFile", "Exit");
        }

        private static string DecodeMessage(MIMEPart filterMessage)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeMessage", "Entry");

            string content = string.Empty;

            if (filterMessage != null)
            {
                string contentType = filterMessage.Headers["Content-Type"];
                string charset = "us-ascii"; // default charset
                string contentTransferEncoding = filterMessage.Headers["Content-Transfer-Encoding"];

                Match m = CharsetRegex.Match(contentType);

                if (m.Success)
                    charset = m.Groups["charset"].Value;

                content = (contentTransferEncoding != null) ? DecodeContent(contentTransferEncoding, filterMessage.MessageText, charset)
                      : filterMessage.MessageText;
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeMessage", "Exit");

            return content;
        }

        private static string DecodeBase64String(string charset, string encodedString)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeBase64String", "Entry");
            //string info = "@charset: " + charset + " @encodedString: " + encodedString.ToString();
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeBase64String", info);

            //encodedString = File.ReadAllText(@"D:\Deven.Deshpande\Office\19520\encoded-file.txt");

            string decodeString = encodedString;

            if (encodedString.Contains("FLAGS (\\"))
            {
                string[] stringSeparators = new string[] { "FLAGS (\\" };
                string[] result = encodedString.Split(stringSeparators, StringSplitOptions.None);
                //encodedString = string.Empty;
                if (!string.IsNullOrEmpty(result[0]))
                {
                    result[0] = result[0].Trim();
                    encodedString = result[0];
                }
            }

            try
            {
                if (charset.ToLower().Equals("utf8"))
                    charset = "utf-8";
                Decoder decoder = Encoding.GetEncoding(charset.ToLower().Trim()).GetDecoder();
                byte[] buffer = Convert.FromBase64String(encodedString);
                char[] charArray = new char[decoder.GetCharCount(buffer, 0, buffer.Length)];
                decoder.GetChars(buffer, 0, buffer.Length, charArray, 0);
                decodeString = new string(charArray);
            }
            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeBase64String", "Invalid Base64 encoding", ex);
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeBase64String", "Exit");

            return decodeString;
        }

        public static string DecodeQuotedPrintableSubject(string charset, string encodedString)
        {

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeQuotedPrintableSubject", "Entry");
            //string info = "@charset: " + charset + " @encodedString: " + encodedString.ToString();
            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeQuotedPrintableSubject", info);

            StringBuilder sbEncodString;

            string decodedString = encodedString;

            MatchCollection matches = QuotedPrintableRegex.Matches(encodedString);

            try
            {
                if (matches.Count > 0)
                {
                    sbEncodString = new StringBuilder();

                    sbEncodString.Append(QuotedPrintableRegex.Replace(encodedString, new MatchEvaluator(delegate (Match m)
                    {
                        string[] values = m.ToString().Trim().Split('=');
                        int divider = GetDivider(values[1]);
                        byte[] bytes = null;

                        switch (divider)
                        {
                            case 1:
                                bytes = new byte[divider];
                                break;
                            case 2:
                                bytes = new byte[2];
                                break;
                            case 4:
                                bytes = new byte[4];
                                break;
                            case 3:
                                bytes = new byte[m.Value.Length / divider];
                                break;
                        }
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            try
                            {
                                string hex = m.Value.Substring(i * 3 + 1, 2);
                                int iHex = Convert.ToInt32(hex, 16);
                                bytes[i] = Convert.ToByte(iHex);
                            }
                            catch (Exception ex)
                            {
                                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeQuotedPrintableSubject", string.Empty, ex);
                            }
                        }

                        return Encoding.GetEncoding(charset.ToLower().Trim()).GetString(bytes);

                    })));

                }
                else
                {
                    sbEncodString = new StringBuilder();
                    sbEncodString.Append(encodedString);
                }

                decodedString = Regex.Replace(sbEncodString.ToString(), "=\r\n", string.Empty);
                decodedString = decodedString.Replace("_", " ");
            }

            catch (Exception ex)
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeQuotedPrintableSubject", "Invalid QP encoding", ex);
            }

            //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.INFORMATION, "EmailParser: DecodeQuotedPrintableSubject", "Exit");

            return decodedString;
        }

        private static string extractAttachmentName(string fileName, string stringToBeReplaced)
        {
            if (!isUtfOrIsoEncodedFileName(fileName))
            {
                return fileName.Replace(stringToBeReplaced, string.Empty);
            }

            return DecodeSubject(fileName);
        }

        private static bool isUtfOrIsoEncodedFileName(string fileName)
        {
            return fileName.ToLower().Contains("utf-8")
                  || fileName.ToLower().Contains("iso-8859-1");
        }

        public void Trace(TraceEventArg e)
        {
            if (LogTrace != null)
                LogTrace(this, e);
        }
    }

    static class QuotedPrintable
    {

        public static string DecodeEncodedMessage(string toDecode, string charset)
        {
            string decodedText;

            try
            {
                Encoding encoding = Encoding.GetEncoding(charset.ToLower().Trim());
                decodedText = encoding.GetString(QuotedPrintableDecode(toDecode, true));
            }

            catch (Exception ex)
            {
                decodedText = "Unable to decode Message";
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeEncodedMessage", "Unable to decode Text", ex);
            }

            return decodedText;
        }


        private static byte[] QuotedPrintableDecode(string toDecode, bool encodedWordVariant)
        {


            // Create a byte array builder which is roughly equivalent to a StringBuilder
            using (MemoryStream byteArrayBuilder = new MemoryStream())
            {
                // Remove illegal control characters
                toDecode = RemoveIllegalControlCharacters(toDecode);

                // Run through the whole string that needs to be decoded
                for (int i = 0; i < toDecode.Length; i++)
                {
                    char currentChar = toDecode[i];
                    if (currentChar == '=')
                    {
                        // Check that there is at least two characters behind the equal sign
                        if (toDecode.Length - i < 3)
                        {
                            // We are at the end of the toDecode string, but something is missing. Handle it the way RFC 2045 states
                            WriteAllBytesToStream(byteArrayBuilder, DecodeEqualSignNotLongEnough(toDecode.Substring(i)));

                            // Since it was the last part, we should stop parsing anymore
                            break;
                        }

                        // Decode the Quoted-Printable part
                        string quotedPrintablePart = toDecode.Substring(i, 3);
                        WriteAllBytesToStream(byteArrayBuilder, DecodeEqualSign(quotedPrintablePart));

                        // We now consumed two extra characters. Go forward two extra characters
                        i += 2;
                    }
                    else
                    {
                        // This character is not quoted printable hex encoded.

                        // Could it be the _ character, which represents space
                        // and are we using the encoded word variant of QuotedPrintable
                        if (currentChar == '_' && encodedWordVariant)
                        {
                            // The RFC specifies that the "_" always represents hexadecimal 20 even if the
                            // SPACE character occupies a different code position in the character set in use.
                            byteArrayBuilder.WriteByte(0x20);
                        }
                        else
                        {
                            // This is not encoded at all. This is a literal which should just be included into the output.
                            byteArrayBuilder.WriteByte((byte)currentChar);
                        }
                    }
                }

                return byteArrayBuilder.ToArray();
            }
        }

        /// <summary>
        /// Writes all bytes in a byte array to a stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="toWrite">The bytes to write to the <paramref name="stream"/></param>
        private static void WriteAllBytesToStream(Stream stream, byte[] toWrite)
        {
            stream.Write(toWrite, 0, toWrite.Length);
        }


        private static string RemoveIllegalControlCharacters(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // First we remove any \r or \n which is not part of a \r\n pair
            input = RemoveCarriageReturnAndNewLinewIfNotInPair(input);

            // Here only legal \r\n is left over
            // We now simply keep them, and the \t which is also allowed
            // \x0A = \n
            // \x0D = \r
            // \x09 = \t)
            return Regex.Replace(input, "[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");
        }


        private static string RemoveCarriageReturnAndNewLinewIfNotInPair(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Use this for building up the new string. This is used for performance instead
            // of altering the input string each time a illegal token is found
            StringBuilder newString = new StringBuilder(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                // There is a character after it
                // Check for lonely \r
                // There is a lonely \r if it is the last character in the input or if there
                // is no \n following it
                if (input[i] == '\r' && (i + 1 >= input.Length || input[i + 1] != '\n'))
                {
                    // Illegal token \r found. Do not add it to the new string

                    // Check for lonely \n
                    // There is a lonely \n if \n is the first character or if there
                    // is no \r in front of it
                }
                else if (input[i] == '\n' && (i - 1 < 0 || input[i - 1] != '\r'))
                {
                    // Illegal token \n found. Do not add it to the new string
                }
                else
                {
                    // No illegal tokens found. Simply insert the character we are at
                    // in our new string
                    newString.Append(input[i]);
                }
            }

            return newString.ToString();
        }


        private static byte[] DecodeEqualSignNotLongEnough(string decode)
        {
            if (decode == null)
            {
                //throw new ArgumentNullException("decode");
            }
            // We can only decode wrong length equal signs
            if (decode.Length >= 3)
            {
                //throw new ArgumentException("decode must have length lower than 3", "decode");
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeEqualSignNotLongEnough", "decode must have length lower than 3");
            }
            // First char must be =
            if (decode[0] != '=')
            {
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeEqualSignNotLongEnough", "First part of decode must be an equal sign");
                //throw new ArgumentException("First part of decode must be an equal sign", "decode");
            }
            // We will now believe that the string sent to us, was actually not encoded
            // Therefore it must be in US-ASCII and we will return the bytes it corrosponds to
            return Encoding.ASCII.GetBytes(decode);
        }


        private static byte[] DecodeEqualSign(string decode)
        {
            if (decode == null)
            {
                //throw new ArgumentNullException("decode");
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "", string.Empty, ex);
            }
            // We can only decode the string if it has length 3 - other calls to this function is invalid
            if (decode.Length != 3)
            {
                //throw new ArgumentException("decode must have length 3", "decode");
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeEqualSign", "decode must have length lower than 3");
            }
            // First char must be =
            if (decode[0] != '=')
            {
                //throw new ArgumentException("decode must start with an equal sign", "decode");
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeEqualSign", "decode must start with an equal sign");
            }
            // There are two cases where an equal sign might appear
            // It might be a
            //   - hex-string like =3D, denoting the character with hex value 3D
            //   - it might be the last character on the line before a CRLF
            //     pair, denoting a soft linebreak, which simply
            //     splits the text up, because of the 76 chars per line restriction
            if (decode.Contains("\r\n"))
            {
                // Soft break detected
                // We want to return string.Empty which is equivalent to a zero-length byte array
                return new byte[0];
            }

            // Hex string detected. Convertion needed.
            // It might be that the string located after the equal sign is not hex characters
            // An example: =JU
            // In that case we would like to catch the FormatException and do something else
            try
            {
                // The number part of the string is the last two digits. Here we simply remove the equal sign
                string numberString = decode.Substring(1);

                // Now we create a byte array with the converted number encoded in the string as a hex value (base 16)
                // This will also handle illegal encodings like =3d where the hex digits are not uppercase,
                // which is a robustness requirement from RFC 2045.
                byte[] oneByte = new[] { Convert.ToByte(numberString, 16) };

                // Simply return our one byte byte array
                return oneByte;
            }
            catch (FormatException ex)
            {
                // RFC 2045 says about robust implementation:
                // An "=" followed by a character that is neither a
                // hexadecimal digit (including "abcdef") nor the CR
                // character of a CRLF pair is illegal.  This case can be
                // the result of US-ASCII text having been included in a
                // quoted-printable part of a message without itself
                // having been subjected to quoted-printable encoding.  A
                // reasonable approach by a robust implementation might be
                // to include the "=" character and the following
                // character in the decoded data without any
                // transformation and, if possible, indicate to the user
                // that proper decoding was not possible at this point in
                // the data.

                // So we choose to believe this is actually an un-encoded string
                // Therefore it must be in US-ASCII and we will return the bytes it corrosponds to
                //Automation.Common.Log.Write(Automation.Common.Log.Modules.Player, Automation.Common.Log.LogTypes.FATAL, "DecodeEqualSign", string.Empty, ex);
                return Encoding.ASCII.GetBytes(decode);
            }
        }
    }
}
