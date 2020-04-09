using System;

namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// Common base class for mail events, defining the Dispatch method.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public abstract class MailEventArgs : EventArgs
    {
        private static readonly long serialVersionUID = 5846275636325456631L;

        /// <summary>
        /// Construct a MailEventArgs
        /// </summary>
        public MailEventArgs() : base() { }

        /// <summary>
        /// This method invokes the appropriate method on a handler for
        /// this event. Subclasses provide the implementation.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="eventHandler">the handler to invoke on</param>
        public abstract void Dispatch(object source, object eventHandler);
    }
}
