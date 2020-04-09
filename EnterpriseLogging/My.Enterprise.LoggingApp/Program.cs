using My.Enterprise.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Enterprise.LoggingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new ConsoleLogger(new Log4NetLogger("Sync"));

            Log.Write(LogType.DEBUG, "1. Log");
            Log.Write(LogType.DEBUG, "2. Log {0} {1}", "Himanshu", 175);

            try
            {
                throw new ArgumentException("Invalid Argument");
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "3. Exception Log");
                Log.Write(LogType.ERROR, ex, "4. Exception Log {0} {1}", "Himanshu", 175);
            }

            Log.Logger = new Log4NetLogger("Async");

            Log.Write(LogType.DEBUG, "1. Log");
            Log.Write(LogType.DEBUG, "2. Log {0} {1}", "Himanshu", 175);

            try
            {
                throw new ArgumentException("Invalid Argument");
            }
            catch (Exception ex)
            {
                Log.Write(LogType.ERROR, ex, "3. Exception Log");
                Log.Write(LogType.ERROR, ex, "4. Exception Log {0} {1}", "Himanshu", 175);
            }

            Console.Read();
        }
    }

    class ConsoleLogger : LoggerDecorator
    {
        public ConsoleLogger(ILogger logger) : base(logger) { }

        public override void Debug(string message)
        {
            base.Debug(message);
            Write(LogType.DEBUG, message);
        }

        public override void Debug(string messageTemplate, params object[] propertyValues)
        {
            base.Debug(messageTemplate, propertyValues);
            Write(LogType.DEBUG, messageTemplate, propertyValues);
        }

        public override void Debug(Exception exception, string message)
        {
            base.Debug(exception, message);
            Write(LogType.DEBUG, exception, message);
        }

        public override void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            base.Debug(exception, messageTemplate, propertyValues);
            Write(LogType.DEBUG, exception, messageTemplate, propertyValues);
        }

        public override void Dispose(bool dispossing)
        {
            base.Dispose();
        }

        public override void Error(string message)
        {
            base.Error(message);
            Write(LogType.ERROR, message);
        }

        public override void Error(string messageTemplate, params object[] propertyValues)
        {
            base.Error(messageTemplate, propertyValues);
            Write(LogType.ERROR, messageTemplate, propertyValues);
        }

        public override void Error(Exception exception, string message)
        {
            base.Error(exception, message);
            Write(LogType.ERROR, exception, message);
        }

        public override void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            base.Error(exception, messageTemplate, propertyValues);
            Write(LogType.ERROR, exception, messageTemplate, propertyValues);
        }

        public override void Fatal(string message)
        {
            base.Fatal(message);
            Write(LogType.FATAL, message);
        }

        public override void Fatal(Exception exception, string message)
        {
            base.Fatal(exception, message);
            Write(LogType.FATAL, exception, message);
        }

        public override void Fatal(string messageTemplate, params object[] propertyValues)
        {
            base.Fatal(messageTemplate, propertyValues);
            Write(LogType.FATAL, messageTemplate, propertyValues);
        }

        public override void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            base.Fatal(exception, messageTemplate, propertyValues);
            Write(LogType.FATAL, exception, messageTemplate, propertyValues);
        }

        public override void Info(string message)
        {
            base.Info(message);
            Write(LogType.INFO, message);
        }

        public override void Info(Exception exception, string message)
        {
            base.Info(exception, message);
            Write(LogType.INFO, exception, message);
        }

        public override void Info(string messageTemplate, params object[] propertyValues)
        {
            base.Info(messageTemplate, propertyValues);
            Write(LogType.INFO, messageTemplate, propertyValues);
        }

        public override void Info(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            base.Info(exception, messageTemplate, propertyValues);
            Write(LogType.INFO, exception, messageTemplate, propertyValues);
        }

        public override void Warn(string message)
        {
            base.Warn(message);
            Write(LogType.WARN, message);
        }

        public override void Warn(Exception exception, string message)
        {
            base.Warn(exception, message);
            Write(LogType.WARN, exception, message);
        }

        public override void Warn(string messageTemplate, params object[] propertyValues)
        {
            base.Warn(messageTemplate, propertyValues);
            Write(LogType.WARN, messageTemplate, propertyValues);
        }

        public override void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            base.Warn(exception, messageTemplate, propertyValues);
            Write(LogType.WARN, exception, messageTemplate, propertyValues);
        }

        private void Write(LogType logType, string message)
        {
            var msg = string.Format("{0} {1}: {2}", DateTime.Now, logType, message);
            Console.WriteLine(msg);
        }

        private void Write(LogType logType, Exception ex, string message)
        {
            var msg = string.Concat(message, Environment.NewLine, ex);
            Write(logType, msg);
        }

        private void Write(LogType logType, string messageTemplate, params object[] propertyValues)
        {
            var msg = string.Format(messageTemplate, propertyValues);
            Write(logType, msg);
        }

        private void Write(LogType logType, Exception ex, string messageTemplate, params object[] propertyValues)
        {
            var message = string.Format(messageTemplate, propertyValues);
            Write(logType, ex, message);
        }
    }
}
