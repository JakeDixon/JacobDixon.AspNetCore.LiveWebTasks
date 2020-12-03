using JacobDixon.AspNetCore.LiveWebTasks;
using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using JacobDixon.AspNetCore.LiveWebTasks.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using JacobDixon.AspNetCore.LiveWebTasks.Options;

namespace LiveWebTasksUnitTests
{
    public class TaskFileWatcherTests
    {
        private const string _testDirectoryName = "LiveWebTasksTest.TaskFileWatcherTests";
        private string _testRootPath;
        private string _testSourcePath;
        private string _testDestinationPath;
        private TaskFileWatcherOptions _testOptions;

        private void InitialiseTestEnvironment()
        {
            var tempDir = Environment.GetEnvironmentVariable("temp");
            if (string.IsNullOrEmpty(tempDir))
                throw new XunitException("Unable to load the temp directory from environment variables.");

            _testRootPath = Path.Combine(tempDir, _testDirectoryName);
            Directory.CreateDirectory(_testRootPath);

            _testSourcePath = Path.Combine(_testRootPath, "Source");
            Directory.CreateDirectory(_testSourcePath);

            _testDestinationPath = Path.Combine(_testRootPath, "Destination");
            Directory.CreateDirectory(_testDestinationPath);

            _testOptions = new TaskFileWatcherOptions 
            { 
                FileNameFilters = new List<string>
                {
                    "*.scss",
                    "*.sass"
                },
                FileNameExclusions = new List<string>
                {
                    "_*"
                },
                SourcePath = _testSourcePath, 
                DestinationPath = _testDestinationPath, 
                RunOnStart = false 
            };
        }

        [Fact]
        public void TaskFileWatcher_EmptyFileNameFilters_ThrowsEmptyArrayException()
        {
            TaskFileWatcherOptions options;
            var compilerMock = new Mock<ITask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                options = new TaskFileWatcherOptions
                {
                    SourcePath = _testOptions.SourcePath,
                    DestinationPath = _testOptions.DestinationPath,
                    FileNameFilters = new List<string>(),
                    RunOnStart = false
                    // TODO: need to assert before the clean up happens so that the folders are still there.
                };
            }
            finally
            {
                CleanUpTestEnvironment();
            }

            // Assert
            Assert.Throws<EmptyArrayException>(() => new TaskFileWatcher(options, compilerMock.Object));
        }

