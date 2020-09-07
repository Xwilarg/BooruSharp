namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires a
    /// post ID to perform an API request is called with the invalid ID.
    /// </summary>
    public class InvalidPostId : System.ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPostId"/>
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public InvalidPostId(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPostId"/> class.
        /// </summary>
        public InvalidPostId()
            : base("There is no post with this id")
        { }
    }
}
