using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AAUtility.Json
{
    delegate object JsonPathScriptEvaluator(string script, object value, string context);
    delegate void JsonPathResultAccumulator(object value, string[] indicies);

    sealed class JsonPathContext
    {
        static readonly Lazy<JsonPathContext> _context = new Lazy<JsonPathContext>(()=>new JsonPathContext());

        private JsonPathContext() { }

        public static JsonPathContext DefaultContext => _context.Value;

        public JsonPathScriptEvaluator ScriptEvaluator { get; set; }

        public IJsonValueSystem ValueSystem { get; set; }

        public void SelectTo(object obj, string expr, JsonPathResultAccumulator output)
        { 
            if (obj == null) 
                throw new ArgumentNullException("obj"); 
        
        
            if (output == null) 
                throw new ArgumentNullException("output"); 
        
        
            Interpreter i = new Interpreter(output, ScriptEvaluator, ValueSystem); 
        
        
            expr = Normalize(expr); 
        
        
            if (expr.Length >= 1 && expr[0] == '$') // ^\$:? 
                expr = expr.Substring(expr.Length >= 2 && expr[1] == ';' ? 2 : 1); 
        
        
            i.Trace(expr, obj, "$"); 
        }

        public IList SelectNodesTo(object obj, string expr, IList output)
        { 
            ListAccumulator accumulator = new ListAccumulator(output??new ArrayList()); 
            SelectTo(obj, expr, new JsonPathResultAccumulator(accumulator.Put)); 
            return output; 
        }

        public JsonPathNode[] SelectNodes(object obj, string expr)
        { 
            ArrayList list = new ArrayList(); 
            SelectNodesTo(obj, expr, list); 
            return (JsonPathNode[]) list.ToArray(typeof(JsonPathNode)); 
        }


        public static string AsBracketNotation(string[] indicies)
        {
            if (indicies == null)
                throw new ArgumentNullException("indicies");

            StringBuilder builder = new StringBuilder();
            foreach(var index in indicies)
            {
                if (builder.Length == 0)
                    builder.Append("$");
                else
                {
                    builder.Append("[");
                    if (RegEx(@"^[0-9*]+$").IsMatch(index))
                        builder.Append(index);
                    else
                        builder.Append("\"").Append(index).Append("\"");
                    builder.Append("]");
                }
            }
            return builder.ToString();
        }

        private static string Normalize(string expr)
        { 
            NormalizationSwap swap = new NormalizationSwap(); 
            //Resolved Issue: Member names containing dot fails. 
            //expr = RegExp(@"[\['](\??\(.*?\))[\]']").Replace(expr, new MatchEvaluator(swap.Capture)); 
            expr = RegEx(@"[\['](\??\(.*?\))[\]']|\['(.*?)'\]").Replace(expr, new MatchEvaluator(swap.Capture)); 
            expr = RegEx(@"'?\.'?|\['?").Replace(expr, ";"); 
            expr = RegEx(@";;;|;;").Replace(expr, ";..;"); 
            expr = RegEx(@";$|'?\]|'$").Replace(expr, string.Empty); 
            expr = RegEx(@"#([0-9]+)").Replace(expr, new MatchEvaluator(swap.Yield)); 
            return expr; 
        }

        private static Regex RegEx(string pattern) { return new Regex(pattern, RegexOptions.ECMAScript); }

        private sealed class NormalizationSwap
        {
            private readonly ArrayList subx = new ArrayList(4);

            public string Capture(Match match)
            {
                int index = subx.Add(match.Groups[1].Value);
                return string.Concat("[#", index.ToString(CultureInfo.InvariantCulture), "]");
            }

            public string Yield(Match match)
            {
                int index = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                return (string)subx[index];
            }
        }

        private sealed class ListAccumulator
        {
            private readonly IList list;

            public ListAccumulator(IList list) { this.list = list; }

            public void Put(object value, string[] indicies)
            {
                list.Add(new JsonPathNode(value, JsonPathContext.AsBracketNotation(indicies)));
            }
        }
    }
}
