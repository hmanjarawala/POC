using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample
{
    public class ConsoleLogger : ILogger
    {
        public void OpenLog()
        {
        }

        public void CloseLog()
        {
        }

        public void Log(string message)
        {
            Console.WriteLine("-----------------------------------------------------------------------------------------");
            Console.WriteLine(string.Format("Log Message : {0}", message));
            Console.WriteLine(string.Format("Log Date    : {0:dd}/{0:MM}/{0:yyyy}", DateTime.Now));
            Console.WriteLine(string.Format("Log Time    : {0:hh}:{0:mm}:{0:ss} {0:tt}", DateTime.Now));
            Console.WriteLine("-----------------------------------------------------------------------------------------");
            Console.WriteLine();
        }
    }
}
