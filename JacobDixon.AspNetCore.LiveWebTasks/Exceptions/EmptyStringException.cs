using System;
using System.Collections.Generic;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{
    /// <summary>
    /// The exception to throw if a string is empty.
    /// </summary>
    [Serializable]
    public class EmptyStringException : Exception
    {
        /// <summary>
        /// Constructs an empty exception.
        /// </summary>
        public EmptyStringException() { }

        /// <inheritdoc/>
        public EmptyStringException(string message) : base(message) { }

        /// <inheritdoc/>
        public EmptyStringException(string message, Exception inner) : base(message, inner) { }

        /// <inheritdoc/>
        protected EmptyStringException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
