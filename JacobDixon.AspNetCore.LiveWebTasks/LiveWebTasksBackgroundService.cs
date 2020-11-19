using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using JacobDixon.AspNetCore.LiveWebTasks.Options;

namespace JacobDixon.AspNetCore.LiveWebTasks
{
    /// <summary>
    /// The background service for watching for file changes and running the associated tasks.
    /// </summary>
    public class LiveWebTasksBackgroundService : IHostedService
    {
        private ITaskInitialiser _taskInitialiser;
        private readonly IOptions<LiveWebTasksOptions> _options;

        /// <summary>
        /// Constructs the background service
        /// </summary>
        /// <param name="options">The Live Web Tasks options from appsettings</param>
        /// <param name="taskInitialiser">The task initialiser interface that sets up file watchers</param>
        public LiveWebTasksBackgroundService(IOptions<LiveWebTasksOptions> options, ITaskInitialiser taskInitialiser)
        {
            _options = options;
            _taskInitialiser = taskInitialiser;
        }

        /// <summary>
        /// Starts the file watchers if Live Web Tasks is enabled via appsettings.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request</param>
        /// <returns>void</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.Enabled)
            {
                _taskInitialiser.StartFileWatchers();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the file watchers.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request</param>
        /// <returns>void</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_taskInitialiser != null)
                _taskInitialiser.StopFileWatchers();
            return Task.CompletedTask;
        }
    }
}
