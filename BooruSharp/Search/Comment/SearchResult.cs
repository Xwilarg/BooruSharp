using System;

namespace BooruSharp.Search.Comment
{
    public struct SearchResult
    {
        public SearchResult(uint commentId, uint postId, uint authorId, DateTime creation, string authorName, string body)
        {
            this.commentId = commentId;
            this.postId = postId;
            this.authorId = authorId;
            this.creation = creation;
            this.authorName = authorName;
            this.body = body;
        }
        public readonly uint commentId;
        public readonly uint postId;
        public readonly uint authorId;
        public readonly DateTime creation;
        public readonly string authorName;
        public readonly string body;
    }
}
