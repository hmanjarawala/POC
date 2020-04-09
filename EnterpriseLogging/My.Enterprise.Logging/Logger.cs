using System;

namespace My.Enterprise.Logging
{
    public abstract class Logger : ILogger, IDisposable
    {
        public abstract void Debug(string message);

        public abstract void Debug(Exception exception, string message);

        public abstract void Debug(string messageTemplate, params object[] propertyValues);

        public abstract void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Dispose(bool dispossing);

        public abstract void Error(string message);

        public abstract void Error(Exception exception, string message);

        public abstract void Error(string messageTemplate, params object[] propertyValues);

        public abstract void Error(Exception exception, string messageTemplate, params object[] propertyValues);

        public abstract void Fatal(string message);

        public abstract void Fatal(Exception exception, string message);

        public abstract void Fatal(string messageTemplate, params object[] propertyValues);

        public abstract void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);

        public abstract void Info(string message);

        public abstract void Info(Exception exception, string message);

        public abstract void Info(string messageTemplate, params object[] propertyValues);

        public abstract void Info(Exception exception, string messageTemplate, params object[] propertyValues);

        public abstract void Warn(string message);

        public abstract void Warn(Exception exception, string message);

        public abstract void Warn(string messageTemplate, params object[] propertyValues);

        public abstract void Warn(Exception exception, string messageTemplate, params object[] propertyValues);

        ~Logger()
        {
            Dispose(false);
        }
    }
}
