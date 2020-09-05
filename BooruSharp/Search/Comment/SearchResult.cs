using System;

namespace BooruSharp.Search.Comment
{
    /// <summary>
    /// Represents a comment API search result.
    /// </summary>
    public struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct. 
        /// </summary>
        /// <param name="commentId">The ID of the comment.</param>
        /// <param name="postId">The ID of the image associated with the comment</param>
        /// <param name="authorId">The ID of the author of the comment, or
        /// <see langword="null"/> if the author is anonymous.</param>
        /// <param name="creation">The date when the comment was posted.</param>
        /// <param name="authorName">The name of the author of the comment.</param>
        /// <param name="body">The comment's message.</param>
        public SearchResult(int commentId, int postId, int? authorId, DateTime creation, string authorName, string body)
        {
            this.commentId = commentId;
            this.postId = postId;
            this.authorId = authorId;
            this.creation = creation;
            this.authorName = authorName;
            this.body = body;
        }

        /// <summary>
        /// Gets the ID of the comment.
        /// </summary>
        public readonly int commentId;

        /// <summary>
        /// Gets the ID of the image associated with the comment.
        /// </summary>
        public readonly int postId;

        /// <summary>
        /// Gets the ID of the author of the comment, or
        /// <see langword="null"/> if the author is anonymous.
        /// </summary>
        public readonly int? authorId;

        /// <summary>
        /// Gets the date when the comment was posted.
        /// </summary>
        public readonly DateTime creation;

        /// <summary>
        /// Gets the name of the author of the comment.
        /// </summary>
        public readonly string authorName;

        /// <summary>
        /// Gets the comment's message.
        /// </summary>
        public readonly string body;
    }
}
