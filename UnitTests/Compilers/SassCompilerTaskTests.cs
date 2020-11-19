using JacobDixon.AspNetCore.LiveWebTasks.Options;
using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using LiveSassCompileUnitTests.Compilers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks.Tests
{
    public class SassCompilerTaskTests
    {
        private string _testRootPath;

        [Theory]
        [ClassData(typeof(SassCompilerTaskTestDataCollection))]
        public void Compile(List<SassCompilerTaskTestData> filesToCreate, string sourceDirectory, string destinationDirectory)
        {
            bool fileExists = false;
            try
            {
                // Arrange
                foreach (var file in filesToCreate)
                {
                    WriteScssFile(file.SourceLocation, file.FileContent);
                }
                FileWatcherOptions options = new FileWatcherOptions() { RunOnStart = false, DestinationPath = destinationDirectory, SourcePath = sourceDirectory };
                ITask compiler = new SassCompilerTask(options);

                // Act
                compiler.Run(sourceDirectory);

                foreach(var file in filesToCreate)
                {
                    if (compiler.IsExcluded(Path.GetFileName(file.SourceLocation)))
                        fileExists = !File.Exists(file.DestinationLocation);
                    else
                        fileExists = File.Exists(file.DestinationLocation);

                    if (fileExists == false)
                        break;
                }
            }
            finally
            {
                CleanUpTestEnvironment();
            }
            // Assert
            Assert.True(fileExists);
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

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, content);
        }
    }
}