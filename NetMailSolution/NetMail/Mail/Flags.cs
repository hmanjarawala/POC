using CoffeeBean.Mail.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeBean.Mail
{
    /// <summary>
    /// The Flags class represents the set of flags on a Message.  Flags
    /// are composed of predefined system flags, and user defined flags.
    /// 
    /// A System flag is represented by the <code>Flags.Flag</code> 
    /// inner class. A User defined flag is represented as a String.
    /// User flags are case-independent.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    [Serializable]
    public class Flags : ICloneable
    {
        private uint system_flags = 0;
        // used as a case-independent Set that preserves the original case,
        // the key is the lowercase flag name and the value is the original
        private IDictionary<string, string> user_flags = null;

        private readonly static uint ANSWERED_BIT = 0x01;
        private readonly static uint DELETED_BIT = 0x02;
        private readonly static uint DRAFT_BIT = 0x04;
        private readonly static uint FLAGGED_BIT = 0x08;
        private readonly static uint RECENT_BIT = 0x10;
        private readonly static uint SEEN_BIT = 0x20;
        private readonly static uint USER_BIT = 0x80000000;

        private static readonly long serialVersionUID = 6243590407214169028L;

        /// <summary>
        /// This inner class represents an individual system flag. A set
        /// of standard system flag objects are predefined here.
        /// </summary>
        public sealed class Flag
        {
            /// <summary>
            /// This message has been answered. This flag is set by clients 
            /// to indicate that this message has been answered to.
            /// </summary>
            public static readonly Flag ANSWERED = new Flag(ANSWERED_BIT);

            /// <summary>
            /// This message is marked deleted. Clients set this flag to
            /// mark a message as deleted. The expunge operation on a folder
            /// removes all messages in that folder that are marked for deletion.
            /// </summary>
            public static readonly Flag DELETED = new Flag(DELETED_BIT);

            /// <summary>
            /// This message is a draft. This flag is set by clients
            /// to indicate that the message is a draft message.
            /// </summary>
            public static readonly Flag DRAFT = new Flag(DRAFT_BIT);

            /// <summary>
            /// This message is flagged. No semantic is defined for this flag.
            /// Clients alter this flag.
            /// </summary>
            public static readonly Flag FLAGGED = new Flag(FLAGGED_BIT);

            /// <summary>
            /// This message is recent. Folder implementations set this flag
            /// to indicate that this message is new to this folder, that is,
            /// it has arrived since the last time this folder was opened.
            /// 
            /// Clients cannot alter this flag.
            /// </summary>
            public static readonly Flag RECENT = new Flag(RECENT_BIT);

            /// <summary>
            /// This message is seen. This flag is implicitly set by the 
            /// implementation when this Message's content is returned 
            /// to the client in some form. The <code>getInputStream</code>
            /// and <code>getContent</code> methods on Message cause this
            /// flag to be set.
            /// 
            /// Clients can alter this flag.
            /// </summary>
            public static readonly Flag SEEN = new Flag(SEEN_BIT);

            /// <summary>
            /// A special flag that indicates that this folder supports
            /// user defined flags.
            /// 
            /// The implementation sets this flag. Clients cannot alter 
            /// this flag but can use it to determine if a folder supports
            /// user defined flags by using
            /// <code>folder.GetPermanentFlags().contains(Flags.Flag.USER)</code>.
            /// </summary>
            public static readonly Flag USER = new Flag(USER_BIT);

            private uint bit;
            private Flag(uint bit)
            {
                this.bit = bit;
            }

            public uint Bit { get { return bit; } }
        }

        /// <summary>
        /// Construct an empty Flags object.
        /// </summary>
        public Flags() { }

        /// <summary>
        /// Construct a Flags object initialized with the given flags.
        /// </summary>
        /// <param name="flags">the flags for initialization</param>
        public Flags(Flags flags)
        {
            this.system_flags = flags.system_flags;
            if (flags.user_flags.IsNotNull()) user_flags = flags.user_flags;
        }

        /// <summary>
        /// Construct a Flags object initialized with the given system flag.
        /// </summary>
        /// <param name="flag">the flag for initialization</param>
        public Flags(Flag flag)
        {
            system_flags |= flag.Bit;
        }

        /// <summary>
        /// Construct a Flags object initialized with the given user flag.
        /// 
        /// </summary>
        /// <param name="flag">the flag for initialization</param>
        public Flags(string flag)
        {
            user_flags = new Dictionary<string, string>(1);
            user_flags.Add(flag.ToLowerInvariant(), flag);
        }

        /// <summary>
        /// Add the specified system flag to this Flags object.
        /// </summary>
        /// <param name="flag">the flag to add</param>
        public void Add(Flag flag)
        {
            system_flags |= flag.Bit;
        }

        /// <summary>
        /// Add the specified user flag to this Flags object.
        /// </summary>
        /// <param name="flag">the flag to add</param>
        public void Add(string flag)
        {
            if (user_flags.IsNull())
                user_flags = new Dictionary<string, string>(1);
            user_flags.Add(flag.ToLowerInvariant(), flag);
        }

        /// <summary>
        /// Add all the flags in the given Flags object to this
        /// Flags object.
        /// </summary>
        /// <param name="f">Flags object</param>
        public void Add(Flags f)
        {
            system_flags |= f.system_flags; // add system flags

            if (f.user_flags.IsNotNull()) // add user-defined flags
            {
                if (user_flags.IsNull())
                    user_flags = new Dictionary<string, string>(1);

                foreach(var key in f.user_flags.Keys)
                {
                    user_flags.Add(key, f.user_flags[key]);
                }
            }
        }

        /// <summary>
        /// Remove the specified system flag from this Flags object.
        /// </summary>
        /// <param name="flag">the flag to be removed</param>
        public void Remove(Flag flag)
        {
            system_flags &= ~flag.Bit;
        }

        /// <summary>
        /// Remove the specified user flag from this Flags object.
        /// </summary>
        /// <param name="flag">the flag to be removed</param>
        public void Remove(string flag)
        {
            if (user_flags.IsNotNull())
                user_flags.Remove(flag.ToLowerInvariant());
        }

        /// <summary>
        /// Remove all flags in the given Flags object from this 
        /// Flags object.
        /// </summary>
        /// <param name="f">Flags object</param>
        public void Remove(Flags f)
        {
            system_flags &= ~f.system_flags;

            if (f.user_flags.IsNotNull())
            {
                if (user_flags.IsNull())
                    return;

                foreach (var key in f.user_flags.Keys)
                    user_flags.Remove(key);
            }
        }

        /// <summary>
        /// Check whether the specified system flag is present in this Flags object.
        /// </summary>
        /// <param name="flag">the flag to test</param>
        /// <returns>true of the given flag is present, otherwise false.</returns>
        public bool Contains(Flag flag)
        {
            return (system_flags & flag.Bit) != 0;
        }

        /// <summary>
        /// Check whether the specified user flag is present in this Flags object.
        /// </summary>
        /// <param name="flag">the flag to test</param>
        /// <returns>true of the given flag is present, otherwise false.</returns>
        public bool Contains(string flag)
        {
            if (user_flags.IsNull())
                return false;
            else
                return user_flags.ContainsKey(flag.ToLowerInvariant());
        }

        /// <summary>
        /// Check whether all the flags in the specified Flags object are
        /// present in this Flags object.
        /// 
        /// </summary>
        /// <param name="f">the flags to test</param>
        /// <returns>true if all flags in the given Flags object are present, 
        /// 	otherwise false.</returns>
        public bool Contains(Flags f)
        {
            // Check system flags
            if ((f.system_flags & system_flags) != f.system_flags)
                return false;
            // Check user flags
            if (f.user_flags.IsNotNull())
            {
                if (user_flags.IsNull())
                    return false;

                foreach(var key in f.user_flags.Keys)
                {
                    if (!user_flags.ContainsKey(key))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Remove any flags <strong>not</strong> in the given Flags object.
        /// Useful for clearing flags not supported by a server.  If the
        /// given Flags object includes the Flags.Flag.USER flag, all user
        /// flags in this Flags object are retained.
        /// </summary>
        /// <param name="f">the flags to keep</param>
        /// <returns>true if this Flags object changed</returns>
        public bool RetainAll(Flags f)
        {
            bool changed = false;
            uint sf = system_flags & f.system_flags;
            if(system_flags != sf)
            {
                system_flags = sf;
                changed = true;
            }

            // if we have user flags, and the USER flag is not set in "f",
            // determine which user flags to clear
            if(user_flags.IsNotNull() && (f.system_flags & USER_BIT) == 0)
            {
                if (f.user_flags.IsNotNull())
                {
                    foreach(var key in user_flags.Keys)
                    {
                        if (!f.user_flags.ContainsKey(key))
                        {
                            user_flags.Remove(key);
                            changed = true;
                        }
                    }
                }
                else
                {
                    changed = user_flags.Count > 0;
                    user_flags = null;
                }
            }
            return changed;
        }

        /// <summary>
        /// Check whether the two Flags objects are equal.
        /// </summary>
        /// <returns>true if they're equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Flags))
                return false;

            Flags f = (Flags)obj;

            // Check system flags
            if (system_flags != f.system_flags)
                return false;
            // Check user flags
            int size = (user_flags.IsNull()) ? 0 : user_flags.Count;
            int fsize = (f.user_flags.IsNull()) ? 0 : f.user_flags.Count;
            if (size == 0 && fsize == 0)
                return true;
            if (user_flags.IsNotNull() && f.user_flags.IsNotNull() && size == fsize)
                return user_flags.Keys.Equals(f.user_flags.Keys);
            return false;
        }

        /// <summary>
        /// Compute a hash code for this Flags object.
        /// </summary>
        /// <returns>the hash code</returns>
        public override int GetHashCode()
        {
            int hash = (int)system_flags;
            if (user_flags.IsNotNull())
            {
                foreach (var key in user_flags.Keys)
                    hash += key.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Return all the system flags in this Flags object.  Returns
        /// an array of size zero if no flags are set.
        /// </summary>
        /// <returns>array of Flags.Flag objects representing system flags</returns>
        public Flag[] GetSystemFlags()
        {
            IList<Flag> list = new List<Flag>(7);

            if ((system_flags & ANSWERED_BIT) != 0)
                list.Add(Flag.ANSWERED);
            if ((system_flags & DELETED_BIT) != 0)
                list.Add(Flag.DELETED);
            if ((system_flags & DRAFT_BIT) != 0)
                list.Add(Flag.DRAFT);
            if ((system_flags & FLAGGED_BIT) != 0)
                list.Add(Flag.FLAGGED);
            if ((system_flags & RECENT_BIT) != 0)
                list.Add(Flag.RECENT);
            if ((system_flags & SEEN_BIT) != 0)
                list.Add(Flag.SEEN);
            if ((system_flags & USER_BIT) != 0)
                list.Add(Flag.USER);

            Flag[] f = new Flag[list.Count];
            list.CopyTo(f, 0);
            return f;
        }

        /// <summary>
        /// Return all the user flags in this Flags object.  Returns
        /// an array of size zero if no flags are set.
        /// 
        /// </summary>
        /// <returns>array of Strings, each String represents a flag.</returns>
        public string[] GetUserFlags()
        {
            List<string> list = new List<string>();

            foreach (var element in user_flags.Values)
                list.Add(element);
            return list.ToArray();
        }

        /// <summary>
        /// Clear all of the system flags.
        /// </summary>
        public void ClearSystemFlags()
        {
            system_flags = 0;
        }

        /// <summary>
        /// Clear all of the user flags.
        /// </summary>
        public void ClearUserFlags()
        {
            user_flags = null;
        }

        /// <summary>
        /// Return a string representation of this Flags object.
        /// Note that the exact format of the string is subject to change.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if ((system_flags & ANSWERED_BIT) != 0)
                sb.Append("\\Answered ");
            if ((system_flags & DELETED_BIT) != 0)
                sb.Append("\\Deleted ");
            if ((system_flags & DRAFT_BIT) != 0)
                sb.Append("\\Draft ");
            if ((system_flags & FLAGGED_BIT) != 0)
                sb.Append("\\Flagged ");
            if ((system_flags & RECENT_BIT) != 0)
                sb.Append("\\Recent ");
            if ((system_flags & SEEN_BIT) != 0)
                sb.Append("\\Seen ");
            if ((system_flags & USER_BIT) != 0)
                sb.Append("\\* ");

            bool first = true;
            if (user_flags.IsNotNull())
            {
                foreach(var flag in user_flags.Values)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(" ");
                    sb.Append(flag);
                }
            }

            if (first && sb.Length > 0)
                sb.Length = sb.Length - 1; // smash trailing space
            return sb.ToString();
        }

        /// <summary>
        /// Returns a clone of this Flags object.
        /// </summary>
        public object Clone()
        {
            Flags f = null;
            try
            {
                f = (Flags)base.MemberwiseClone();
            }
            catch (NotSupportedException) { }
            if (user_flags.IsNotNull())
                f.user_flags = user_flags;
            return f;
        }
    }
}
