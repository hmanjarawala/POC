using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeBean.Mail.Events
{
    public class StoreEventArgs : MailEventArgs
    {
        public enum MessageTypes
        {
            /// <summary>
            /// Indicates that this message is an ALERT.
            /// </summary>
            ALERT = 1,

            /// <summary>
            /// Indicates that this message is a NOTICE.
            /// </summary>
            NOTICE = 2
        };
        private static readonly long serialVersionUID = 1938704919992515330L;

        protected string message;

        protected MessageTypes messageType;

        /// <summary>
        /// The message text to be presented from the Store.
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// The event type.
        /// <see cref="ALERT"/>
        /// <see cref="NOTICE"/>
        /// </summary>
        public MessageTypes MessageType
        {
            get { return messageType; }
        }

        public StoreEventArgs(MessageTypes messageType, string message) : base()
        {
            this.messageType = messageType;
            this.message = message;
        }

        /// <summary>
        /// Invokes the appropriate StoreListener method.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="eventHandler">the handler to invoke on</param>
        public override void Dispatch(object source, object eventHandler)
        {
            ((IStoreEventHandler)eventHandler).Notification(source, this);
        }
    }
}
