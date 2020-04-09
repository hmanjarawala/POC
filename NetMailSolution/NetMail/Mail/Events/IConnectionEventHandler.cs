namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// This is the Handler interface for Connection events.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public interface IConnectionEventHandler
    {
        /// <summary>
        /// Invoked when a Store/Folder/Transport is opened.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the ConnectionEventArgs</param>
        void Opened(object source, ConnectionEventArgs e);

        /// <summary>
        /// Invoked when a Store is disconnected. Note that a folder
        /// cannot be disconnected, so a folder will not fire this event
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the ConnectionEventArgs</param>
        void Disconnected(object source, ConnectionEventArgs e);

        /// <summary>
        /// Invoked when a Store/Folder/Transport is closed.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the ConnectionEventArgs</param>
        void Closed(object source, ConnectionEventArgs e);
    }
}
