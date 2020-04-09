using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample.Setter_Dependency_Injection
{
    public class LoggerEngine
    {
        public ILogger Logger { get; set; }

        public LoggerEngine() { }
        public LoggerEngine(ILogger logger) { Logger = logger; }

        public void Log(string message)
        {
            if (Logger != null)
            {
                Logger.OpenLog();
                Logger.Log(message);
                Logger.CloseLog();
            }
        }
    }
}
