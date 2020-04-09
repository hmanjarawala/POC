using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;

namespace IoCSample
{
    public class XmlLogger : ILogger
    {
        XmlDocument xdoc = null;
        private const string _fileName = "AppLog.xml";
        public string LogFileDirectory { get; private set; }
        public XmlLogger(string logFileDirectory) { LogFileDirectory = logFileDirectory; }
        public XmlLogger() : this(@"C:\Windows\Temp") { }

        public void OpenLog()
        {
            if (!Directory.Exists(LogFileDirectory))
                Directory.CreateDirectory(LogFileDirectory);

            xdoc = new XmlDocument();

            if (File.Exists(Path.Combine(LogFileDirectory, _fileName)))
                xdoc.Load(Path.Combine(LogFileDirectory, _fileName));
        }

        public void CloseLog()
        {
            if (xdoc != null)
            {
                xdoc.Save(Path.Combine(LogFileDirectory, _fileName));
                xdoc = null;
            }
        }

        public void Log(string message)
        {
            if (xdoc != null)
            {
                XmlElement root = null;
                XmlElement element = null;
                if (xdoc.DocumentElement == null)
                {
                    XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    xdoc.AppendChild(dec);
                    root = xdoc.CreateElement("Logs");
                }
                else
                    root = xdoc.DocumentElement;

                element = xdoc.CreateElement("Log");

                var e = xdoc.CreateElement("Message");
                e.InnerText = message;
                element.AppendChild(e);

                e = xdoc.CreateElement("Date");
                e.InnerText = string.Format("{0:dd}/{0:MM}/{0:yyyy}", DateTime.Now);
                element.AppendChild(e);

                e = xdoc.CreateElement("Time");
                e.InnerText = string.Format("{0:hh}:{0:mm}:{0:ss} {0:tt}", DateTime.Now);
                element.AppendChild(e);

                root.AppendChild(element);
                xdoc.AppendChild(root);
            }
        }
    }
}
