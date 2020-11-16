using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{

    [Serializable]
    public class CompilerNameConflictException : Exception
    {
        protected string _compilerName = string.Empty;
        protected string _compilerType = string.Empty;
        protected string _registeredCompilerType = string.Empty;
        protected const string _defaultErrorMessage = "Cannot register {0} with type {1} as it is already registered with type {2}.";

        public CompilerNameConflictException() { }
        public CompilerNameConflictException(string message) : base(message) { }
        public CompilerNameConflictException(string message, Exception inner) : base(message, inner) { }
        public CompilerNameConflictException(string compilerName, string compilerTypeName, string registeredCompilerTypeName) : base(_defaultErrorMessage)
        {
            _compilerName = compilerName;
            _compilerType = compilerTypeName;
            _registeredCompilerType = registeredCompilerTypeName;
        }

        public CompilerNameConflictException(string compilerName, string compilerTypeName, string registeredCompilerTypeName, string message) : base(message)
        {
            _compilerName = compilerName;
            _compilerType = compilerTypeName;
            _registeredCompilerType = registeredCompilerTypeName;
        }

        public override string Message
        {
            get
            {
                string s = base.Message;
                if (_compilerName.IsNullOrEmpty() || _compilerType.IsNullOrEmpty() || _registeredCompilerType.IsNullOrEmpty())
                {
                    return s;
                }
                else
                {
                    return string.Format(s, _compilerName, _compilerType, _registeredCompilerType);
                }
            }
        }

        protected CompilerNameConflictException(SerializationInfo info, StreamingContext context) : base(info, context) {
            _compilerName = info.GetString("CompilerName");
            _compilerType = info.GetString("CompilerType");
            _registeredCompilerType = info.GetString("RegisteredCompilerType");
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
            info.AddValue("CompilerType", _compilerType, typeof(string));
            info.AddValue("RegisteredCompilerType", _registeredCompilerType, typeof(string));
        }
    }
}
