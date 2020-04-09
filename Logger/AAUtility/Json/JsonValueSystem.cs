using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;

namespace AAUtility.Json
{
    sealed class JsonValueSystem : IJsonValueSystem
    {
        public IEnumerable GetMembers(object value)
        {
            return ((JObject)value).Properties().Select(property => property.Name);
        }

        public object GetMemberValue(object value, string member)
        {
            if (IsPrimitive(value))
                throw new ArgumentException("value");

            JObject jObject = value as JObject;
            if (jObject != null)
                return jObject[member];

            JArray jArray = value as JArray;
            int index = ParseInt(member, -1);
            if (index >= 0 && index < jArray.Count)
                return jArray[index];

            return null;
        }

        public bool HasMember(object value, string member)
        {
            if (IsPrimitive(value))
                return false;

            JObject jObject = value as JObject;
            if (jObject != null)
                return jObject.Properties().Any(property => property.Name == member);

            JArray jArray = value as JArray;
            if (jArray != null)
            {
                int index = ParseInt(member, -1);
                return index >= 0 && index < jArray.Count;
            }

            return false;   
        }

        public bool IsArray(object value)
        {
            return value is JArray;
        }

        public bool IsObject(object value)
        {
            return value is JObject;
        }

        public bool IsPrimitive(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return !(IsObject(value) || IsArray(value));
        }

        private int ParseInt(string str, int defaultValue)
        {
            int result;
            return int.TryParse(str, out result) ? result : defaultValue;
        }
    }
}
