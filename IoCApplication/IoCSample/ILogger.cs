using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoCSample
{
    public interface ILogger
    {
        void OpenLog();
        void CloseLog();
        void Log(string message);
    }
}
