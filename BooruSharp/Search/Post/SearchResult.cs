using System;

namespace BooruSharp.Search.Post
{
    public struct SearchResult
    {
        public SearchResult(Uri fileUrl, Uri previewUrl, Rating rating, string[] tags, int id,
                            int? size, int height, int width, int? previewHeight, int? previewWidth, DateTime creation, string source, int score)
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
            this.source = source;
            this.score = score;
        }

        public readonly Uri fileUrl;
        public readonly Uri previewUrl;
        public readonly Rating rating;
        public readonly string[] tags;
        public readonly int id;
        public readonly int? size;
        public readonly int height;
        public readonly int width;
        public readonly int? previewHeight;
        public readonly int? previewWidth;
        public readonly DateTime creation;
        public readonly string source;
        public readonly int score;
    }
}
