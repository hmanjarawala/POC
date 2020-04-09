using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Interface_Dependency_Injection
{
    public class LoggerEngine
    {
        private ILogger logger;

        public void Log(ILoggerInject loggerInject, string message)
        {
            logger = loggerInject.Construct();
            logger.OpenLog();
            logger.Log(message);
            logger.CloseLog();
        }
    }
}
