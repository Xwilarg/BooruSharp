using System;
using System.Runtime.Serialization;

namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires a
    /// tag query to perform an API request is supplied too many tags.
    /// </summary>
    [Serializable]
    public class TooManyTags : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyTags"/> class.
        /// </summary>
        public TooManyTags(int maxTagCount)
            : base($"You can't have more than {maxTagCount} tags for a search with this booru.")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyTags"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a <see langword="null"/> reference if no inner exception is specified.</param>
        public TooManyTags(string message, Exception innerException) 
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyTags"/>
        /// class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.</param>
        protected TooManyTags(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
