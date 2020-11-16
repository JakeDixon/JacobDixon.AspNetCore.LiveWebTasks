using System;
using System.Runtime.Serialization;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{
    [Serializable]
    internal class NoTaskRegistered : Exception
    {
        protected string _compilerName = string.Empty;
        protected const string _defaultErrorMessage = "{0} was not found in the registered tasks.";

        public NoTaskRegistered() { }
        public NoTaskRegistered(string message, Exception inner) : base(message, inner) { }
        public NoTaskRegistered(string compilerName) : base(_defaultErrorMessage)
        {
            _compilerName = compilerName;
        }

        public NoTaskRegistered(string compilerName, string message) : base(message)
        {
            _compilerName = compilerName;
        }

        public override string Message
        {
            get
            {
                string s = base.Message;
                if (_compilerName.IsNullOrEmpty())
                {
                    return s;
                }
                else
                {
                    return string.Format(s, _compilerName);
                }
            }
        }

        protected NoTaskRegistered(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _compilerName = info.GetString("CompilerName");
        }

        [System.Security.SecurityCritical]  // auto-generated_required
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("CompilerName", _compilerName, typeof(string));
        }
    }
}