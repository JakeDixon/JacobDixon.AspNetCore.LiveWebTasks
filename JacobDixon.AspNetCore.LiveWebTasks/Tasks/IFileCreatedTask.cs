using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    /// <summary>
    /// An interface that contains a method to bind to file created events
    /// </summary>
    public interface IFileCreatedTask
    {
        /// <summary>
        /// The method to call when a file created event is raised.
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguements.</param>
        void FileCreated(object sender, FileSystemEventArgs e);
    }
}
