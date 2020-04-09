using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailAutomationLibrary.Imap
{
    internal class ImapMessageBuilder : MessageBuilder
    {
        internal override MailAddress[] ParseAddressList(string list)
        {
            List<MailAddress> mails = new List<MailAddress>();

            if (string.IsNullOrEmpty(list))
                return mails.ToArray();

            foreach (string part in SplitAddressList(list))
            {
                MailAddressCollection mcol = new MailAddressCollection();
                try
                {
                    // .NET won't accept address-lists ending with a ';' or a ',' character, see #68.
                    mcol.Add(part.TrimEnd(';', ','));
                    foreach (MailAddress m in mcol)
                    {
                        // We might still need to decode the display name if it is Q-encoded.
                        string displayName = Util.DecodeWords(m.DisplayName);
                        mails.Add(new MailAddress(m.Address, displayName));
                    }
                }
                catch
                {
                    // We don't want this to throw any exceptions even if the entry is malformed.
                }
            }
            return mails.ToArray();
        }

        internal IEnumerable<string> SplitAddressList(string list)
        {
            IList<string> parts = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool inQuotes = false;
            char last = '.';
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == '"' && last != '\\')
                    inQuotes = !inQuotes;
                if (list[i] == ',' && !inQuotes)
                {
                    parts.Add(builder.ToString().Trim());
                    builder.Length = 0;
                }
                else {
                    builder.Append(list[i]);
                }
                if (i == list.Length - 1)
                    parts.Add(builder.ToString().Trim());
            }
            return parts;
        }

        protected override NameValueCollection ParseMIMEField(string field)
        {
            NameValueCollection coll = new NameValueCollection();
            var fixup = new HashSet<string>();
            try
            {
                // This accounts for MIME Parameter Value Extensions (RFC2231).
                MatchCollection matches = Regex.Matches(field,
                    @"([\w\-]+)(?:\*\d{1,3})?(\*?)?\s*=\s*([^;]+)");
                foreach (Match m in matches)
                {
                    string pname = m.Groups[1].Value.Trim(), pval = m.Groups[3].Value.Trim('"');
                    coll[pname] = coll[pname] + pval;
                    if (m.Groups[2].Value == "*")
                        fixup.Add(pname);
                }
                foreach (var pname in fixup)
                {
                    try
                    {
                        coll[pname] = Util.Rfc2231Decode(coll[pname]);
                    }
                    catch (FormatException)
                    {
                        // If decoding fails, we should at least return the un-altered value.
                    }
                }
                Match mvalue = Regex.Match(field, @"^\s*([^;]+)");
                coll.Add("value", mvalue.Success ? mvalue.Groups[1].Value.Trim() : "");
            }
            catch
            {
                // We don't want this to blow up on the user with weird mails.
                coll.Add("value", String.Empty);
            }
            return coll;
        }
    }
}
