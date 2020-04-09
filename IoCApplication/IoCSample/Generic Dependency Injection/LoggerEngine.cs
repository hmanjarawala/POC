using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Generic_Dependency_Injection
{
    public class LoggerEngine        
    {
        public void Log<TLogger>(string message) where TLogger : ILogger, new()
        {
            ILogger logger = Activator.CreateInstance<TLogger>();
            logger.OpenLog();
            logger.Log(message);
            logger.CloseLog();
        }
    }
}
