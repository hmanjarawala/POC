using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Constructor_Dependency_Injection
{
    public class LoggingEngine
    {
        private ILogger logger = null;

        public LoggingEngine(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(string message)
        {
            logger.OpenLog();
            logger.Log(message);
            logger.CloseLog();
        }
    }
}
