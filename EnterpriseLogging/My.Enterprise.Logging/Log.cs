using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace My.Enterprise.Logging
{
    public static class Log
    {
        static Logger _logger;
        static ReaderWriterLockSlim _rwls = new ReaderWriterLockSlim();

        public static Logger Logger
        {
            get
            {
                try
                {
                    _rwls.EnterReadLock();
                    return _logger;
                }
                finally
                {
                    _rwls.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _rwls.EnterWriteLock();
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));
                    _logger = value;
                }
                finally
                {
                    _rwls.ExitWriteLock();
                }
            }
        }

        public static void Write(LogType logType, string message)
        {
            message = string.Concat(GetMessageInfo(logType), message);
            switch (logType)
            {
                case LogType.DEBUG:
                    _logger.Debug(message);
                    break;
                case LogType.ERROR:
                    _logger.Error(message);
                    break;
                case LogType.FATAL:
                    _logger.Fatal(message);
                    break;
                case LogType.INFO:
                    _logger.Info(message);
                    break;
                case LogType.WARN:
                    _logger.Warn(message);
                    break;
            }
        }

        public static void Write(LogType logType, Exception exception, string message)
        {
            message = string.Concat(GetMessageInfo(logType), message);
            switch (logType)
            {
                case LogType.DEBUG:
                    _logger.Debug(exception, message);
                    break;
                case LogType.ERROR:
                    _logger.Error(exception, message);
                    break;
                case LogType.FATAL:
                    _logger.Fatal(exception, message);
                    break;
                case LogType.INFO:
                    _logger.Info(exception, message);
                    break;
                case LogType.WARN:
                    _logger.Warn(exception, message);
                    break;
            }
        }

        public static void Write(LogType logType, string messageTemplate, params object[] propertyValues)
        {
            messageTemplate = string.Concat(GetMessageInfo(logType), messageTemplate);
            switch (logType)
            {
                case LogType.DEBUG:
                    _logger.Debug(messageTemplate, propertyValues);
                    break;
                case LogType.ERROR:
                    _logger.Error(messageTemplate, propertyValues);
                    break;
                case LogType.FATAL:
                    _logger.Fatal(messageTemplate, propertyValues);
                    break;
                case LogType.INFO:
                    _logger.Info(messageTemplate, propertyValues);
                    break;
                case LogType.WARN:
                    _logger.Warn(messageTemplate, propertyValues);
                    break;
            }
        }

        public static void Write(LogType logType, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            messageTemplate = string.Concat(GetMessageInfo(logType), messageTemplate);
            switch (logType)
            {
                case LogType.DEBUG:
                    _logger.Debug(exception, messageTemplate, propertyValues);
                    break;
                case LogType.ERROR:
                    _logger.Error(exception, messageTemplate, propertyValues);
                    break;
                case LogType.FATAL:
                    _logger.Fatal(exception, messageTemplate, propertyValues);
                    break;
                case LogType.INFO:
                    _logger.Info(exception, messageTemplate, propertyValues);
                    break;
                case LogType.WARN:
                    _logger.Warn(exception, messageTemplate, propertyValues);
                    break;
            }
        }

        private static string GetMessageInfo(LogType logType, [CallerMemberName] string memberName="", [CallerFilePath] string sourceFilePath="", [CallerLineNumber]int sourceLineNumber = 0)
        {
            return string.Format("{0} entry from method \'{1}\' in file {2} at line {3}.\r\n", logType, memberName, sourceFilePath, sourceLineNumber);
        }
    }
}
