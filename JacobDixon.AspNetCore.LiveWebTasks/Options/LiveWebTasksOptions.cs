using System;
using System.Collections.Generic;
using System.Threading;

namespace JacobDixon.AspNetCore.LiveWebTasks.Options
{
    /// <summary>
    /// A class holding the options for live web tasks
    /// </summary>
    public class LiveWebTasksOptions
    {
        /// <summary>
        /// The const string that defined the options section in settings 
        /// </summary>
        public const string OptionsName = "LiveWebTasksOptions";

        /// <summary>
        /// A boolean value which controls whether live web tasks is on (true) or off (false).
        /// Default: false
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The folders to monitor for sass/scss file changes 
        /// and the matching destination folders.
        /// </summary>
        public List<FileWatcherOptions> TaskFileWatchers { get; set; } = new List<FileWatcherOptions>();
    }
}
