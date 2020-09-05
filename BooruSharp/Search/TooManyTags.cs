namespace BooruSharp.Search
{
    /// <summary>
    /// Represents errors that occur whenever a method that requires a
    /// tag query to perform an API request is supplied too many tags.
    /// </summary>
    public class TooManyTags : System.ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyTags"/> class.
        /// </summary>
        public TooManyTags() : base("You can't have more than 2 tags for a search with this booru")
        { }
    }
}
