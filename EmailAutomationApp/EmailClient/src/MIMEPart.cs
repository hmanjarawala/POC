using System.Collections.Specialized;

namespace EmailAutomationLibrary
{
    /// <summary>
    /// It represents a part of the body of the email. 
    /// Since most of the emails are MIME encoded, which means that there can be more than one type (text/plain, text/html, etc.) of message part, 
    /// and that there can be attached files, we represent the body of the email with an array of MessagePart objects. 
    /// This class lets you access the headers and body of a message part of mail.
    /// </summary>
    public class MIMEPart
    {
        public NameValueCollection Headers { get; set; }

        public string MessageText { get; set; }

        public MIMEPart() { }
    }
}
