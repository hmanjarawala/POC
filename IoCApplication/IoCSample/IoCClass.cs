using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample
{
    public class IoCClass
    {
        ILogger logger = null;

        public string FileDirectory { get; set; }

        public void WriteLogUsingConstructorInjection(string message)
        {
            logger = new FileLogger(FileDirectory);

            Constructor_Dependency_Injection.LoggingEngine cEngine = 
                new Constructor_Dependency_Injection.LoggingEngine(logger);
            cEngine.Log(message);
            cEngine = null;

            logger = new ConsoleLogger();

            cEngine = new Constructor_Dependency_Injection.LoggingEngine(logger);
            cEngine.Log(message);
            cEngine = null;

            logger = new XmlLogger(FileDirectory);

            cEngine = new Constructor_Dependency_Injection.LoggingEngine(logger);
            cEngine.Log(message);
            cEngine = null;
        }

        public void WriteLogUsingSetterInjection(string message)
        {
            logger = new FileLogger(FileDirectory);

            Setter_Dependency_Injection.LoggerEngine sEngine = 
                new Setter_Dependency_Injection.LoggerEngine();

            sEngine.Logger = logger;
            sEngine.Log(message);

            logger = new ConsoleLogger();

            sEngine.Logger = logger;
            sEngine.Log(message);

            logger = new XmlLogger(FileDirectory);

            sEngine.Logger = logger;
            sEngine.Log(message);

            sEngine = null;
        }

        public void WriteLogUsingInterfaceInjection(string message)
        {
            Interface_Dependency_Injection.LoggerEngine iEngine = 
                new Interface_Dependency_Injection.LoggerEngine();

            Interface_Dependency_Injection.ILoggerInject loggerInject = 
                new Interface_Dependency_Injection.FileLoggerInject() { FileDirectory = FileDirectory };

            iEngine.Log(loggerInject, message);
            loggerInject = null;

            loggerInject = new Interface_Dependency_Injection.ConsoleLoggerInject();

            iEngine.Log(loggerInject, message);
            loggerInject = null;

            loggerInject = new Interface_Dependency_Injection.XmlLoggerInject() { FileDirectory = FileDirectory };

            iEngine.Log(loggerInject, message);
            loggerInject = null;

            iEngine = null;
        }

        public void WriteLogUsingServiceInjection(string message)
        {
            Service_Dependency_Injection.LoggerEngine slEngine = 
                new Service_Dependency_Injection.LoggerEngine();

            Service_Dependency_Injection.LoggerService.Logger = new FileLogger(FileDirectory);
            slEngine.Log(message);

            Service_Dependency_Injection.LoggerService.Logger = new ConsoleLogger();
            slEngine.Log(message);

            Service_Dependency_Injection.LoggerService.Logger = new XmlLogger(FileDirectory);
            slEngine.Log(message);

            slEngine = null;
        }

        public void WriteLogUsingGenericTypeInjection(string message)
        {
            Generic_Dependency_Injection.LoggerEngine gEngine = 
                new Generic_Dependency_Injection.LoggerEngine();

            gEngine.Log<FileLogger>(message);

            gEngine.Log<ConsoleLogger>(message);

            gEngine.Log<XmlLogger>(message);

            gEngine = null;
        }
    }
}
