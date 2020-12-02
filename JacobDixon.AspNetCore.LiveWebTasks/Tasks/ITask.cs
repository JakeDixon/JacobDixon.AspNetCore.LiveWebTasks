using System;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    /// <summary>
    /// Defines the standard methods expected on a Task object. Used for registering
    /// tasks to the TaskFactory.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// The method to call when a file has changed within the file watcher.
        /// </summary>
        /// <param name="path">The path to the file that has changed</param>
        void Run(string path);
    }
}