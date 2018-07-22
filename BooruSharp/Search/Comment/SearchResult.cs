using System;

namespace BooruSharp.Search.Comment
{
    public struct SearchResult
    {
        public SearchResult(int commentId, int postId, int authorId, DateTime creation, string authorName, string body)
        {
            this.commentId = commentId;
            this.postId = postId;
            this.authorId = authorId;
            this.creation = creation;
            this.authorName = authorName;
            this.body = body;
        }
        public readonly int commentId;
        public readonly int postId;
        public readonly int authorId;
        public readonly DateTime creation;
        public readonly string authorName;
        public readonly string body;
    }
}
