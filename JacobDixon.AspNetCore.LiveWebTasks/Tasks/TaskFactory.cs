using JacobDixon.AspNetCore.LiveWebTasks.Exceptions;
using JacobDixon.AspNetCore.LiveWebTasks.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    /// <summary>
    /// A task factory for registering and creating new tasks of that type.
    /// </summary>
    public class TaskFactory : ITaskFactory
    {
        /// <summary>
        /// The task map between name and type.
        /// </summary>
        protected Dictionary<string, Type> _tasks = new Dictionary<string, Type>();

        /// <summary>
        /// Creates a task based on the <paramref name="name"/> with the <paramref name="options"/>
        /// </summary>
        /// <param name="name">The name of the task to create. Must be registered already.</param>
        /// <param name="options">The options to create the task with.</param>
        /// <returns>A task that implements <see cref="ITask"/></returns>
        public ITask CreateTask(string name, FileWatcherOptions options)
        {
            if (_tasks.ContainsKey(name))
            {
                Type type = _tasks[name];
                object? task = Activator.CreateInstance(type, options);
                if (task == null)
                    throw new NullReferenceException("Activator failed to create an instance of " + nameof(type));

                return (ITask)task;
            }

            throw new NoTaskRegisteredException(name);
        }

        /// <summary>
        /// Registers the task with that name.
        /// </summary>
        /// <param name="name">The name of the task to register</param>
        /// <param name="type">The type of task to create when <see cref="CreateTask(string, FileWatcherOptions)"/> is called</param>
        /// <exception cref="TaskNameConflictException">Name conflict already registered type</exception>
        public void Register(string name, Type type)
        {
            if (_tasks.ContainsKey(name))
            {
                if (_tasks[name] != type)
                {
                    Type registeredType = _tasks[name];
                    throw new TaskNameConflictException(name, nameof(type), nameof(registeredType));
                }
                return;
            }

            _tasks.Add(name, type);
        }

        /// <summary>
        /// Removes the task registered with <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the task to remove</param>
        public void Deregister(string name)
        {
            if (_tasks.ContainsKey(name))
                _tasks.Remove(name);
        }

        /// <summary>
        /// Checks if the <paramref name="name"/> is available for registering.
        /// </summary>
        /// <param name="name">The name of the task to check</param>
        /// <returns><c>true</c> if the task name is available, or <c>false</c> if the task name is already registered.</returns>
        public bool IsNameAvailable(string name)
        {
            return !_tasks.ContainsKey(name);
        }

        /// <summary>
        /// Checks if the task type is already registered.
        /// </summary>
        /// <param name="type">The type of task to check if is registered.</param>
        /// <returns><c>true</c> if the task is registered otherwise <c>false</c></returns>
        public bool IsTaskRegistered(Type type)
        {
            return _tasks.ContainsValue(type);
        }
    }
}