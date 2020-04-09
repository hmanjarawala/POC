using System;
using System.Collections;

namespace AAUtility.Json
{
    sealed class BasicValueSystem : IJsonValueSystem
    {
        public IEnumerable GetMembers(object value)
        {
            return ((IDictionary)value).Keys;
        }

        public object GetMemberValue(object value, string member)
        {
            if (IsPrimitive(value))
                throw new ArgumentException("value");

            IDictionary dictionary = value as IDictionary;
            if (dictionary != null)
                return dictionary[member];

            IList list = value as IList;
            int index = ParseInt(member, -1);
            if (index >= 0 && index < list.Count)
                return list[index];

            return null;
        }

        public bool HasMember(object value, string member)
        {
            if (IsPrimitive(value))
                return false;

            IDictionary dictionary = value as IDictionary;
            if (dictionary != null)
                return dictionary.Contains(member);

            IList list = value as IList;
            if (list != null)
            {
                int index = ParseInt(member, -1);
                return index >= 0 && index < list.Count;
            }

            return false;
        }

        public bool IsArray(object value)
        {
            return value is IList;
        }

        public bool IsObject(object value)
        {
            return value is IDictionary;
        }

        public bool IsPrimitive(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return Type.GetTypeCode(value.GetType()) != TypeCode.Object;
        }

        private int ParseInt(string str, int defaultValue)
        {
            int result;
            return int.TryParse(str, out result) ? result : defaultValue;
        }
    }
}
