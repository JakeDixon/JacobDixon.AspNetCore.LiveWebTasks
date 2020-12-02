using JacobDixon.AspNetCore.LiveWebTasks;
using JacobDixon.AspNetCore.LiveWebTasks.Options;
using JacobDixon.AspNetCore.LiveWebTasks.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LiveWebTasksUnitTests.Compilers
{
    public class TaskFactoryTests
    {
        [Fact]
        public void RegisterCompiler()
        {
            // Arrange
            var factory = new TaskFactory();
            factory.Register("sass", typeof(SassCompilerTask));

            // Act
            var compiler = factory.CreateTask("sass", new TaskFileWatcherOptions());

            // Assert
            Assert.IsType<SassCompilerTask>(compiler);
        }
    }
}
