namespace BooruSharp.Search.Related
{
    /// <summary>
    /// Represents a related API search result.
    /// </summary>
    public readonly struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <param name="count">The number of occurences of the tag.</param>
        public SearchResult(string name, int? count)
        {
            Name = name;
            Count = count;
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the number of occurences of the tag, or
        /// <see langword="null"/> if that number is unknown.
        /// </summary>
        public int? Count { get; }
    }
}
