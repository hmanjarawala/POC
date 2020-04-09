using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Service_Dependency_Injection
{
    class LoggerService
    {
        public static ILogger Logger { private get; set; }

        public static ILogger getLoggerService()
        {
            if (Logger == null)
                Logger = new ConsoleLogger();
            return Logger;
        }
    }
}
