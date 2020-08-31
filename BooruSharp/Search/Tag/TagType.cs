namespace BooruSharp.Search.Tag
{
    /// <summary>
    /// Indicates the type of the tag.
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// Indicates the common tag.
        /// </summary>
        Trivia,
        /// <summary>
        /// Indicates the artist tag.
        /// </summary>
        Artist,
        /// <summary>
        /// Indicates the copyright tag.
        /// </summary>
        Copyright = 3,
        /// <summary>
        /// Indicates the character tag.
        /// </summary>
        Character,
        /// <summary>
        /// Indicates the metadata tag.
        /// </summary>
        Metadata
    }
}
