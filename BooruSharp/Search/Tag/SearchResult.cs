namespace BooruSharp.Search.Tag
{
    /// <summary>
    /// Represents a tag API search result.
    /// </summary>
    public readonly struct SearchResult
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
            ID = id;
            Name = name;
            Type = type;
            Count = count;
        }

        /// <summary>
        /// Gets the ID of the tag.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the tag.
        /// </summary>
        public TagType Type { get; }

        /// <summary>
        /// Gets the number of occurences of the tag.
        /// </summary>
        public int Count { get; }
    }
}
