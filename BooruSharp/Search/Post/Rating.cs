namespace BooruSharp.Search.Post
{
    // Rating descriptions are taken from:
    // https://danbooru.donmai.us/wiki_pages/howto%3Arate
    /// <summary>
    /// Represents a level of explicit content of the post.
    /// </summary>
    public enum Rating
    {
        /// <summary>
        /// Indicates that post contains G-rated, completely safe for work content.
        /// </summary>
        General,
        /// <summary>
        /// Indicates that post contains something not completely safe for work, or not completely safe to view in front of others.
        /// This is called 'Sensitive' in the new booru version
        /// </summary>
        Safe,
        /// <summary>
        /// Indicates that post may contain some non-explicit nudity or sexual content, but isn't quite pornographic.
        /// </summary>
        Questionable,
        /// <summary>
        /// Indicates that post contains explicit sex, gratuitously exposed genitals, or it is otherwise pornographic.
        /// </summary>
        Explicit
    }
}
