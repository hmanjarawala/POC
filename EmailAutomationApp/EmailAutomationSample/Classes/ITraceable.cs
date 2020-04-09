using System;

namespace EmailAutomationSample.Classes
{
    public delegate void TraceEventHandler(object sender, TraceEventArg e);

    public interface ITraceable
    {
        event TraceEventHandler LogTrace;

        void Trace(TraceEventArg e);
    }

    public sealed class TraceEventArg
    {
        public Func<string> MessageDelegate { get; set; }

        public TraceEventArg(Func<string> messageDelegate)
        {
            MessageDelegate = messageDelegate;
        }
    }
}
