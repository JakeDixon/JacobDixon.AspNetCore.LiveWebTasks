using JacobDixon.AspNetCore.LiveWebTasks.Extensions;
using JacobDixon.AspNetCore.LiveWebTasks.Options;
using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using LiveWebTasksUnitTests;
using LiveWebTasksUnitTests.Compilers;
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
                TaskFileWatcherOptions options = new TaskFileWatcherOptions() {
                    RunOnStart = false,
                    FileNameFilters = new List<string>
                    {
                        "*.scss",
                        "*.sass"
                    },
                    FileNameExclusions = new List<string>
                    {
                        "_*"
                    },
                    DestinationPath = destinationDirectory, 
                    SourcePath = sourceDirectory };
                ITask compiler = new SassCompilerTask(options);

                // Act
                compiler.Run(sourceDirectory);

                foreach(var file in filesToCreate)
                {
                    if (Path.GetFileName(file.SourceLocation).MatchesAnyGlob(options.FileNameExclusions))
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
            if (Directory.Exists(TestPaths.RootDirectory))
                Directory.Delete(TestPaths.RootDirectory, true);
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