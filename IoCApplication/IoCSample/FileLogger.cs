using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample
{
    public class FileLogger : ILogger
    {
        StreamWriter streamWriter = null;
        private const string _fileName = "AppLog.txt";
        public string LogFileDirectory { get; private set; }
        public FileLogger(string logFileDirectory) { LogFileDirectory = logFileDirectory; }
        public FileLogger() : this(@"C:\Windows\Temp") { }

        public void OpenLog()
        {
            if (!Directory.Exists(LogFileDirectory))
                Directory.CreateDirectory(LogFileDirectory);

            streamWriter = new StreamWriter(File.Open(Path.Combine(LogFileDirectory, _fileName), FileMode.Append));
            streamWriter.AutoFlush = true;
        }

        public void CloseLog()
        {
            if (streamWriter != null)
            {
                streamWriter.Close();
                streamWriter.Dispose();
                streamWriter = null;
            }
        }

        public void Log(string message)
        {
            if (streamWriter != null)
            {
                streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
                streamWriter.WriteLine(string.Format("Log Message : {0}", message));
                streamWriter.WriteLine(string.Format("Log Date    : {0:dd}/{0:MM}/{0:yyyy}", DateTime.Now));
                streamWriter.WriteLine(string.Format("Log Time    : {0:hh}:{0:mm}:{0:ss} {0:tt}", DateTime.Now));
                streamWriter.WriteLine("-----------------------------------------------------------------------------------------");
                streamWriter.WriteLine();
            }
        }
    }
}
