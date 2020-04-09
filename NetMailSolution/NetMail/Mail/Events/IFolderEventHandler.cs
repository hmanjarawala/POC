namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// This is the Handler interface for Folder events.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public interface IFolderEventHandler
    {
        /// <summary>
        /// Invoked when a Folder is created.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the FolderEventArgs</param>
        void FolderCreated(object source, FolderEventArgs e);

        /// <summary>
        /// Invoked when a folder is deleted.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the FolderEventArgs</param>
        void FolderDeleted(object source, FolderEventArgs e);

        /// <summary>
        /// Invoked when a folder is renamed.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the FolderEventArgs</param>
        void FolderRenamed(object source, FolderEventArgs e);
    }
}
