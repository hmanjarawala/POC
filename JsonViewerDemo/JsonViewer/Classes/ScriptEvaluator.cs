using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace JsonViewer.Classes
{
    public interface IScriptEvaluator
    {
        object EvaluateScript(string expression, object value, string context);
    }

    public class ScriptEvaluatorFactory
    {
        public enum ScriptEvaluatorTypes { Basic = 0, Json };

        public static IScriptEvaluator Create(ScriptEvaluatorTypes scriptEvaluatorType = ScriptEvaluatorTypes.Basic)
        {
            IScriptEvaluator eval = null;
            switch (scriptEvaluatorType)
            {
                case ScriptEvaluatorTypes.Json:
                    eval = new JsonScriptEvaluator();
                    break;
                case ScriptEvaluatorTypes.Basic:
                    eval = new BasicScriptEvaluator();
                    break;
            }
            return eval;
        }
    }

    public abstract class ScriptEvaluator : IScriptEvaluator
    {
        private Lazy<ScriptEngine> engine = new Lazy<ScriptEngine>(() => Python.CreateEngine());
        private ScriptEngine Engine { get { return engine.Value; } }

        protected const string ItemName = "_ScriptEvaluator_item";        

        public object EvaluateScript(string expression, object value, string context)
        {
            var cleanScript = CleanupExpression(expression, value);
            var scope = Engine.CreateScope();
            scope.SetVariable(ItemName, value);
            return Engine.Execute<object>(cleanScript, scope);
        }

        protected abstract string CleanupExpressionForObject(string expression);

        protected abstract bool IsObject(object value);

        private string CleanupExpression(string expression, object value)
        {
            if (IsObject(value))
                return CleanupExpressionForObject(expression);

            return CleanupExpression(expression);
        }

        private string CleanupExpression(string expression)
        {
            return Regex.Replace(expression, @"@", ItemName);
        }
    }

    class BasicScriptEvaluator : ScriptEvaluator
    {
        protected override string CleanupExpressionForObject(string expression)
        {
            expression = Regex.Replace(expression, @"(@\.(.*?)\w+)", delegate(Match match)
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

    class JsonScriptEvaluator : ScriptEvaluator
    {
        protected override string CleanupExpressionForObject(string expression)
        {
            return Regex.Replace(expression, @"@\.", ItemName + ".");
        }

        protected override bool IsObject(object value)
        {
            return value is JObject;
        }
    }
}
