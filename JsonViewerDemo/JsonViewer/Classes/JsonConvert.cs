using System;
using System.Web.Script.Serialization;

namespace JsonViewer.Classes
{
    public class JsonConvert
    {
        static JavaScriptSerializer json = null;

        static JavaScriptSerializer Parser { get { return json ?? new JavaScriptSerializer(); } }

        public static T Deserialize<T>(string jsonString) where T : class
        {
            return Parser.Deserialize<T>(jsonString);
        }

        public static object Deserialize(string jsonString, Type targetType)
        {
            return Parser.Deserialize(jsonString, targetType);
        }

        public static object DeserializeObject(string jsonString)
        {
            return Parser.DeserializeObject(jsonString);
        }

        public static string Serialize<T>(T jsonObject) where T : class
        {
            return Parser.Serialize(jsonObject);
        }

        public static string Serialize(object jsonObject)
        {
            return Serialize<object>(jsonObject);
        }
    }
}
