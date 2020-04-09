using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AAUtility
{
    partial class UtilityFasad
    {
        public int GetMatchCount(string input, string expression, int startAt, bool isMultiline, bool matchCase)
        {
            var regEx = new Regex(expression, getRegexOptions(isMultiline, matchCase));
            return Convert.ToInt32(regEx.Matches(input, startAt-1)?.Count);
        }

        public bool IsStringMatches(string input, string expression, int startAt, bool isMultiline, bool matchCase)
        {
            var regEx = new Regex(expression, getRegexOptions(isMultiline, matchCase));
            return regEx.IsMatch(input, startAt-1);
        }

        public string[] GetMatchingStrings(string input, string expression, int startAt, bool isMultiline,
            bool matchCase)
        {
            var regEx = new Regex(expression, getRegexOptions(isMultiline, matchCase));
            if (regEx.IsMatch(input, startAt-1))
            {
                var collection = regEx.Matches(input, startAt - 1);
                return Array.ConvertAll(collection.Cast<Match>().ToArray(), (c) => c.Groups[0].Value);
            }
            return new string[0];
        }

        public string GetMatchingGroup(string input, string expression, int startAt, int occurance, bool isMultiline, 
            bool matchCase)
        {
            var regEx = new Regex(expression, getRegexOptions(isMultiline, matchCase));
            if(regEx.IsMatch(input, startAt-1))
            {
                var collection = regEx.Matches(input, startAt-1);
                if (collection.Count >= occurance) return collection[occurance-1].Groups[0].ToString();
            }
            return string.Empty;
        }

        public string GetMatchingString(string input, string expression, int startAt, int occurance, bool isMultiline,
            bool matchCase)
        {
            var regEx = new Regex(expression, getRegexOptions(isMultiline, matchCase));
            if (regEx.IsMatch(input, startAt-1))
            {
                var collection = regEx.Matches(input, startAt - 1);
                if (collection.Count >= occurance) return collection[occurance - 1].Groups[0].Value;
            }
            return string.Empty;
        }

        public string ReplaceMatchingString(string input, string expression, string replacement, int startAt, int occurance, bool isMultiline,
            bool matchCase)
        {
            var regEx = new Regex(expression, getRegexOptions(isMultiline, matchCase));
            if (regEx.IsMatch(input, startAt-1))
            {
                return regEx.Replace(input, replacement, occurance, startAt-1);
            }
            return input;
        }

        private RegexOptions getRegexOptions(bool isMultiline, bool matchCase)
        {
            RegexOptions options;

            options = (isMultiline) ? RegexOptions.Multiline : RegexOptions.Singleline;

            options = (!matchCase) ? options | RegexOptions.IgnoreCase : options;

            return options;
        }
    }
}