        [Fact]
        public void StartFileWatcher_CompileOnStartTrue_CallsICompilerCompileMethod()
        {
            // Arrange
            InitialiseTestEnvironment();
            var compilerMock = new Mock<ITask>();
            compilerMock.Setup(o => o.Run(It.IsAny<string>()));
            var options = new TaskFileWatcherOptions 
            { 
                SourcePath = _testOptions.SourcePath, 
                DestinationPath = _testOptions.DestinationPath, 
                FileNameExclusions = _testOptions.FileNameExclusions,
                FileNameFilters = _testOptions.FileNameFilters,
                RunOnStart = true 
            };
            var sassWatcher = new TaskFileWatcher(options, compilerMock.Object);

            // Act
            sassWatcher.StartFileWatcher();
            sassWatcher.StopFileWatcher();

            CleanUpTestEnvironment();
            // Assert
            compilerMock.Verify(o => o.Run(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void StartFileWatcher_CompileOnStartFalse_DoesNotCallICompilerCompileMethod()
        {
            var compilerMock = new Mock<ITask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                compilerMock.Setup(o => o.Run(It.IsAny<string>()));
                var sassWatcher = new TaskFileWatcher(_testOptions, compilerMock.Object);

                // Act
                sassWatcher.StartFileWatcher();
                sassWatcher.StopFileWatcher();
            }
            finally
            {
                CleanUpTestEnvironment();
            }
            // Assert
            compilerMock.Verify(o => o.Run(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void TaskFileWatcher_FileCreated_CallFileCreatedMethod()
        {
            var compilerMock = new Mock<ITask>();
            var fileCreatedMock = compilerMock.As<IFileCreatedTask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                fileCreatedMock.Setup(o => o.FileCreated(It.IsAny<object>(), It.IsAny<FileSystemEventArgs>()));
                var watcher = new TaskFileWatcher(_testOptions, compilerMock.Object);
                watcher.StartFileWatcher();

                // Act
                WriteScssFile(Path.Combine(_testSourcePath, "styles.scss"));
                // Give the file system a second to catch up
                Thread.Sleep(200);

                watcher.StopFileWatcher();
            }
            finally
            {
                CleanUpTestEnvironment();
            }
            // Assert
            fileCreatedMock.Verify(o => o.FileCreated(It.IsAny<object>(), It.IsAny<FileSystemEventArgs>()), Times.Once);
        }

        [Fact]
        public void TaskFileWatcher_FileUpdated_CallsFileUpdatedMethod()
        {
            var compilerMock = new Mock<ITask>();
            var fileChangedMock = compilerMock.As<IFileChangedTask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                string _updateContentsScssFileName = "update.scss";

                fileChangedMock.Setup(o => o.FileChanged(It.IsAny<object>(), It.IsAny<FileSystemEventArgs>()));
                WriteScssFile(Path.Combine(_testSourcePath, _updateContentsScssFileName));
                var watcher = new TaskFileWatcher(_testOptions, compilerMock.Object);
                watcher.StartFileWatcher();

                // Act
                WriteScssFile(Path.Combine(_testSourcePath, _updateContentsScssFileName), @"body {  
color: blue;
}");
                // Give the file system a second to catch up
                Thread.Sleep(200);

                watcher.StopFileWatcher();
            }
            finally
            {
                CleanUpTestEnvironment();
            }
            // Assert
            fileChangedMock.Verify(o => o.FileChanged(It.IsAny<object>(), It.IsAny<FileSystemEventArgs>()), Times.Once);
        }

        [Fact]
        public void TaskFileWatcher_FileRenamed_CallsFileRenamedMethod()
        {
            var compilerMock = new Mock<ITask>();
            var fileRenamedMock = compilerMock.As<IFileRenamedTask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                string _renameFileScssOldFileName = "rename.scss";
                string _renameFileScssNewFileName = "renameNew.scss";

                fileRenamedMock.Setup(o => o.FileRenamed(It.IsAny<object>(), It.IsAny<RenamedEventArgs>()));
                WriteScssFile(Path.Combine(_testSourcePath, _renameFileScssOldFileName));
                var watcher = new TaskFileWatcher(_testOptions, compilerMock.Object);
                watcher.StartFileWatcher();

                // Act
                var oldPath = Path.Combine(_testSourcePath, _renameFileScssOldFileName);
                var newPath = Path.Combine(_testSourcePath, _renameFileScssNewFileName);
                File.Move(oldPath, newPath);

                // Give the file system a second to catch up
                Thread.Sleep(200);

                watcher.StopFileWatcher();
            }
            finally
            {
                CleanUpTestEnvironment();
            }
            // Assert
            fileRenamedMock.Verify(o => o.FileRenamed(It.IsAny<object>(), It.IsAny<RenamedEventArgs>()), Times.Once);
        }

        [Fact]
        public void TaskFileWatcher_FileDeleted_CallsFileDeletedMethod()
        {
            var compilerMock = new Mock<ITask>();
            var fileDeletedMock = compilerMock.As<IFileDeletedTask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                string _deleteFileScssFileName = "delete.scss";
                string _deleteFileCssFileName = "delete.css";

                fileDeletedMock.Setup(o => o.FileDeleted(It.IsAny<object>(), It.IsAny<FileSystemEventArgs>()));
                var scssPath = Path.Combine(_testSourcePath, _deleteFileScssFileName);
                var cssPath = Path.Combine(_testDestinationPath, _deleteFileCssFileName);
                WriteScssFile(scssPath);
                WriteScssFile(cssPath);
                var watcher = new TaskFileWatcher(_testOptions, compilerMock.Object);
                watcher.StartFileWatcher();

                // Act
                File.Delete(scssPath);

                // Give the file system a second to catch up
                Thread.Sleep(200);

                watcher.StopFileWatcher();
            }
            finally
            {
                CleanUpTestEnvironment();
            }

            // Assert
            fileDeletedMock.Verify(o => o.FileDeleted(It.IsAny<object>(), It.IsAny<FileSystemEventArgs>()), Times.Once);
        }

        private void CleanUpTestEnvironment()
        {
            if (Directory.Exists(_testRootPath))
                Directory.Delete(_testRootPath, true);
        }

        private void WriteScssFile(string path, string content = null)
        {
            if (content == null)
            {
                content = @"body {  
color: red;
}";
            }

            File.WriteAllText(path, content);
        }
    }
}
