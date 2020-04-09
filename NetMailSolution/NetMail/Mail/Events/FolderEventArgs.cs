using System;

namespace CoffeeBean.Mail.Events
{
    /// <summary>
    /// This class models Folder <em>existence</em> events. FolderEvents are
    /// delivered to FolderListeners registered on the affected Folder as
    /// well as the containing Store.
    /// </summary>
    public class FolderEventArgs : MailEventArgs
    {
        public enum Type
        {
            /// <summary>
            /// The folder was created.
            /// </summary>
            CREATED = 1,

            /// <summary>
            /// The folder was deleted.
            /// </summary>
            DELETED = 2,

            /// <summary>
            /// The folder was renamed.
            /// </summary>
            RENAMED = 3
        };

        protected Type eventType;

        [NonSerialized]
        protected IFolder folder;

        [NonSerialized]
        protected IFolder newFolder;

        private static readonly long serialVersionUID = 5278131310563694307L;

        /// <summary>
        /// The folder the event occurred on.
        /// </summary>
        public IFolder Folder
        {
            get { return folder; }
        }

        /// <summary>
        /// The folder that represents the new name, in case of a RENAMED event.
        /// </summary>
        public IFolder NewFolder
        {
            get { return newFolder; }
        }

        /// <summary>
        /// The type of this event.
        /// </summary>
        public Type EventType
        {
            get { return eventType; }
        }

        /// <summary>
        /// Construct a FolderEvent, use for RENAMED events.
        /// </summary>
        /// <param name="folder">The folder that is renamed</param>
        /// <param name="newFolder">The folder that represents the new name</param>
        /// <param name="eventType">The event type</param>
        public FolderEventArgs(IFolder folder, IFolder newFolder, Type eventType) : base()
        {
            this.folder = folder;
            this.newFolder = newFolder;
            this.eventType = eventType;
        }        

        /// <summary>
        /// Construct a FolderEvent.
        /// </summary>
        /// <param name="folder">The affected folder</param>
        /// <param name="eventType">The event type</param>
        public FolderEventArgs(IFolder folder, Type eventType) : this(folder, folder, eventType) { }

        /// <summary>
        /// Invokes the appropriate FolderListener method
        /// </summary>
        /// <param name="source">the source of the event</param>
        /// <param name="eventHandler">the handler to invoke on</param>
        public override void Dispatch(object source, object eventHandler)
        {
            if (eventType == Type.CREATED)
                ((IFolderEventHandler)eventHandler).FolderCreated(source, this);
            else if (eventType == Type.DELETED)
                ((IFolderEventHandler)eventHandler).FolderDeleted(source, this);
            else if (eventType == Type.RENAMED)
                ((IFolderEventHandler)eventHandler).FolderRenamed(source, this);
        }
    }
}
