using System;

namespace EmailAutomationLibrary.Imap
{
    public class Mailbox : IComparable
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Separator { get; set; }

        public Mailbox(string name, string fullName, string separator = "/")
        {
            FullName = fullName;
            Separator = separator;
            Name = name;
        }

        public int CompareTo(object other)
        {
            if (other == null) return 1;

            Mailbox otherMailbox = other as Mailbox;

            if (otherMailbox == null)
                return 1;
            return FullName.CompareTo(otherMailbox.FullName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            Mailbox otherMailbox = obj as Mailbox;

            if (otherMailbox == null)
                return false;
            return FullName.Equals(otherMailbox.FullName);
        }
    }
}
