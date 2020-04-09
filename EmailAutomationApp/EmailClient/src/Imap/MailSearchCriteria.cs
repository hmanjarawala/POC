using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EmailAutomationLibrary.Imap
{
    public class MailSearchCriteria
    {
        #region Static Methods
        /// <summary>
		/// Finds all messages in the mailbox.
		/// </summary>
		/// <returns>A MailSearchCriteria object representing the "all" search criterion.</returns>
		public static MailSearchCriteria All()
        {
            return new MailSearchCriteria { Field = Fields.ALL };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the header or body of the message.
        /// </summary>
        /// <param name="text">String to search messages for.</param>
        /// <returns>A MailSearchCriteria object representing the "text" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria Text(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.TEXT, Value = text };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the envelope structure's BCC field.
        /// </summary>
        /// <param name="text">String to search the envelope structure's BCC field for.</param>
        /// <returns>A MailSearchCriteria object representing the "BCC" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria BCC(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.BCC, Value = text };
        }
        /// <summary>
        /// Finds messages whose internal date (disregarding time and timezone) is earlier than the
        /// specified date.
        /// </summary>
        /// <param name="date">The date to compare the message's internal date with.</param>
        /// <returns>A MailSearchCriteria object representing the "Before" search criterion.</returns>
        public static MailSearchCriteria Before(DateTime date)
        {
            return new MailSearchCriteria { Field = Fields.BEFORE, Value = date };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the body of the message.
        /// </summary>
        /// <param name="text">String to search the message body for.</param>
        /// <returns>A MailSearchCriteria object representing the "Body" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria Body(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.BODY, Value = text };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the envelope structure's CC field.
        /// </summary>
        /// <param name="text">String to search the envelope structure's CC field for.</param>
        /// <returns>A MailSearchCriteria object representing the "CC" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria Cc(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.CC, Value = text };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the envelope structure's FROM field.
        /// </summary>
        /// <param name="text">String to search the envelope structure's FROM field for.</param>
        /// <returns>A MailSearchCriteria object representing the "FROM" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria From(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.FROM, Value = text };
        }
        /// <summary>
        /// Finds messages that have a header with the specified field-name and that contains the
        /// specified string in the text of the header.
        /// </summary>
        /// <param name="name">field-name of the header to search for.</param>
        /// <param name="text">String to search for in the text of the header.</param>
        /// <returns>A MailSearchCriteria object representing the "HEADER" search criterion.</returns>
        /// <remarks>
        /// If the string to search is zero-length, this matches all messages that have a header line
        /// with the specified field-name regardless of the contents.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The name parameter or the text parameter is
        /// null.</exception>
        public static MailSearchCriteria Header(string name, string text)
        {
            name.ThrowIfNull("name");
            text.ThrowIfNull("text");
            return new MailSearchCriteria
            {
                Field = Fields.HEADER,
                Quote = false,
                Value = name + " " + text.QuoteString()
            };
        }
        /// <summary>
        /// Finds messages with the specified keyword flag set.
        /// </summary>
        /// <param name="text">The keyword flag to search for.</param>
        /// <returns>A MailSearchCriteria object representing the "KEYWORD" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria Keyword(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.KEYWORD, Value = text };
        }
        /// <summary>
        /// Finds messages with a size larger than the specified number of bytes.
        /// </summary>
        /// <param name="size">Minimum size, in bytes a message must have to be included in the search
        /// result.</param>
        /// <returns>A MailSearchCriteria object representing the "LARGER" search criterion.</returns>
        public static MailSearchCriteria Larger(long size)
        {
            return new MailSearchCriteria { Field = Fields.LARGER, Value = size };
        }
        /// <summary>
        /// Finds messages with a size smaller than the specified number of bytes.
        /// </summary>
        /// <param name="size">Maximum size, in bytes a message must have to be included in the search
        /// result.</param>
        /// <returns>A MailSearchCriteria object representing the "SMALLER" search criterion.</returns>
        public static MailSearchCriteria Smaller(long size)
        {
            return new MailSearchCriteria { Field = Fields.SMALLER, Value = size };
        }
        /// <summary>
        /// Finds messages whose Date: header (disregarding time and timezone) is earlier than the
        /// specified date.
        /// </summary>
        /// <param name="date">The date to compare the Date: header field with.</param>
        /// <returns>A MailSearchCriteria object representing the "SENTBEFORE" search criterion.</returns>
        public static MailSearchCriteria SentBefore(DateTime date)
        {
            return new MailSearchCriteria { Field = Fields.SENTBEFORE, Value = date };
        }
        /// <summary>
        /// Finds messages whose Date: header (disregarding time and timezone) is within the specified
        /// date.
        /// </summary>
        /// <param name="date">The date to compare the Date: header field with.</param>
        /// <returns>A MailSearchCriteria object representing the "SENTON" search criterion.</returns>
        public static MailSearchCriteria SentOn(DateTime date)
        {
            return new MailSearchCriteria { Field = Fields.SENTON, Value = date };
        }
        /// <summary>
        /// Finds messages whose Date: header (disregarding time and timezone) is within or later than
        /// the specified date.
        /// </summary>
        /// <param name="date">The date to compare the Date: header field with.</param>
        /// <returns>A MailSearchCriteria object representing the "SENTSINCE" search criterion.</returns>
        public static MailSearchCriteria SentSince(DateTime date)
        {
            return new MailSearchCriteria { Field = Fields.SENTSINCE, Value = date };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the envelope structure's SUBJECT field.
        /// </summary>
        /// <param name="text">String to search the envelope structure's SUBJECT field for.</param>
        /// <returns>A MailSearchCriteria object representing the "SUBJECT" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria Subject(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.SUBJECT, Value = text };
        }
        /// <summary>
        /// Finds messages that contain the specified string in the envelope structure's TO field.
        /// </summary>
        /// <param name="text">String to search the envelope structure's TO field for.</param>
        /// <returns>A MailSearchCriteria object representing the "TO" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria To(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.TO, Value = text };
        }
        /// <summary>
        /// Finds messages with unique identifiers corresponding to the specified unique identifier set.
        /// </summary>
        /// <param name="uids">One or several unique identifiers (UID).</param>
        /// <returns>A MailSearchCriteria object representing the "UID" search criterion.</returns>
        public static MailSearchCriteria UID(params uint[] uids)
        {
            return new MailSearchCriteria
            {
                Field = Fields.UID,
                Value = uids
            };
        }
        /// <summary>
        /// Finds messages with a unique identifier greater than the specified unique identifier.
        /// </summary>
        /// <param name="uid">A unique identifier (UID).</param>
        /// <returns>A MailSearchCriteria object representing the "UID" search criterion.</returns>
        /// <remarks>
        /// Because of the nature of the IMAP search mechanism, the result set will always contain the
        /// UID of the last message in the mailbox, even if said UID is smaller than the UID specified.
        /// </remarks>
        public static MailSearchCriteria GreaterThan(uint uid)
        {
            return new MailSearchCriteria
            {
                Field = Fields.UID,
                Value = (uid + 1).ToString() + ":*",
                Quote = false
            };
        }
        /// <summary>
        /// Finds messages with a unique identifier less than the specified unique identifier.
        /// </summary>
        /// <param name="uid">A unique identifier (UID).</param>
        /// <returns>A MailSearchCriteria object representing the "UID" search criterion.</returns>
        public static MailSearchCriteria LessThan(uint uid)
        {
            return new MailSearchCriteria
            {
                Field = Fields.UID,
                Value = "1:" + (uid - 1).ToString(),
                Quote = false
            };
        }
        /// <summary>
        /// Finds messages that do not have the specified keyword flag set.
        /// </summary>
        /// <param name="text">The IMAP keyword flag to search for.</param>
        /// <returns>A MailSearchCriteria object representing the "UNKEYWORD" search criterion.</returns>
        /// <exception cref="ArgumentNullException">The text parameter is null.</exception>
        public static MailSearchCriteria Unkeyword(string text)
        {
            text.ThrowIfNull("text");
            return new MailSearchCriteria { Field = Fields.UNKEYWORD, Value = text };
        }
        /// <summary>
        /// Finds messages that have the \Answered flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "ANSWERED" search criterion.</returns>
        public static MailSearchCriteria Answered()
        {
            return new MailSearchCriteria { Field = Fields.ANSWERED };
        }
        /// <summary>
        /// Finds messages that have the \Deleted flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "DELETED" search criterion.</returns>
        public static MailSearchCriteria Deleted()
        {
            return new MailSearchCriteria { Field = Fields.DELETED };
        }
        /// <summary>
        /// Finds messages that have the \Draft flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "DRAFT" search criterion.</returns>
        public static MailSearchCriteria Draft()
        {
            return new MailSearchCriteria { Field = Fields.DRAFT };
        }
        /// <summary>
        /// Finds messages that have the \Flagged flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "FLAGGED" search criterion.</returns>
        public static MailSearchCriteria Flagged()
        {
            return new MailSearchCriteria { Field = Fields.FLAGGED };
        }
        /// <summary>
        /// Finds messages that have the \Recent flag set but not the \Seen flag.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "NEW" search criterion.</returns>
        public static MailSearchCriteria New()
        {
            return new MailSearchCriteria { Field = Fields.NEW };
        }
        /// <summary>
        /// Finds messages that do not have the \Recent flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "OLD" search criterion.</returns>
        public static MailSearchCriteria Old()
        {
            return new MailSearchCriteria { Field = Fields.OLD };
        }
        /// <summary>
        /// Finds messages that have the \Recent flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "RECENT" search criterion.</returns>
        public static MailSearchCriteria Recent()
        {
            return new MailSearchCriteria { Field = Fields.RECENT };
        }
        /// <summary>
        /// Finds messages that have the \Seen flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "SEEN" search criterion.</returns>
        public static MailSearchCriteria Seen()
        {
            return new MailSearchCriteria { Field = Fields.SEEN };
        }
        /// <summary>
        /// Finds messages that do not have the \Answered flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "UNANSWERED" search criterion.</returns>
        public static MailSearchCriteria Unanswered()
        {
            return new MailSearchCriteria { Field = Fields.UNANSWERED };
        }
        /// <summary>
        /// Finds messages that do not have the \Deleted flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "UNDELETED" search criterion.</returns>
        public static MailSearchCriteria Undeleted()
        {
            return new MailSearchCriteria { Field = Fields.UNDELETED };
        }
        /// <summary>
        /// Finds messages that do not have the \Draft flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "UNDRAFT" search criterion.</returns>
        public static MailSearchCriteria Undraft()
        {
            return new MailSearchCriteria { Field = Fields.UNDRAFT };
        }
        /// <summary>
        /// Finds messages that do not have the \Flagged flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "UNFLAGGED" search criterion.</returns>
        public static MailSearchCriteria Unflagged()
        {
            return new MailSearchCriteria { Field = Fields.UNFLAGGED };
        }
        /// <summary>
        /// Finds messages that do not have the \Seen flag set.
        /// </summary>
        /// <returns>A MailSearchCriteria object representing the "UNSEEN" search criterion.</returns>
        public static MailSearchCriteria Unseen()
        {
            return new MailSearchCriteria { Field = Fields.UNSEEN };
        }

        /// <summary>
        /// Logically ANDs multiple search conditions, meaning a message will only be included in the
        /// search result if both of the ANDed conditions are met.
        /// </summary>
        /// <param name="other">A search condition to logically AND this MailSearchCriteria instance
        /// with.</param>
        /// <returns>A new MailSearchCriteria instance which can be further chained with other search
        /// conditions.</returns>
        /// <exception cref="ArgumentNullException">The other parameter is null.</exception>
        public MailSearchCriteria And(MailSearchCriteria other)
        {
            other.ThrowIfNull("other");
            return Join(string.Empty, this, other);
        }

        /// <summary>
        /// Logically negates search conditions, meaning a message will only be included in the search
        /// result if the specified conditions are not met.
        /// </summary>
        /// <param name="other">A search condition that must not be met by a message for it to be
        /// included in the search result set.</param>
        /// <returns>A new MailSearchCriteria instance which can be further chained with other search
        /// conditions.</returns>
        /// <exception cref="ArgumentNullException">The other parameter is null.</exception>
        public MailSearchCriteria Not(MailSearchCriteria other)
        {
            other.ThrowIfNull("other");
            return Join("NOT", this, other);
        }

        /// <summary>
        /// Logically ORs multiple search conditions, meaning a message will be included in the search
        /// result if it meets at least either of the conditions.
        /// </summary>
        /// <param name="other">A search condition to logically OR this MailSearchCriteria instance
        /// with.</param>
        /// <returns>A new MailSearchCriteria instance which can be further chained with other search
        /// conditions.</returns>
        /// <exception cref="ArgumentNullException">The other parameter is null.</exception>
        public MailSearchCriteria Or(MailSearchCriteria other)
        {
            other.ThrowIfNull("other");
            return Join("OR", this, other);
        }
        /// <summary>
		/// Joins two MailSearchCriteria objects into a new one using the specified logical operator.
		/// </summary>
		/// <param name="condition">The logical operator to use for joining the search conditions.
		/// Possible values are "OR", "NOT" and the empty string "" which denotes a logical AND.</param>
		/// <param name="left">The first MailSearchCriteria object</param>
		/// <param name="right">The second MailSearchCriteria object</param>
		/// <returns>A new MailSearchCriteria object representing the two search conditions joined by the
		/// specified logical operator.</returns>
        static MailSearchCriteria Join(string condition, MailSearchCriteria left, MailSearchCriteria right)
        {
            return new MailSearchCriteria() { Operator = condition.ToUpper(), Criterias = new List<MailSearchCriteria> { left, right } };
        }

        /// <summary>
		/// Concatenates the members of a collection, using the specified separator between each
		/// member.
		/// </summary>
		/// <typeparam name="T">The type of the members of values.</typeparam>
		/// <param name="seperator">The string to use as a separator.</param>
		/// <param name="values">A collection that contains the objects to concatenate.</param>
		/// <returns>A string that consists of the members of values delimited by the separator
		/// string. If values has no members, the method returns System.String.Empty.</returns>
		/// <exception cref="ArgumentNullException">The values parameter is null.</exception>
		/// <remarks>This is already part of the String class in .NET 4.0 and newer but is needed
		/// for backwards compatibility with .NET 3.5.</remarks>
        static string Join<T>(string seperator, IEnumerable<T> values)
        {
            values.ThrowIfNull(nameof(values));
            IList<string> list = new List<string>();
            foreach (T v in values)
                list.Add(v.ToString());
            return string.Join(seperator, list.ToArray());
        }

        #endregion

        #region Fields enum
        /// <summary>
        /// The search keys which can be used with the IMAP SEARCH command, as are defined in section
        /// 6.4.4 of RFC 3501.
        /// </summary>
        enum Fields
        {
            BCC, BEFORE, BODY, CC, FROM, HEADER, KEYWORD,
            LARGER, ON, SENTBEFORE, SENTON, SENTSINCE, SINCE, SMALLER, SUBJECT,
            TEXT, TO, UID, UNKEYWORD, ALL, ANSWERED, DELETED, DRAFT, FLAGGED,
            NEW, OLD, RECENT, SEEN, UNANSWERED, UNDELETED, UNDRAFT, UNFLAGGED, UNSEEN
        }
        #endregion

        Fields? Field;

        object Value = null;

        IEnumerable<MailSearchCriteria> Criterias;

        string Operator;

        bool Quote = true;

        private MailSearchCriteria() { }

        /// <summary>
		/// Constructs a string from the MailSearchCriteria object using the proper syntax as is required
		/// for the IMAP SEARCH command.
		/// </summary>
		/// <returns>A string representing this MailSearchCriteria instance that can be used with the IMAP
		/// SEARCH command.</returns>
        public override string ToString()
        {
            if (Criterias != null && Criterias.Count() > 0 && Operator != null)
                return (Operator.ToUpper() + " (" + Join(") (", Criterias) + ")").Trim();

            StringBuilder builder = new StringBuilder();

            if (Field != null)
                builder.Append(Field.ToString());

            object Val = Value;
            if (Val != null)
            {
                if (Field != null)
                    builder.Append(" ");
                if (Val is string)
                {
                    string s = (string)Val;
                    // If the string contains non-ASCII characters we must use the somewhat cumbersome literal
                    // form as is outlined in RFC 3501 Section 4.3.
                    if (!s.IsASCII())
                    {
                        builder.AppendLine("{" + Encoding.UTF8.GetBytes(s).Length + "}");
                    }
                    else
                    {
                        if (Quote)
                            Val = ((string)Val).QuoteString();
                    }
                }
                else if (Val is DateTime)
                {
                    Val = ((DateTime)Val).ToString("dd-MMM-yyyy",
                        CultureInfo.InvariantCulture).QuoteString();
                }
                else if (Val is uint[])
                {
                    Val = Join(",", (uint[])Val);
                }
                builder.Append(Val);
            }
            return builder.ToString();
        }
    }
}
