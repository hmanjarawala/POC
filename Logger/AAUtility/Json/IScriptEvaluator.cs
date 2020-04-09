using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Text.RegularExpressions;

namespace AAUtility.Json
{
    interface IScriptEvaluator
    {
        object Eval(string expression, object value, string context);
    }

    abstract class ScriptEvaluator : IScriptEvaluator
    {
        private Lazy<ScriptEngine> engine = new Lazy<ScriptEngine>(() => Python.CreateEngine());
        private ScriptEngine Engine => engine.Value;

        protected const string ItemName = "_ScriptEvaluator_Item";

        protected abstract string CleanExpressionForObject(string expression);

        protected abstract bool IsObject(object value);

        public object Eval(string expression, object value, string context)
        {
            var cleanScript = CleanupExpression(expression, value);
            var scope = Engine.CreateScope();
            scope.SetVariable(ItemName, value);
            return Engine.Execute(cleanScript, scope);
        }

        private string CleanupExpression(string expression, object value)
        {
            if (IsObject(value))
                return CleanExpressionForObject(expression);
            return CleanupExpression(expression);
        }

        private string CleanupExpression(string expression)
        {
            return Regex.Replace(expression, @"@", ItemName);
        }
    }
}
