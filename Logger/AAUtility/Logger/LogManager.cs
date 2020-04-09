using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AAUtility.Logger
{
    internal class LogManager
    {
        static string configuration =
                                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                    "<configuration>" +
                                      "<configSections>" +
                                        "<section name = \"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler,Log4net\"/>" +
                                      "</configSections>" +

                                      "<log4net>" +
                                        "<appender name = \"HistoryLog\" type=\"log4net.Appender.RollingFileAppender\" >" +
                                          "<file value = \"[FILE-NAME]\\HistoryLog_" + DateTime.Now.ToString("dd-MM-yyy") + ".log\"/>" +
                                          "<encoding value=\"utf-8\" />" +
                                          "<appendToFile value = \"true\" />" +
                                          "<rollingStyle value=\"Date\" />" +
                                          "<rollingStyle value = \"Size\" />" +
                                          "<maxSizeRollBackups value=\"5\" />" +
                                          "<maximumFileSize value = \"10MB\" />" +
                                          "<staticLogFileName value=\"true\" />" +
                                          "<layout type = \"log4net.Layout.PatternLayout\" >" +
                                            "<conversionPattern value=\"%date %level - %message%n\" />" +
                                          //"<conversionPattern value=\"%date %level [%thread] %type.%method - %message%n\" />" +
                                          "</layout>" +
                                        "</appender>" +

                                        "<appender name = \"ErrorLog\" type=\"log4net.Appender.FileAppender\" >" +
                                          "<file value = \"[FILE-NAME]\\ErrorLog.csv\"/>" +
                                          //"<encoding value=\"utf-8\" />" +
                                          "<appendToFile value = \"true\" />" +
                                          "<layout type = \"log4net.Layout.PatternLayout\" >" +
                                              //"<header value=\"Error Date,Machine Name,User Name,Task Name,Error Description&#13;&#10;\" />" +
                                              "<conversionPattern value=\"%date{M/d/yyyy H:mm:ss.fff},%message%n\" />" +
                                          //"<conversionPattern value=\"%date %level [%thread] %type.%method - %message%n\" />" +
                                          "</layout>" +
                                        "</appender>" +
                                        //"<root>" +
                                        //"<level value = \"All\" />" +
                                        //"<appender-ref ref=\"HistoryLog\" />" +
                                        //"<appender-ref ref=\"ErrorLog\" />" +
                                        //"</root>" +
                                        "<logger additivity=\"false\" name=\"HistoryLog\">" +
                                            "<level value = \"All\" />" +
                                            "<appender-ref ref=\"HistoryLog\" />" +
                                        "</logger>" +
                                        "<logger additivity=\"false\" name=\"ErrorLog\">" +
                                            "<level value = \"Error\" />" +
                                            "<appender-ref ref=\"ErrorLog\" />" +
                                        "</logger>" +

                                      "</log4net>" +

                                        "<startup> " +
                                            "<supportedRuntime version = \"v4.0\" sku=\".NETFramework,Version=v4.5.2\" />" +
                                        "</startup>" +
                                    "</configuration>";
        static string _filename;
        readonly IDictionary<string, ILogger> _loggerDictonary;
        readonly XmlElement element;
        static readonly Lazy<LogManager> logManager = new Lazy<LogManager>(() => new LogManager());

        public ILogger GetLogger(string loggerName)
        {
            if (!_loggerDictonary.ContainsKey(loggerName))
                _loggerDictonary.Add(loggerName, new Log4NetLogger(element, loggerName));
            return _loggerDictonary[loggerName];
        }

        public static LogManager GetLogManager(string filename) { _filename = filename; return logManager.Value; }

        private LogManager()
        {
            _loggerDictonary = new Dictionary<string, ILogger>();
            configuration = configuration.Replace("[FILE-NAME]", _filename);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configuration);
            element = (XmlElement)doc.SelectSingleNode("//configuration/log4net");
        }

        private class Log4NetLogger: ILogger
        {
            //string filename = string.Format(@"C:\Users\himanshu.manjarawala\Documents\C#\Sample Logger\HistoryLog-{0}.log", DateTime.Now.ToString("dd-MM-yyyy"));
            
            private readonly ILog log;

            internal Log4NetLogger(XmlElement element, string loggername)
            {
                XmlConfigurator.Configure(element);
                log = log4net.LogManager.GetLogger(loggername);
            }

            public void LogInfo(string message) { log.Info(message); }

            public void LogWarn(string message) { log.Warn(message); }

            public void LogFatal(string message) { log.Fatal(message); }

            public void LogDebug(string message) { log.Debug(message); }

            public void LogError(string message) { log.Error(message); }
        }
    }
}
