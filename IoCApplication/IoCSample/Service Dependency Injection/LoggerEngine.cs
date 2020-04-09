using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Service_Dependency_Injection
{
    public class LoggerEngine
    {
        private ILogger logger;
        public void Log(string message)
        {
            logger = LoggerService.getLoggerService();
            logger.OpenLog();
            logger.Log(message);
            logger.CloseLog();
        }
    }
}
