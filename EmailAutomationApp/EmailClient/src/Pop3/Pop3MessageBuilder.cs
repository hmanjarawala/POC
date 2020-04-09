using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EmailAutomationLibrary.Pop3
{
    internal class Pop3MessageBuilder : MessageBuilder
    {
        internal override MailAddress[] ParseAddressList(string list)
        {
            List<MailAddress> mails = new List<MailAddress>();
            string[] addr = list.Split(',');
            foreach (string a in addr)
            {
                Match m = Regex.Match(a.Trim(),
                    @"(.*)\s*<?([A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4})>?",
                    RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                if (m.Success)
                {
                    // The above regex will erroneously match some illegal (very rare)
                    // local-parts. RFC-compliant validation is not worth the effort
                    // at all, so just wrap this in a try/catch block in case
                    // MailAddress' ctor complains.
                    try
                    {
                        mails.Add(new MailAddress(m.Groups[2].Value, m.Groups[1].Value));
                    }
                    catch { }
                }
            }
            return mails.ToArray();
        }

        protected override NameValueCollection ParseMIMEField(string field)
        {
            NameValueCollection coll = new NameValueCollection();
            try
            {
                MatchCollection matches = Regex.Matches(field,
                    "([\\w\\-]+)\\s*=\\s*([^;]+)");
                foreach (Match m in matches)
                    coll.Add(m.Groups[1].Value, m.Groups[2].Value.Trim('"'));
                Match mvalue = Regex.Match(field, @"^\s*([^;]+)");
                coll.Add("value", mvalue.Success ? mvalue.Groups[1].Value.Trim() : "");
            }
            catch
            {
                // We don't want this to blow up on the user with weird mails so
                // just return an empty collection.
                coll.Add("value", String.Empty);
            }
            return coll;
        }
    }
}
