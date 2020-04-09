using System;

namespace My.Enterprise.Logging
{
    public interface ILogger
    {
        void Debug(string message);

        void Debug(string messageTemplate, params object[] propertyValues);

        void Debug(Exception exception, string message);

        void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

        void Error(string message);

        void Error(string messageTemplate, params object[] propertyValues);

        void Error(Exception exception, string message);

        void Error(Exception exception, string messageTemplate, params object[] propertyValues);

        void Fatal(string message);

        void Fatal(string messageTemplate, params object[] propertyValues);

        void Fatal(Exception exception, string message);

        void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);

        void Info(string message);

        void Info(string messageTemplate, params object[] propertyValues);

        void Info(Exception exception, string message);

        void Info(Exception exception, string messageTemplate, params object[] propertyValues);

        void Warn(string message);

        void Warn(string messageTemplate, params object[] propertyValues);

        void Warn(Exception exception, string message);

        void Warn(Exception exception, string messageTemplate, params object[] propertyValues);

    }
}
