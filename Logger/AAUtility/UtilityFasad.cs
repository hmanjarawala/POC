using System;
using System.Xml;
using AAUtility.Logger;
using AAUtility.Json;
using Newtonsoft.Json;
using AAUtility.Extensions;

namespace AAUtility
{
    public partial class UtilityFasad
    {
        string configurationFilePath;
        string logFilePath;
        bool status = false;
        ILogger _logger, _errLogger;
        XmlDocument _doc;
        JsonPathContext context;

        public UtilityFasad()
        {
            context = JsonPathContext.DefaultContext;
            context.ScriptEvaluator = new JsonPathScriptEvaluator(ScriptEvaluatorFactory.Create(ScriptEvaluatorFactory.ScriptEvaluatorType.Json).Eval);
            context.ValueSystem = new JsonValueSystem();
        }

        public void Initialize(string configurationFilePath, string logFilePath)
        {
            configurationFilePath.ThrowIfNullOrEmpty(nameof(configurationFilePath));
            logFilePath.ThrowIfNullOrEmpty(nameof(logFilePath));
            this.configurationFilePath = configurationFilePath;
            this.logFilePath = logFilePath;
            _doc = new XmlDocument();
            _doc.Load(this.configurationFilePath);
            status = GetLoggingStatus();
            var manager = LogManager.GetLogManager(this.logFilePath);
            _logger = manager.GetLogger("HistoryLog");
            _errLogger = manager.GetLogger("ErrorLog");
        }

        public void UpdateActivityLog(string message, string taskname)
        {
            if (status)
                _logger.LogInfo(string.Format("{0} : {1}", taskname, message));
        }

        public void LogError(string message, string taskname)
        {
            if (status)
            {
                _logger.LogError(string.Format("{0} : {1}", taskname, message));
                _errLogger.LogError(string.Format("{0},{1},{2},{3}", Environment.MachineName, Environment.UserName,
                    taskname, message));
            }
        }

        public string GetConfigurationValue(string name)
        {
            XmlNode node = _doc.SelectSingleNode(string.Concat("//PARAM[@NAME='", name, "']"));

            if (node != null) return node.Attributes["VALUE"].Value;
            return string.Empty;
        }

        /// <seealso cref="https://github.com/json-path/JsonPath"/>
        public string[] GetMultipleNodes(string jsonString, string expression)
        {
            jsonString.ThrowIfNullOrEmpty(nameof(jsonString));
            expression.ThrowIfNullOrEmpty(nameof(expression));
            var json = JsonConvert.DeserializeObject(jsonString);
            var nodes = context.SelectNodes(json, expression);
            return Array.ConvertAll(JsonPathNode.ValuesFrom(nodes), (p) => p.ToString());
        }

        public string GetSingleNode(string jsonString, string expression)
        {
            var nodes = GetMultipleNodes(jsonString, expression);
            return (nodes != null && nodes.Length > 0) ? nodes[0] : string.Empty;
        }

        public string GetLongestMatchedString(string string1, string string2)
        {
            string1.ThrowIfNullOrEmpty(nameof(string1));
            string2.ThrowIfNullOrEmpty(nameof(string2));
            int len1=0, len2=0, lcs_temp;
            string match, answer;
            len1 = string1.Length;
            len2 = string2.Length;

            answer = string.Empty;

            for (int i = 0; i < len1; i++)
            {
                for (int j = 0; j < len2; j++)
                {
                    lcs_temp = 0;
                    match = string.Empty;

                    while((i+lcs_temp<len1) && (j+lcs_temp<len2) && string1[i + lcs_temp] == string2[j + lcs_temp])
                    {
                        match += string2[j + lcs_temp];
                        lcs_temp++;
                    }

                    if (match.Length > answer.Length) answer = match;
                }
            }

            return answer;
        }

        private bool GetLoggingStatus()
        {
            var status = GetConfigurationValue("ENABLE_LOGGING");
            return Boolean.Parse(status);
        }
    }
}
