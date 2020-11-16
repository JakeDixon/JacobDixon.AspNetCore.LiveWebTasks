using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace JacobDixon.AspNetCore.LiveWebTasks
{
    public class LiveSassCompileBackgroundService : IHostedService
    {
        private TaskInitialiser _watcher;
        private readonly IOptions<LiveWebTasksOptions> _options;

        public LiveSassCompileBackgroundService(IOptions<LiveWebTasksOptions> options)
        {
            _options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.EnableLiveCompile)
            {
                _watcher = new TaskInitialiser(_options);
                _watcher.StartFileWatchers();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_watcher != null)
                _watcher.StopFileWatchers();
            return Task.CompletedTask;
        }
    }
}
