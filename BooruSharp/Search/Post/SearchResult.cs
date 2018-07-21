using System;

namespace BooruSharp.Search.Post
{
    public struct SearchResult
    {
        public SearchResult(string fileUrl, string previewUrl, Rating rating, string[] tags, uint id,
                            uint? size, uint height, uint width, uint previewHeight, uint previewWidth, DateTime creation, string sourceUrl)
        {
            this.fileUrl = fileUrl;
            this.previewUrl = previewUrl;
            this.rating = rating;
            this.tags = tags;
            this.id = id;
            this.size = size;
            this.height = height;
            this.width = width;
            this.previewHeight = previewHeight;
            this.previewWidth = previewWidth;
            this.creation = creation;
            this.sourceUrl = sourceUrl;
        }
        public readonly string fileUrl;
        public readonly string previewUrl;
        public readonly Rating rating;
        public readonly string[] tags;
        public readonly uint id;
        public readonly uint? size;
        public readonly uint height;
        public readonly uint width;
        public readonly uint previewHeight;
        public readonly uint previewWidth;
        public readonly DateTime creation;
        public readonly string sourceUrl;
    }
}
