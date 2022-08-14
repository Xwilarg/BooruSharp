namespace BooruSharp.Search.Tag
{
    /// <summary>
    /// Indicates the type of the tag.
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// Indicate a rating, only for Derpibooru, Ponybooru and Twibooru
        /// </summary>
        Rating = -4,
        /// <summary>
        /// Indicates the species tag (Not available in all boorus).
        /// </summary>
        Species,
        /// <summary>
        /// Indicates the invalid tag (Not available in all boorus).
        /// </summary>
        Invalid,
        /// <summary>
        /// Indicates the lore tag (Not available in all boorus).
        /// </summary>
        Lore,
        /// <summary>
        /// Indicates the common tag.
        /// </summary>
        Trivia = 0,
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
