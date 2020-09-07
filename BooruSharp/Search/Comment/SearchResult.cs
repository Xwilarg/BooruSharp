using System;

namespace BooruSharp.Search.Comment
{
    /// <summary>
    /// Represents a comment API search result.
    /// </summary>
    public readonly struct SearchResult
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
            CommentID = commentId;
            PostID = postId;
            AuthorID = authorId;
            Creation = creation;
            AuthorName = authorName;
            Body = body;
        }

        /// <summary>
        /// Gets the ID of the comment.
        /// </summary>
        public int CommentID { get; }

        /// <summary>
        /// Gets the ID of the image associated with the comment.
        /// </summary>
        public int PostID { get; }

        /// <summary>
        /// Gets the ID of the author of the comment, or
        /// <see langword="null"/> if the author is anonymous.
        /// </summary>
        public int? AuthorID { get; }

        /// <summary>
        /// Gets the date when the comment was posted.
        /// </summary>
        public DateTime Creation { get; }

        /// <summary>
        /// Gets the name of the author of the comment.
        /// </summary>
        public string AuthorName { get; }

        /// <summary>
        /// Gets the comment's message.
        /// </summary>
        public string Body { get; }
    }
}
