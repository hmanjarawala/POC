using System;

namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// This class models Connection events.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class ConnectionEventArgs : MailEventArgs
    {
        public enum Type
        {
            /// <summary>
            /// A Connection was opened.
            /// </summary>
            OPENED = 1,

            /// <summary>
            /// A Connection was disconnected.
            /// </summary>
            DISCONNECTED = 2,

            /// <summary>
            /// A Connection was closed.
            /// </summary>
            CLOSED = 3
        };

        private static readonly long serialVersionUID = -1855480171284792957L;

        protected Type eventType;

        /// <summary>
        /// Return the type of this event
        /// </summary>
        public Type EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Construct a ConnectionEvent.
        /// </summary>
        /// <param name="eventType">the event type</param>
        public ConnectionEventArgs(Type eventType) : base()
        {
            this.eventType = eventType;
        }

        /// <summary>
        /// Invokes the appropriate ConnectionListener method
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="eventHandler">the handler to invoke on</param>
        public override void Dispatch(object source, object eventHandler)
        {
            if (eventType == Type.OPENED)
                ((IConnectionEventHandler)eventHandler).Opened(source, this);
            else if (eventType == Type.DISCONNECTED)
                ((IConnectionEventHandler)eventHandler).Disconnected(source, this);
            else if (eventType == Type.CLOSED)
                ((IConnectionEventHandler)eventHandler).Closed(source, this);
        }
    }
}
