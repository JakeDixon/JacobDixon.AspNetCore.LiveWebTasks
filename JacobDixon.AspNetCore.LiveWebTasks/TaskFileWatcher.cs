using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using JacobDixon.AspNetCore.LiveWebTasks.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using JacobDixon.AspNetCore.LiveWebTasks.Options;
using JacobDixon.AspNetCore.LiveWebTasks.Extensions;

namespace JacobDixon.AspNetCore.LiveWebTasks
{
    public class TaskFileWatcher : FileSystemWatcher
    {
        private readonly TaskFileWatcherOptions _options;
        private Dictionary<string, DateTime> _lastRead = new Dictionary<string, DateTime>();
        private readonly ITask _task;

        private Action<object, FileSystemEventArgs> _fileCreated;
        private Action<object, FileSystemEventArgs> _fileChanged;
        private Action<object, RenamedEventArgs> _fileRenamed;
        private Action<object, FileSystemEventArgs> _fileDeleted;

        public TaskFileWatcher(TaskFileWatcherOptions options, ITask task) : base(options.SourcePath)
        {
            _task = task;
            _options = options;

            if (string.IsNullOrEmpty(_options.DestinationPath))
                throw new EmptyStringException("DestinationPath option must not be empty or null");

            if (_options.FileNameFilters.Count == 0)
                throw new EmptyArrayException("FileNameFilters must contain atleast one filter");

            foreach (var filter in _options.FileNameFilters)
                Filters.Add(filter);

            IncludeSubdirectories = true;

            NotifyFilter = NotifyFilters.LastWrite
                            | NotifyFilters.FileName
                            | NotifyFilters.DirectoryName;

            if (_task is IFileCreatedTask fileCreated)
            {
               _fileCreated = fileCreated.FileCreated;
            }

            if (_task is IFileChangedTask fileChanged)
            {
                _fileChanged = fileChanged.FileChanged;
            }

            if (_task is IFileRenamedTask fileRenamed)
            {
                _fileRenamed = fileRenamed.FileRenamed;
            }

            if (_task is IFileDeletedTask fileDeleted)
            {
                _fileDeleted = fileDeleted.FileDeleted;
            }

            Created += TaskFileWatcher_Created;
            Changed += TaskFileWatcher_Changed;
            Renamed += TaskFileWatcher_Renamed;
            Deleted += TaskFileWatcher_Deleted;
        }

        public void StartFileWatcher()
        {
            if (_options.RunOnStart)
            {
                _task.Run(_options.SourcePath);
            }

            if (_options.SubscribeToEvents)
            {
                EnableRaisingEvents = true;
            }
        }

        public void StopFileWatcher()
        {
            Created -= TaskFileWatcher_Created;
            Changed -= TaskFileWatcher_Changed;
            Renamed -= TaskFileWatcher_Renamed;
            Deleted -= TaskFileWatcher_Deleted;

            EnableRaisingEvents = false;
            Dispose();
        }

        private bool HasFileChangedSinceLastRun(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var lastWriteTime = File.GetLastWriteTime(filePath);

            if (_lastRead.ContainsKey(filePath) && _lastRead[filePath] > lastWriteTime)
                return false;
            
            _lastRead[filePath] = DateTime.Now;

            return true;
        }

        private void DeleteCompiledFile(string filePath)
        {
            //if (filePath.IsNullOrEmpty())
            //    return;

            //var fileName = Path.GetFileName(filePath);
            //if (_task.IsExcluded(fileName))
            //    _task.Run(_options.SourcePath, IsExcluded);

            //var cssFilePath = Path.ChangeExtension(filePath, _compileFileExtension);
            //var relativePath = Path.GetRelativePath(_options.SourcePath, cssFilePath);
            //var destinationPath = Path.Combine(_options.DestinationPath, relativePath);

            //if (File.Exists(destinationPath))
            //{
            //    File.Delete(destinationPath);
            //}
        }

        private void TaskFileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (HasFileChangedSinceLastRun(e.FullPath))
                _fileCreated?.Invoke(sender, e);
        }

        private void TaskFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (HasFileChangedSinceLastRun(e.FullPath))
                _fileChanged?.Invoke(sender, e);
        }

        private void TaskFileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            _lastRead.Remove(e.OldFullPath);
            if (HasFileChangedSinceLastRun(e.FullPath))
                _fileRenamed?.Invoke(sender, e);
        }

        private void TaskFileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _lastRead.Remove(e.FullPath);
            _fileDeleted?.Invoke(sender, e);
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
