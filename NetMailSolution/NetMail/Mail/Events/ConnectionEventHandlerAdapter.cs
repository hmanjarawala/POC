namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// The adapter which receives connection events.
    /// The methods in this class are empty;  this class is provided as a
    /// convenience for easily creating listeners by extending this class
    /// and overriding only the methods of interest.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public abstract class ConnectionEventHandlerAdapter : IConnectionEventHandler
    {
        /// <summary>
        /// Invoked when a Store/Folder/Transport is opened.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the ConnectionEventArgs</param>
        public virtual void Closed(object source, ConnectionEventArgs e) { }

        /// <summary>
        /// Invoked when a Store is disconnected. Note that a folder
        /// cannot be disconnected, so a folder will not fire this event
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the ConnectionEventArgs</param>
        public virtual void Disconnected(object source, ConnectionEventArgs e) { }

        /// <summary>
        /// Invoked when a Store/Folder/Transport is closed.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the ConnectionEventArgs</param>
        public virtual void Opened(object source, ConnectionEventArgs e) { }
    }
}
