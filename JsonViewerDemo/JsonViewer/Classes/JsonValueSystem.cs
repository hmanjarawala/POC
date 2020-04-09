using System;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace JsonViewer.Classes
{
    public sealed class JsonValueSystem : IJsonPathValueSystem
    {
        public bool HasMember(object value, string member)
        {
            if (IsPrimitive(value))
                return false;

            JObject jobject = value as JObject;
            if (jobject != null)
                return jobject.Properties().Any(property => property.Name == member);

            JArray jarray = value as JArray;
            int index = ParseInt(member, -1);
            return index >= 0 && index < jarray.Count;
        }

        public object GetMemberValue(object value, string member)
        {
            if (IsPrimitive(value))
                throw new ArgumentException("value");

            JObject jobject = value as JObject;
            if (jobject != null)
                return jobject[member];

            JArray jarray = value as JArray;
            int index = ParseInt(member, -1);
            if (index >= 0 && index < jarray.Count)
                return jarray[index];

            return null;
        }

        public IEnumerable GetMembers(object value)
        {
            return ((JObject)value).Properties().Select(property => property.Name);
        }

        public bool IsObject(object value)
        {
            return value is JObject;
        }

        public bool IsArray(object value)
        {
            return value is JArray;
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