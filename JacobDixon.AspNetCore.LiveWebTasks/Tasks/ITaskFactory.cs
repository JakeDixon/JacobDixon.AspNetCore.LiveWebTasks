using JacobDixon.AspNetCore.LiveWebTasks.Options;
using System;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    /// <summary>
    /// The interface for a task factory that can be used to register and create tasks.
    /// </summary>
    public interface ITaskFactory
    {
        /// <summary>
        /// Removes a task from the registry by name.
        /// </summary>
        /// <param name="name">The name of the task to remove.</param>
        void Deregister(string name);
        /// <summary>
        /// Creates a task based on the type registered with the name, with the options provided.
        /// </summary>
        /// <param name="name">The name of the registered task to create.</param>
        /// <param name="options">The options to pass into it's constructor.</param>
        /// <returns>A new task of the type registered under <paramref name="name"/>.</returns>
        ITask CreateTask(string name, FileWatcherOptions options);
        /// <summary>
        /// Registers a task under the <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to register the task under.</param>
        /// <param name="type">The type of the task to register.</param>
        void Register(string name, Type type);
        /// <summary>
        /// Checks if the <paramref name="name"/> is available for registering.
        /// </summary>
        /// <param name="name">The name of the task to check</param>
        /// <returns><c>true</c> if the task name is available, or <c>false</c> if the task name is already registered.</returns>
        bool IsNameAvailable(string name);
        /// <summary>
        /// Checks if the task type is already registered.
        /// </summary>
        /// <param name="type">The type of task to check if is registered.</param>
        /// <returns><c>true</c> if the task is registered otherwise <c>false</c></returns>
        bool IsTaskRegistered(Type type);
    }
}