using System;
using System.Collections.Generic;
using System.Text;

namespace JacobDixon.AspNetCore.LiveWebTasks.Exceptions
{
    /// <summary>
    /// The exception to throw if an array is empty.
    /// </summary>
    [Serializable]
    public class EmptyArrayException : Exception
    {
        /// <summary>
        /// Constructs an empty exception.
        /// </summary>
        public EmptyArrayException() { }

        /// <summary>
        /// Constructs an exception with the message provided.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        public EmptyArrayException(string message) : base(message) { }

        /// <summary>
        /// Constructs an exception with a message and inner exception.
        /// </summary>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="inner">The inner exception.</param>
        public EmptyArrayException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected EmptyArrayException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
