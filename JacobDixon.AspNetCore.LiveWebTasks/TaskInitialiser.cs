using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks
{
    public class TaskInitialiser : ITaskInitialiser
    {
        private List<TaskFileWatcher> _sassFileWatchers = new List<TaskFileWatcher>();
        private readonly IOptions<LiveWebTasksOptions> _options;

        public TaskInitialiser(IOptions<LiveWebTasksOptions> options)
        {
            _options = options;
        }

        public void StartFileWatchers()
        {
            var sassFileWatchersOptions = _options.Value.TaskFileWatchers;

            foreach (var sassFileWatcherOptions in sassFileWatchersOptions)
            {
                ITask sassCompiler = new SassCompilerTask(sassFileWatcherOptions);
                var sassFileWatcher = new TaskFileWatcher(sassFileWatcherOptions, sassCompiler);
                sassFileWatcher.StartFileWatcher();
                _sassFileWatchers.Add(sassFileWatcher);
            }

        }

        public void StopFileWatchers()
        {
            foreach (var fw in _sassFileWatchers)
            {
                fw.StopFileWatcher();
            }
        }
    }
}
