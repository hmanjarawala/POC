using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Enterprise.Logging
{
    public class LoggerDecorator : Logger
    {
        private ILogger _logger;

        public LoggerDecorator(ILogger logger)
        {
            _logger = logger;
        }

        public override void Debug(string message)
        {
            _logger.Debug(message);
        }

        public override void Debug(string messageTemplate, params object[] propertyValues)
        {
            _logger.Debug(messageTemplate, propertyValues);
        }

        public override void Debug(Exception exception, string message)
        {
            _logger.Debug(exception, message);
        }

        public override void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Debug(exception, messageTemplate, propertyValues);
        }

        public override void Dispose(bool dispossing)
        {
            (_logger as Logger).Dispose();
        }

        public override void Error(string message)
        {
            _logger.Error(message);
        }

        public override void Error(string messageTemplate, params object[] propertyValues)
        {
            _logger.Error(messageTemplate, propertyValues);
        }

        public override void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }

        public override void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Error(exception, messageTemplate, propertyValues);
        }

        public override void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public override void Fatal(Exception exception, string message)
        {
            _logger.Fatal(exception, message);
        }

        public override void Fatal(string messageTemplate, params object[] propertyValues)
        {
            _logger.Fatal(messageTemplate, propertyValues);
        }

        public override void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Fatal(exception, messageTemplate, propertyValues);
        }

        public override void Info(string message)
        {
            _logger.Info(message);
        }

        public override void Info(Exception exception, string message)
        {
            _logger.Info(exception, message);
        }

        public override void Info(string messageTemplate, params object[] propertyValues)
        {
            _logger.Info(messageTemplate, propertyValues);
        }

        public override void Info(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Info(exception, messageTemplate, propertyValues);
        }

        public override void Warn(string message)
        {
            _logger.Warn(message);
        }

        public override void Warn(Exception exception, string message)
        {
            _logger.Warn(exception, message);
        }

        public override void Warn(string messageTemplate, params object[] propertyValues)
        {
            _logger.Warn(messageTemplate, propertyValues);
        }

        public override void Warn(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            _logger.Warn(exception, messageTemplate, propertyValues);
        }
    }
}
