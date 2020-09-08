using System;
using System.Runtime.Serialization;

namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires
    /// certain booru features (such as tag wiki API) is called.
    /// </summary>
    [Serializable]
    public class FeatureUnavailable : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureUnavailable"/> class.
        /// </summary>
        public FeatureUnavailable()
            : base("This feature isn't available for this Booru.")
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureUnavailable"/>
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FeatureUnavailable(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureUnavailable"/> class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a <see langword="null"/> reference if no inner exception is specified.</param>
        public FeatureUnavailable(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureUnavailable"/>
        /// class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.</param>
        protected FeatureUnavailable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
