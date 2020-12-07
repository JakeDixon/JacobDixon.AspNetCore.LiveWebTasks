using JacobDixon.AspNetCore.LiveWebTasks.Exceptions;
using JacobDixon.AspNetCore.LiveWebTasks.Extensions;
using JacobDixon.AspNetCore.LiveWebTasks.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    /// <summary>
    /// The SassCompilerTask class. Used to compile SASS and SCSS files from
    /// either a file or directory path.
    /// </summary>
    public class SassCompilerTask : ITask, IFileChangedTask, IFileCreatedTask, IFileRenamedTask, IFileDeletedTask
    {
        private const int _maxRetryAttempts = 2;
        private const string _compileFileExtension = ".css";
        private const int _msDelayBetweenRetries = 100;
        private TaskFileWatcherOptions _options;

        /// <summary>
        /// Creates a new SassCompilerTask ready to be called when a file changes.
        /// </summary>
        /// <param name="options">The file watcher options. Must have the SourcePath, DestinationPath and FileNameExclusions.</param>
        public SassCompilerTask(TaskFileWatcherOptions options)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public void FileChanged(object sender, FileSystemEventArgs e)
        {

        }

        public void FileCreated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void FileDeleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void FileRenamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Run(string path)
        {
            var fileName = Path.GetFileName(path) ?? throw new ArgumentNullException(nameof(path));
            var isDirectory = Directory.Exists(path);
            var isExcluded = fileName.MatchesAnyGlob(_options.FileNameExclusions);

            if (isExcluded)
                CompileDirectory(_options.SourcePath);
            else if (isDirectory)
                CompileDirectory(path);
            else
                CompileFile(path);
        }

        private void CompileDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                CompileFile(file);
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                CompileDirectory(directory);
            }
        }

        private void CompileFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var fileName = Path.GetFileName(filePath);

            if (fileName.MatchesAnyGlob(_options.FileNameExclusions))
                return;

            var cssFilePath = Path.ChangeExtension(filePath, _compileFileExtension);
            var cssFileName = Path.GetFileName(cssFilePath);

            var attempts = 0;
            var successful = false;

            while (!successful && attempts < _maxRetryAttempts)
            {
                try
                {
                    var result = LibSassHost.SassCompiler.CompileFile(
                        filePath,
                        cssFileName,
                        options: new LibSassHost.CompilationOptions
                        {
                            OutputStyle = LibSassHost.OutputStyle.Compressed,
                            IncludePaths = { _options.SourcePath }
                        });

                    var relativePath = Path.GetRelativePath(_options.SourcePath, cssFilePath);
                    var destinationFile = Path.Combine(_options.DestinationPath, relativePath);
                    var destinationPath = Path.GetDirectoryName(destinationFile);
                    Directory.CreateDirectory(destinationPath);
                    File.WriteAllText(Path.Combine(_options.DestinationPath, relativePath), result.CompiledContent);
                    successful = true;
                }
                catch (Exception e)
                {
                    if (attempts >= _maxRetryAttempts)
                        Console.WriteLine(e.ToString());
                    attempts++;
                    Thread.Sleep(_msDelayBetweenRetries);
                }
            }
        }
    }
}
