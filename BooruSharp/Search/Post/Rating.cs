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
        /// Indicates that post cannot be considered either questionable or explicit.
        /// Note that safe does not mean safe for work and may still include "sexy" content.
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
