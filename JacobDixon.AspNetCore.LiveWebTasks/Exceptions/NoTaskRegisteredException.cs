using System;
using System.Runtime.Serialization;
using JacobDixon.AspNetCore.LiveWebTasks.Extensions;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{
    [Serializable]
    internal class NoTaskRegisteredException : Exception
    {
        protected const string _defaultErrorMessage = "{0} was not found in the registered tasks.";
        protected string _taskName = string.Empty;

        public NoTaskRegisteredException()
        { 
        }

        public NoTaskRegisteredException(string message, Exception inner)
            : base(message, inner) 
        { 
        }

        public NoTaskRegisteredException(string taskName)
            : base(_defaultErrorMessage)
        {
            _taskName = taskName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoTaskRegisteredException"/> class.
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="message"></param>
        public NoTaskRegisteredException(string taskName, string message)
            : base(message)
        {
            _taskName = taskName;
        }

        public override string Message
        {
            get
            {
                string s = base.Message;
                if (_taskName.IsNullOrEmpty())
                {
                    return s;
                }
                else
                {
                    return string.Format(s, _taskName);
                }
            }
        }

        protected NoTaskRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _taskName = info.GetString("TaskName") ?? string.Empty;
        }

        [System.Security.SecurityCritical]  // auto-generated_required
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("TaskName", _taskName, typeof(string));
        }
    }
}