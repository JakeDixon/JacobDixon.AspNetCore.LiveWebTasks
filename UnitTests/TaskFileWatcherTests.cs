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

namespace LiveSassCompileUnitTests
{
    public class TaskFileWatcherTests
    {
        private const string _testDirectoryName = "LiveSassCompileTest.SassFileWatcherTests";
        private string _testRootPath;
        private string _testSourcePath;
        private string _testDestinationPath;
        private FileWatcherOptions _testOptions;

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

            _testOptions = new FileWatcherOptions { SourcePath = _testSourcePath, DestinationPath = _testDestinationPath, CompileOnStart = false };
        }


        [Fact]
        public void SassFileWatcher_NullSourcePath_ThrowsEmptyStringException()
        {
            // Arrange
            var options = new FileWatcherOptions { SourcePath = null };
            var compilerMock = new Mock<ITask>();

            // Assert
            Assert.Throws<EmptyStringException>(() => new TaskFileWatcher(options, compilerMock.Object));
        }

        [Fact]
        public void SassFileWatcher_EmptyFileNameFilters_ThrowsEmptyArrayException()
        {
            // Arrange
            var options = new FileWatcherOptions { SourcePath = "test", FileNameFilters = new List<string>() };
            var compilerMock = new Mock<ITask>();

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
            var options = new FileWatcherOptions { SourcePath = _testOptions.SourcePath, CompileOnStart = true };
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
        public void StartFileWatcher_FileCreated_CreatesDestinationFile()
        {
            var compilerMock = new Mock<ITask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                compilerMock.Setup(o => o.Run(It.IsAny<string>()));
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
            compilerMock.Verify(o => o.Run(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void StartFileWatcher_FileUpdated_UpdatesDestinationFile()
        {
            var compilerMock = new Mock<ITask>();

            try
            {
                // Arrange
                InitialiseTestEnvironment();
                string _updateContentsScssFileName = "update.scss";

                compilerMock.Setup(o => o.Run(It.IsAny<string>()));
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
            compilerMock.Verify(o => o.Run(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void StartFileWatcher_FileRenamed_CreatesNewDestinationFile()
        {
            var compilerMock = new Mock<ITask>();
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                string _renameFileScssOldFileName = "rename.scss";
                string _renameFileScssNewFileName = "renameNew.scss";

                compilerMock.Setup(o => o.Run(It.IsAny<string>()));
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
            compilerMock.Verify(o => o.Run(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void StartFileWatcher_FileRenamed_DeletesOldDestinationFile()
        {
            bool fileExists = true;
            var compilerMock = new Mock<ITask>();
            string _renameFileScssOldFileName = "renameOldDeleted.scss";
            string _renameFileCssOldFileName = "renameOldDeleted.css";
            string _renameFileScssNewFileName = "renameNewOldDeleted.scss";

            try
            {
                // Arrange
                InitialiseTestEnvironment();

                compilerMock.Setup(o => o.Run(It.IsAny<string>()));
                var oldPath = Path.Combine(_testSourcePath, _renameFileScssOldFileName);
                WriteScssFile(oldPath);
                WriteScssFile(Path.Combine(_testDestinationPath, _renameFileCssOldFileName));
                var watcher = new TaskFileWatcher(_testOptions, compilerMock.Object);
                watcher.StartFileWatcher();

                // Act
                var newPath = Path.Combine(_testSourcePath, _renameFileScssNewFileName);
                File.Move(oldPath, newPath);

                // Give the file system a second to catch up
                Thread.Sleep(200);

                watcher.StopFileWatcher();
                fileExists = File.Exists(Path.Combine(_testDestinationPath, _renameFileCssOldFileName));
            }
            finally
            {
                CleanUpTestEnvironment();
            }
            // Assert
            compilerMock.Verify(o => o.Run(It.IsAny<string>()), Times.Once);
            Assert.False(fileExists);
        }

        [Fact]
        public void StartFileWatcher_FileDeleted_DeletesDestinationFile()
        {
            bool fileExists = true;
            try
            {
                // Arrange
                InitialiseTestEnvironment();
                string _deleteFileScssFileName = "delete.scss";
                string _deleteFileCssFileName = "delete.css";

                var compilerMock = new Mock<ITask>();
                compilerMock.Setup(o => o.Run(It.IsAny<string>()));
                compilerMock.Setup(o => o.IsExcluded(It.IsAny<string>()));
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
                fileExists = File.Exists(cssPath);
            }
            finally
            {
                CleanUpTestEnvironment();
            }

            // Assert
            Assert.False(fileExists);
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
