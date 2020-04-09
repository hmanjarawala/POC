
namespace AAUtility.Json
{
    internal class ScriptEvaluatorFactory
    {
        internal enum ScriptEvaluatorType { Basic=0, Json};

        public static IScriptEvaluator Create(ScriptEvaluatorType scriptEvaluatorType = ScriptEvaluatorType.Json)
        {
            IScriptEvaluator scriptEvaluator = null;
            switch (scriptEvaluatorType)
            {
                case ScriptEvaluatorType.Json:
                    scriptEvaluator = new JsonScriptEvaluator();
                    break;
                case ScriptEvaluatorType.Basic:
                    scriptEvaluator = new BasicScriptEvaluator();
                    break;
            }
            return scriptEvaluator;
        }
    }
}
