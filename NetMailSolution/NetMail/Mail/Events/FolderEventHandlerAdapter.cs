namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// The adapter which receives Folder events.
    /// The methods in this class are empty;  this class is provided as a
    /// convenience for easily creating listeners by extending this class
    /// and overriding only the methods of interest.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public abstract class FolderEventHandlerAdapter : IFolderEventHandler
    {
        /// <summary>
        /// Invoked when a Folder is created.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the FolderEventArgs</param>
        public virtual void FolderCreated(object source, FolderEventArgs e) { }

        /// <summary>
        /// Invoked when a folder is deleted.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the FolderEventArgs</param>
        public virtual void FolderDeleted(object source, FolderEventArgs e) { }

        /// <summary>
        /// Invoked when a folder is renamed.
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="e">the FolderEventArgs</param>
        public virtual void FolderRenamed(object source, FolderEventArgs e) { }
    }
}
