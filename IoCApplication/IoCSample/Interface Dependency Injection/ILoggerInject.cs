using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Interface_Dependency_Injection
{
    public interface ILoggerInject
    {
        ILogger Construct();
    }

    public class ConsoleLoggerInject : ILoggerInject
    {
        public ILogger Construct()
        {
            return new ConsoleLogger();
        }
    }

    public class FileLoggerInject : ILoggerInject
    {
        public string FileDirectory { get; set; }
        public ILogger Construct()
        {
            return new FileLogger(FileDirectory);
        }
    }

    public class XmlLoggerInject : ILoggerInject
    {
        public string FileDirectory { get; set; }
        public ILogger Construct()
        {
            return new XmlLogger(FileDirectory);
        }
    }
}
