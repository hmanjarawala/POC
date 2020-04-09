using My.Enterprise.Logging;
using System;
using log4net;
using System.Linq;
using log4net.Config;
using System.IO;

namespace My.Enterprise.LoggingApp
{
    class Log4NetLogger : Logger
    {
        private readonly ILog _log;

        public Log4NetLogger(string loggerName)
        {
            var logname = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), loggerName);
            GlobalContext.Properties["LogFileName"] = logname;
            XmlConfigurator.Configure();
            _log = LogManager.GetLogger(typeof(Log4NetLogger));
        }

        public override void Debug(string message)
        {
            WriteInternalLog();
            _log.Debug(message);
        }

        public override void Debug(string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            _log.DebugFormat(messageTemplate, propertyValues);
        }

        public override void Debug(Exception exception, string message)
        {
            WriteInternalLog();
            _log.Debug(message, exception);
        }

        public override void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            messageTemplate = string.Concat(messageTemplate, Environment.NewLine, exception);
            _log.DebugFormat(messageTemplate, propertyValues);
        }

        public override void Dispose(bool dispossing)
        {
            LogManager.GetRepository().Threshold = LogManager.GetRepository().LevelMap["OFF"];
        }

        public override void Error(string message)
        {
            WriteInternalLog();
            _log.Error(message);
        }

        public override void Error(string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            _log.ErrorFormat(messageTemplate, propertyValues);
        }

        public override void Error(Exception exception, string message)
        {
            WriteInternalLog();
            _log.Error(message, exception);
        }

        public override void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            messageTemplate = string.Concat(messageTemplate, Environment.NewLine, exception);
            _log.ErrorFormat(messageTemplate, propertyValues);
        }

        public override void Fatal(string message)
        {
            WriteInternalLog();
            _log.Fatal(message);
        }

        public override void Fatal(string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            _log.FatalFormat(messageTemplate, propertyValues);
        }

        public override void Fatal(Exception exception, string message)
        {
            WriteInternalLog();
            _log.Fatal(message, exception);
        }

        public override void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            messageTemplate = string.Concat(messageTemplate, Environment.NewLine, exception);
            _log.FatalFormat(messageTemplate, propertyValues);
        }

        public override void Info(string message)
        {
            WriteInternalLog();
            _log.Info(message);
        }

        public override void Info(string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            _log.InfoFormat(messageTemplate, propertyValues);
        }

        public override void Info(Exception exception, string message)
        {
            WriteInternalLog();
            _log.Info(message, exception);
        }

        public override void Info(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            messageTemplate = string.Concat(messageTemplate, Environment.NewLine, exception);
            _log.InfoFormat(messageTemplate, propertyValues);
        }

        public override void Warn(string message)
        {
            WriteInternalLog();
            _log.Warn(message);
        }

        public override void Warn(string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            _log.WarnFormat(messageTemplate, propertyValues);
        }

        public override void Warn(Exception exception, string message)
        {
            WriteInternalLog();
            _log.Warn(message, exception);
        }

        public override void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            WriteInternalLog();
            messageTemplate = string.Concat(messageTemplate, Environment.NewLine, exception);
            _log.WarnFormat(messageTemplate, propertyValues);
        }

        private void WriteInternalLog()
        {
            Console.WriteLine("Logger: {0}", (_log as log4net.Core.LogImpl).Logger.Name);
            Console.WriteLine("Appenders: {0}", string.Join(", ", (_log as log4net.Core.LogImpl).Logger.Repository.GetAppenders().ToList().Select(appendr => appendr.Name)));
        }
    }
}
