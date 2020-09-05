namespace BooruSharp.Search.Tag
{
    /// <summary>
    /// Represents a tag API search result.
    /// </summary>
    public struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="id">The ID of the tag.</param>
        /// <param name="name">The name of the tag.</param>
        /// <param name="type">The type of the tag.</param>
        /// <param name="count">The number of occurences of the tag.</param>
        public SearchResult(int id, string name, TagType type, int count)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.count = count;
        }

        /// <summary>
        /// Gets the ID of the tag.
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public readonly TagType type;

        /// <summary>
        /// Gets the number of occurences of the tag.
        /// </summary>
        public readonly int count;
    }
}
