using System;
using System.Runtime.Serialization;

namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires a
    /// tag query to perform an API request is called with the invalid query.
    /// </summary>
    [Serializable]
    public class InvalidPostException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPostException"/> class.
        /// </summary>
        public InvalidPostException()
            : base("There is nothing available with these tags or id.")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPostException"/>
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidPostException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPostException"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a <see langword="null"/> reference if no inner exception is specified.</param>
        public InvalidPostException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPostException"/>
        /// class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.</param>
        protected InvalidPostException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
