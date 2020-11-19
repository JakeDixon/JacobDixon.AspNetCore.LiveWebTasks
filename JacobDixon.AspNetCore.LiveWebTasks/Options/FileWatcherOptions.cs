using System;
using System.Collections.Generic;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Options
{
    /// <summary>
    /// Holds file watcher options
    /// </summary>
    public class FileWatcherOptions
    {
        /// <summary>
        /// The name of the task to run when a file is created/updated.
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// The source path to watch for file changes.
        /// </summary>
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// The destination path to write compiles css files out to.
        /// </summary>
        public string DestinationPath { get; set; } = string.Empty;

        /// <summary>
        /// The file extensions to watch for changes. 
        /// Accepts an array of glob patterns
        /// </summary>
        public List<string> FileNameFilters { get; set; } = new List<string>();

        /// <summary>
        /// The file name patters to exclude from compiling. 
        /// Accepts an array of glob patterns.
        /// </summary>
        public List<string> FileNameExclusions { get; set; } = new List<string>();

        /// <summary>
        /// Compile the sass file(s) after starting to watch for changes
        /// Default: true
        /// </summary>
        public bool RunOnStart { get; set; } = true;

        /// <summary>
        /// A dictionary of additional options to make available to the task.
        /// </summary>
        public Dictionary<string, string> AdditionalOptions { get; set; } = new Dictionary<string, string>();
    }
}
