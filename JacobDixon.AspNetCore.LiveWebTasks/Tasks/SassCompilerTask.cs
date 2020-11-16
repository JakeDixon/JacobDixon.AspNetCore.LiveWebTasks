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
    public class SassCompilerTask : ITask
    {
        private const int _maxRetryAttempts = 2;
        private const string _compileFileExtension = ".css";
        private const int _msDelayBetweenRetries = 100;
        private FileWatcherOptions _options;

        /// <summary>
        /// Creates a new SassCompilerTask ready to be called when a file changes.
        /// </summary>
        /// <param name="options">The file watcher options. Must have the SourcePath, DestinationPath and FileNameExclusions.</param>
        public SassCompilerTask(FileWatcherOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Runs the compiler methods, (either directory or file).
        /// </summary>
        /// <param name="path">The path to compile. (directory or file). 
        /// Directory compiling is recursive.</param>
        public void Run(string path)
        {
            var fileName = Path.GetFileName(path);
            var isDirectory = Directory.Exists(path);

            if (isDirectory || string.IsNullOrEmpty(fileName) || IsExcluded(fileName))
                CompileDirectory(_options.SourcePath);
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

            if (IsExcluded(fileName))
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

        /// <summary>
        /// Checks if a filename is excluded from compiling.
        /// </summary>
        /// <param name="fileName">The filename to check.</param>
        /// <returns><c>true</c> if the filename is excluded. Otherwise <c>false</c>.</returns>
        public bool IsExcluded(string fileName)
        {
            foreach (var exclude in _options.FileNameExclusions)
            {
                if (fileName.MatchesGlob(exclude))
                    return true;
            }

            return false;
        }
    }
}
