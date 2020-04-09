using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AAUtility.Json
{
    sealed class Interpreter
    {
        private readonly JsonPathResultAccumulator output;
        private readonly JsonPathScriptEvaluator eval;
        private readonly IJsonValueSystem system;

        private static readonly IJsonValueSystem defaultSystem = new BasicValueSystem();

        private static readonly char[] COLON = ":".ToCharArray();
        private static readonly char[] SEMICOLON = ";".ToCharArray();

        private delegate void WalkCallback(object member, string loc, string expr, object value, string path);

        public Interpreter(JsonPathResultAccumulator output, JsonPathScriptEvaluator eval, IJsonValueSystem system)
        {
            this.output = output;
            this.eval = eval ?? new JsonPathScriptEvaluator(NullEval);
            this.system = system ?? defaultSystem;
        }

        public void Trace(string expr, object value, string path)
        {
            if (expr == null || expr.Trim().Length == 0)
            {
                Store(path, value);
                return;
            }


            int i = expr.IndexOf(';');
            string atom = i >= 0 ? expr.Substring(0, i) : expr;
            string tail = i >= 0 ? expr.Substring(i + 1) : string.Empty;


            if (value != null && system.HasMember(value, atom))
            {
                Trace(tail, Index(value, atom), path + ";" + atom);
            }
            else if (atom == "*")
            {
                Walk(atom, tail, value, path, new WalkCallback(WalkWild));
            }
            else if (atom == "..")
            {
                Trace(tail, value, path);
                Walk(atom, tail, value, path, new WalkCallback(WalkTree));
            }
            else if (atom.Length > 2 && atom[0] == '(' && atom[atom.Length - 1] == ')') // [(exp)] 
            {
                Trace(eval(atom, value, path.Substring(path.LastIndexOf(';') + 1)) + ";" + tail, value, path);
            }
            else if (atom.Length > 3 && atom[0] == '?' && atom[1] == '(' && atom[atom.Length - 1] == ')') // [?(exp)] 
            {
                Walk(atom, tail, value, path, new WalkCallback(WalkFiltered));
            }
            else if (RegEx(@"^(-?[0-9]*):(-?[0-9]*):?([0-9]*)$").IsMatch(atom)) // [start:end:step] Phyton slice syntax 
            {
                Slice(atom, tail, value, path);
            }
            else if (atom.IndexOf(',') >= 0) // [name1,name2,...] 
            {
                foreach (string part in RegEx(@"'?,'?").Split(atom))
                        Trace(part + ";" + tail, value, path);
            }

        }

        private void Store(string path, object value)
        {
            if (path != null)
                output(value, path.Split(SEMICOLON));
        }

        private void Walk(string loc, string expr, object value, string path, WalkCallback callback)
        { 
            if (system.IsPrimitive(value)) 
                return; 


            if (system.IsArray(value)) 
            { 
                IList list = (IList)value; 
                for (int i = 0; i<list.Count; i++) 
                    callback(i, loc, expr, value, path); 
            } 
            else if (system.IsObject(value)) 
            { 
                foreach (string key in system.GetMembers(value)) 
                    callback(key, loc, expr, value, path); 
            } 
        }

        private void WalkWild(object member, string loc, string expr, object value, string path)
        { 
            Trace(member + ";" + expr, value, path); 
        }

        private void WalkTree(object member, string loc, string expr, object value, string path)
        { 
            object result = Index(value, member.ToString()); 
            if (result != null && !system.IsPrimitive(result)) 
                Trace("..;" + expr, result, path + ";" + member); 
        }

        private void WalkFiltered(object member, string loc, string expr, object value, string path)
        {
            object result = eval(RegEx(@"^\?\((.*?)\)$").Replace(loc, "$1"),
                Index(value, member.ToString()), member.ToString());

            if (ConvertToBoolean(result))
                Trace(member + ";" + expr, value, path);
        }

        private void Slice(string loc, string expr, object value, string path)
        {
            IList list = value as IList;

            if (list == null) return;

            int length = list.Count;
            string[] parts = loc.Split(COLON);
            int start = ParseInt(parts[0]);
            int end = ParseInt(parts[1], list.Count);
            int step = parts.Length > 2 ? ParseInt(parts[2], 1) : 1;
            start = start < 0 ? Math.Max(0, start + length) : Math.Min(length, start);
            end = end < 0 ? Math.Max(0, end + length) : Math.Min(length, end);
            for (int i = start; i < end; i += step)
                Trace(i + ";" + expr, value, path);
        }

        private bool ConvertToBoolean(object result)
        {
            bool retVal = false;
            try
            {
                retVal = Convert.ToBoolean(result, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { retVal = true; }
            catch (Exception) { retVal = false; }
            return retVal;
        }

        private static object NullEval(string expr, object value, string context) { return null; }

        private static int ParseInt(string s) { return ParseInt(s, 0); }

        private static int ParseInt(string s, int defaultValue)
        {
            if (s == null || s.Trim().Length == 0)
                return defaultValue;

            try
            {
                return int.Parse(s, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { return defaultValue; }
        }

        private static Regex RegEx(string pattern) { return new Regex(pattern, RegexOptions.ECMAScript); }

        private object Index(object value, string member)
        {
            return system.GetMemberValue(value, member);
        }
    }
}
