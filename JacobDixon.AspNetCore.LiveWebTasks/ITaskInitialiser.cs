namespace JacobDixon.AspNetCore.LiveWebTasks
{
    /// <summary>
    /// An interface for starting and stopping the file watchers that trigger the tasks.
    /// </summary>
    public interface ITaskInitialiser
    {
        /// <summary>
        /// Starts the file watchers raising events on file changes.
        /// </summary>
        void StartFileWatchers();
        /// <summary>
        /// Stops the file watchers from raising events on file changes.
        /// </summary>
        void StopFileWatchers();
    }
}