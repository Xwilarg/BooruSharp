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

        /// <summary>
        /// Url of the image
        /// </summary>
        public readonly Uri fileUrl;

        /// <summary>
        /// Preview url of the image
        /// </summary>
        public readonly Uri previewUrl;

        /// <summary>
        /// Is the image safe or not
        /// </summary>
        public readonly Rating rating;

        /// <summary>
        /// All the tags contained in the image
        /// </summary>
        public readonly string[] tags;

        /// <summary>
        /// Id of the image
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Size in octets of the image
        /// </summary>
        public readonly int? size;

        /// <summary>
        /// Height in pixels of the image
        /// </summary>
        public readonly int height;

        /// <summary>
        /// Width in pixels of the image
        /// </summary>
        public readonly int width;

        /// <summary>
        /// Height in pixels of the preview image
        /// </summary>
        public readonly int? previewHeight;

        /// <summary>
        /// Width in pixels of the preview image
        /// </summary>
        public readonly int? previewWidth;

        /// <summary>
        /// When was the post created
        /// </summary>
        public readonly DateTime creation;

        /// <summary>
        /// Where the image is coming from
        /// </summary>
        public readonly string source;

        /// <summary>
        /// Score of the image
        /// </summary>
        public readonly int score;
    }
}
