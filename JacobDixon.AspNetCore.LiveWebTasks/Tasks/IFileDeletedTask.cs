using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    /// <summary>
    /// An interface that contains a method to bind to file deleted events
    /// </summary>
    public interface IFileDeletedTask
    {
        /// <summary>
        /// The method to call when a file deleted event is raised.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguements.</param>
        void FileDeleted(object sender, FileSystemEventArgs e);
    }
}
