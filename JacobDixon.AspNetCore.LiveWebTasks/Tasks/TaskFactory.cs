using JacobDixon.AspNetCore.LiveWebTasks.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Tasks
{
    public class TaskFactory
    {
        protected Dictionary<string, Type> _tasks = new Dictionary<string, Type>();

        public ITask GetTask(string name, FileWatcherOptions options)
        {
            if (_tasks.ContainsKey(name))
            {
                Type type = _tasks[name];
                object? task = Activator.CreateInstance(type, options);
                if (task == null)
                    throw new NullReferenceException("Activator failed to create an instance of " + nameof(type));

                return (ITask)task;
            }

            throw new NoTaskRegistered(name);
        }

        public void Register(string name, Type type)
        {
            if (_tasks.ContainsKey(name))
            {
                if (_tasks[name] != type)
                {
                    Type registeredType = _tasks[name];
                    throw new CompilerNameConflictException(name, nameof(type), nameof(registeredType));
                }
                return;
            }

            _tasks.Add(name, type);
        }

        public void Deregister(string name)
        {
            if (_tasks.ContainsKey(name))
                _tasks.Remove(name);
        }
    }
}
