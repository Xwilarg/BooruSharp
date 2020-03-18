using System;

namespace BooruSharp.Search.Comment
{
    public struct SearchResult
    {
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
        /// Id of the comment
        /// </summary>
        public readonly int commentId;

        /// <summary>
        /// Id of the image associated with the comment
        /// </summary>
        public readonly int postId;

        /// <summary>
        /// Id of the author of the comment, is null if the author is anonymous
        /// </summary>
        public readonly int? authorId;

        /// <summary>
        /// When the comment was posted
        /// </summary>
        public readonly DateTime creation;

        /// <summary>
        /// Name of the author of the comment
        /// </summary>
        public readonly string authorName;

        /// <summary>
        /// Comment's message
        /// </summary>
        public readonly string body;
    }
}
