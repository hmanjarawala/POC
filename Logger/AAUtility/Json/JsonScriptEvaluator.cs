using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace AAUtility.Json
{
    class JsonScriptEvaluator : ScriptEvaluator
    {
        protected override string CleanExpressionForObject(string expression)
        {
            return Regex.Replace(expression, @"@\.", ItemName + ".");
        }

        protected override bool IsObject(object value)
        {
            return value is JObject;
        }
    }
}
