using System;
using System.Collections.Generic;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks
{
    /// <summary>
    /// Holds file watcher options
    /// </summary>
    public class FileWatcherOptions
    {
        /// <summary>
        /// The name of the task to run when a file is created/updated.
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// The source path to watch for file changes.
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// The destination path to write compiles css files out to.
        /// Default: wwwroot\css
        /// </summary>
        public string DestinationPath { get; set; } = "wwwroot\\css";

        /// <summary>
        /// The file extensions to watch for changes. 
        /// Accepts an array of glob patterns
        /// Default: [ "*.scss", "*.sass" ]
        /// </summary>
        public List<string> FileNameFilters { get; set; } = new List<string>();

        /// <summary>
        /// The file name patters to exclude from compiling. 
        /// Accepts an array of glob patterns.
        /// Default: [ "_*" ]
        /// </summary>
        public List<string> FileNameExclusions { get; set; } = new List<string> { "_*" };

        /// <summary>
        /// Compile the sass file(s) after starting to watch for changes
        /// Default: true
        /// </summary>
        public bool RunOnStart { get; set; } = true;
    }
}
