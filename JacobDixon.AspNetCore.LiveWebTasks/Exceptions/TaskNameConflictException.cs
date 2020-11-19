using JacobDixon.AspNetCore.LiveWebTasks.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{
    /// <summary>
    /// The execption to raise if a compiler is already registered with that name.
    /// </summary>
    [Serializable]
    internal class TaskNameConflictException : Exception
    {
        /// <summary>
        /// The task name that was is conflicting.
        /// </summary>
        protected string _taskName = string.Empty;
        /// <summary>
        /// The <see cref="Type"/> of the task which was attempted to be registered.
        /// </summary>
        protected string _taskType = string.Empty;
        /// <summary>
        /// The <see cref="Type"/> of the task already registered.
        /// </summary>
        protected string _registeredTaskType = string.Empty;
        /// <summary>
        /// The default message to be used with string format.
        /// 0 = <see cref="_taskName"/>
        /// 1 = <see cref="_taskType"/>
        /// 2 = <see cref="_registeredTaskType"/>
        /// </summary>
        protected const string _defaultErrorMessage = "Cannot register {0} with type {1} as it is already registered with type {2}.";

        /// <summary>
        /// Constructs an empty exception.
        /// </summary>
        public TaskNameConflictException() { }

        /// <summary>
        /// Constructs an exception with only a message.
        /// </summary>
        /// <param name="message">The message to display for the exception</param>
        public TaskNameConflictException(string message) : base(message) { }

        /// <summary>
        /// Constructs an exception with a message and inner exception.
        /// </summary>
        /// <param name="message">The message to display for the exception</param>
        /// <param name="inner">The inner exception to include</param>
        public TaskNameConflictException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Constructs an exception using the default message with the placeholders replaced by 
        /// <paramref name="taskName"/>
        /// <paramref name="taskTypeName"/>
        /// <paramref name="registeredTaskTypeName"/>
        /// </summary>
        /// <param name="taskName">The name of the task that caused the conflict</param>
        /// <param name="taskTypeName">The <see cref="Type"/> of task that was attempted to be registered</param>
        /// <param name="registeredTaskTypeName">The <see cref="Type"/> of task that is already registered</param>
        public TaskNameConflictException(string taskName, string taskTypeName, string registeredTaskTypeName) : base(_defaultErrorMessage)
        {
            _taskName = taskName;
            _taskType = taskTypeName;
            _registeredTaskType = registeredTaskTypeName;
        }

        /// <summary>
        /// Constructs an exception using the <paramref name="message"/> provided using string format where
        /// 0 = <paramref name="taskName"/>
        /// 1 = <paramref name="taskTypeName"/>
        /// 2 = <paramref name="registeredTaskTypeName"/> 
        /// </summary>
        /// <param name="taskName">The name of the task that caused the conflict</param>
        /// <param name="taskTypeName">The <see cref="Type"/> of task that was attempted to be registered</param>
        /// <param name="registeredTaskTypeName">The <see cref="Type"/> of task that is already registered</param>
        /// <param name="message">The message to display for the exception</param>
        public TaskNameConflictException(string taskName, string taskTypeName, string registeredTaskTypeName, string message) : base(message)
        {
            _taskName = taskName;
            _taskType = taskTypeName;
            _registeredTaskType = registeredTaskTypeName;
        }

        /// <summary>
        /// The message of the exception
        /// </summary>
        public override string Message
        {
            get
            {
                string s = base.Message;
                if (_taskName.IsNullOrEmpty() || _taskType.IsNullOrEmpty() || _registeredTaskType.IsNullOrEmpty())
                {
                    return s;
                }
                else
                {
                    return string.Format(s, _taskName, _taskType, _registeredTaskType);
                }
            }
        }

        /// <summary>
        /// Constructs an exception based on serialised information.
        /// </summary>
        /// <param name="info">The serialised information to pull properties from</param>
        /// <param name="context">The streaming context</param>
        protected TaskNameConflictException(SerializationInfo info, StreamingContext context) : base(info, context) {
            _taskName = info.GetString("TaskName") ?? string.Empty;
            _taskType = info.GetString("TaskType") ?? string.Empty;
            _registeredTaskType = info.GetString("RegisteredTaskType") ?? string.Empty;
        }

        /// <summary>
        /// Serialises the exception so it can be reloaded at a future point.
        /// </summary>
        /// <param name="info">The serialisation info which we can add the private parameters too.</param>
        /// <param name="context">The streaming context</param>
        [System.Security.SecurityCritical]  // auto-generated_required
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("TaskName", _taskName, typeof(string));
            info.AddValue("TaskType", _taskType, typeof(string));
            info.AddValue("RegisteredTaskType", _registeredTaskType, typeof(string));
        }
    }
}
