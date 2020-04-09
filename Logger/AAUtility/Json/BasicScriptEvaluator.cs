using System.Collections;
using System.Text.RegularExpressions;

namespace AAUtility.Json
{
    class BasicScriptEvaluator : ScriptEvaluator
    {
        protected override string CleanExpressionForObject(string expression)
        {
            expression = Regex.Replace(expression, @"(@\.(.*?)\w+)", delegate (Match match)
            {
                string v = match.ToString();
                return v.StartsWith("@.") ? string.Format("@.[\"{0}\"]", v.Substring(2)) : v;
            });
            return Regex.Replace(expression, @"@\.", ItemName);

        }

        protected override bool IsObject(object value)
        {
            return value is IDictionary;
        }
    }
}
