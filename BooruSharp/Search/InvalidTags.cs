namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires a
    /// tag query to perform an API request is called with the invalid query.
    /// </summary>
    public class InvalidTags : System.ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTags"/> class.
        /// </summary>
        public InvalidTags()
            : base("There is nothing available with these tags")
        { }
    }
}
