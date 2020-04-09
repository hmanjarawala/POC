namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// This is the Handler interface for Store Notifications.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public interface IStoreEventHandler
    {
        /// <summary>
        /// Invoked when the Store generates a notification event.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the StoreEvent</param>
        void Notification(object source, StoreEventArgs e);
    }
}
