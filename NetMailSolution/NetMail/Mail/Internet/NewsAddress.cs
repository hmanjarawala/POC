using CoffeeBean.Mail.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeBean.Mail.Internet
{
    /// <summary>
    /// This class models an RFC1036 newsgroup address.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class NewsAddress : Address
    {
        private string newsGroup;
        private string host; // may be null

        private static readonly long serialVersionUID = -4203797299824684143L;

        /// <summary>
        /// Default constructor
        /// </summary>
        public NewsAddress() { }

        /// <summary>
        /// Construct a NewsAddress with the given newsgroup.
        /// </summary>
        /// <param name="newsGroup">the newsgroup</param>
        public NewsAddress(string newsGroup) : this(newsGroup, null) { }

        /// <summary>
        /// Construct a NewsAddress with the given newsgroup and host.
        /// </summary>
        /// <param name="newsGroup">the newsgroup</param>
        /// <param name="host">the host</param>
        public NewsAddress(string newsGroup, string host)
        {
            this.newsGroup = newsGroup.Replace("\\s+", "");
            this.host = host;
        }

        /// <summary>
        /// Get/Set the news group.
        /// </summary>
        public string NewsGroup
        {
            get { return newsGroup; }
            set { newsGroup = value; }
        }

        /// <summary>
        /// Get/Set the host.
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// Return the type of this address.  The type of a NewsAddress
        /// is "news".
        /// </summary>
        public override string GetAddressType()
        {
            return "news";
        }

        /// <summary>
        /// The equality operator.
        /// </summary>
        protected override bool equals(object address)
        {
            if (!(address is NewsAddress)) return false;

            NewsAddress s = (NewsAddress)address;
            return (((string.IsNullOrEmpty(newsGroup) && string.IsNullOrEmpty(s.newsGroup)) ||
                (!string.IsNullOrEmpty(newsGroup) && newsGroup.Equals(s.newsGroup))) &&
                ((string.IsNullOrEmpty(host) && string.IsNullOrEmpty(s.host)) || 
                (!string.IsNullOrEmpty(host) && host.Equals(s.host))));
        }

        /// <summary>
        /// Compute a hash code for the address.
        /// </summary>
        public override int GetHashCode()
        {
            int hash = 0;

            if (!string.IsNullOrEmpty(newsGroup)) hash += newsGroup.GetHashCode();

            if (!string.IsNullOrEmpty(host)) hash += host.ToLowerInvariant().GetHashCode();

            return hash;
        }

        /// <summary>
        /// Convert this address into a RFC 1036 address.
        /// </summary>
        /// <returns>newsGroup</returns>
        protected override string toString()
        {
            return newsGroup;
        }

        /// <summary>
        /// Convert the given array of NewsAddress objects into
        /// a comma separated sequence of address strings. The
        /// resulting string contains only US-ASCII characters, and
        /// hence is mail-safe.
        /// </summary>
        /// <param name="addresses">array of NewsAddress objects</param>
        /// <exception cref="InvalidCastException">if any address object in the
        ///              	given array is not a NewsAddress objects. Note
        ///              	that this is a RuntimeException.</exception>
        /// <returns>comma separated address strings</returns>
        public static string ToString(Address[] addresses)
        {
            if (addresses.IsNull() || addresses.Length == 0) return null;

            StringBuilder builder = new StringBuilder(((NewsAddress)addresses[0]).toString());
            int used = builder.Length;
            for(int i = 1; i < addresses.Length; i++)
            {
                builder.Append(",");
                used++;
                string ng = ((NewsAddress)addresses[i]).toString();

                if((used + ng.Length) > 76)
                {
                    builder.Append("\r\n\t");
                    used = 8;
                }
                builder.Append(ng);
                used += ng.Length;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Parse the given comma separated sequence of newsgroups into
        /// NewsAddress objects.
        /// </summary>
        /// <param name="newsGroups">comma separated newsgroup string</param>
        /// <returns>array of NewsAddress objects</returns>
        /// <exception cref="AddressException">if the parse failed</exception>
        public static NewsAddress[] Parse(string newsGroups)
        {
            try
            {
                string[] tokens = newsGroups.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                IList<NewsAddress> nglist = new List<NewsAddress>(tokens.Length);
                foreach (string token in tokens)
                {
                    nglist.Add(new NewsAddress(token));
                }
                return nglist.ToArray();
            }
            catch (Exception e) { throw new AddressException(e.Message); }
        }
    }
}
