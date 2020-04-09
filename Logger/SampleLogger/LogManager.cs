using log4net;
using log4net.Config;
using System;
using System.Linq;
using System.Xml;

namespace SampleLogger
{
    public class LogManager
    {
        static string _filename;
        static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(() => new Log4NetLogger(_filename));

        public static ILogger GetLogger(string filename) { _filename = filename; return _logger.Value; }

        private class Log4NetLogger: ILogger
        {
            //string filename = string.Format(@"C:\Users\himanshu.manjarawala\Documents\C#\Sample Logger\HistoryLog-{0}.log", DateTime.Now.ToString("dd-MM-yyyy"));
            string configuration = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                                    "<configuration>" +
                                      "<configSections>" +
                                        "<section name = \"log4net\" type=\"log4net.Config.Log4NetConfigurationSectionHandler,Log4net\"/>" +
                                      "</configSections>" +

                                      "<log4net>" +
                                        "<appender name = \"TestAppender\" type=\"log4net.Appender.RollingFileAppender\" >" +
                                          "<file value = \"[FILE-NAME]\" />" +
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
                                        "<root>" +
                                          "<level value = \"All\" />" +
                                          "<appender-ref ref=\"TestAppender\" />" +
                                        "</root>" +
                                      "</log4net>" +

                                        "<startup> " +
                                            "<supportedRuntime version = \"v4.0\" sku=\".NETFramework,Version=v4.5.2\" />" +
                                        "</startup>" +
                                    "</configuration>";
            private readonly ILog log;

            internal Log4NetLogger(string filename)
            {
                configuration = configuration.Replace("[FILE-NAME]", filename);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(configuration);
                XmlElement[] element = doc.GetElementsByTagName("log4net").Cast<XmlElement>().ToArray();
                XmlConfigurator.Configure(element[0]);
                log = log4net.LogManager.GetLogger(this.GetType());
            }

            public void LogInfo(string message) { log.Info(message); }

            public void LogWarn(string message) { log.Warn(message); }

            public void LogFatal(string message) { log.Fatal(message); }

            public void LogDebug(string message) { log.Debug(message); }

            public void LogError(string message) { log.Error(message); }
        }
    }

    public interface ILogger
    {
        void LogInfo(string message);

        void LogWarn(string message);

        void LogFatal(string message);

        void LogDebug(string message);

        void LogError(string message);
    }
}
