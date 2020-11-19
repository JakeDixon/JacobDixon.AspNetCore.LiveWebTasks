using JacobDixon.AspNetCore.LiveWebTasks.Options;
using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks
{
    /// <summary>
    /// The task initialiser that starts and stops the file watchers from triggering tasks.
    /// </summary>
    public class TaskInitialiser : ITaskInitialiser
    {
        private List<TaskFileWatcher> _sassFileWatchers = new List<TaskFileWatcher>();
        private readonly IOptions<LiveWebTasksOptions> _options;
        private readonly ITaskFactory _taskFactory;

        /// <summary>
        /// Constructs the task initialiser with the options and task factory provided.
        /// </summary>
        /// <param name="options">The options to create the file watchers and tasks with.</param>
        /// <param name="taskFactory">The task factory to get new tasks from.</param>
        public TaskInitialiser(IOptions<LiveWebTasksOptions> options, ITaskFactory taskFactory)
        {
            _options = options;
            _taskFactory = taskFactory;
        }

        /// <summary>
        /// Starts all the file watchers, enabling them to raise events that call tasks on file changes.
        /// </summary>
        public void StartFileWatchers()
        {
            var sassFileWatchersOptions = _options.Value.TaskFileWatchers;

            foreach (var sassFileWatcherOptions in sassFileWatchersOptions)
            {
                ITask task = _taskFactory.CreateTask(sassFileWatcherOptions.TaskName, sassFileWatcherOptions);
                var sassFileWatcher = new TaskFileWatcher(sassFileWatcherOptions, task);
                sassFileWatcher.StartFileWatcher();
                _sassFileWatchers.Add(sassFileWatcher);
            }

        }

        /// <summary>
        /// Stops all the file watchers preventing them from raising events on file changes.
        /// </summary>
        public void StopFileWatchers()
        {
            foreach (var fw in _sassFileWatchers)
            {
                fw.StopFileWatcher();
            }
        }
    }
}
