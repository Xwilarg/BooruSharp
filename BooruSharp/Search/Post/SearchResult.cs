using System;

namespace BooruSharp.Search.Post
{
    /// <summary>
    /// Represents a post API search result.
    /// </summary>
    public struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="fileUrl">The URI of the file.</param>
        /// <param name="previewUrl">The URI of the preview image.</param>
        /// <param name="postUrl">The URI of the post.</param>
        /// <param name="rating">The post's rating.</param>
        /// <param name="tags">The array containing all the tags associated with the file.</param>
        /// <param name="id">The ID of the post.</param>
        /// <param name="size">The size of the file, in bytes.</param>
        /// <param name="height">The height of the image, in pixels.</param>
        /// <param name="width">The width of the image, in pixels.</param>
        /// <param name="previewHeight">The height of the preview image, in pixels.</param>
        /// <param name="previewWidth">The width of the preview image, in pixels.</param>
        /// <param name="creation">The creation date of the post.</param>
        /// <param name="source">The original source of the file.</param>
        /// <param name="score">The score of the post.</param>
        /// <param name="md5">The MD5 hash of the file.</param>
        public SearchResult(Uri fileUrl, Uri previewUrl, Uri postUrl, Rating rating, string[] tags, int id,
                            int? size, int height, int width, int? previewHeight, int? previewWidth, DateTime? creation, string source, int? score, string md5)
        {
            this.fileUrl = fileUrl;
            this.previewUrl = previewUrl;
            this.postUrl = postUrl;
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
            this.md5 = md5;
        }

        /// <summary>
        /// Gets the URI of the file.
        /// </summary>
        public readonly Uri fileUrl;

        /// <summary>
        /// Gets the URI of the preview image.
        /// </summary>
        public readonly Uri previewUrl;

        /// <summary>
        /// Gets the URI of the post.
        /// </summary>
        public readonly Uri postUrl;

        /// <summary>
        /// Gets the post's rating.
        /// </summary>
        public readonly Rating rating;

        /// <summary>
        /// Gets the array containing all the tags associated with the file.
        /// </summary>
        public readonly string[] tags;

        /// <summary>
        /// Gets the ID of the post.
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Gets the size of the file, in bytes, or
        /// <see langword="null"/> if file size is unknown.
        /// </summary>
        public readonly int? size;

        /// <summary>
        /// Gets the height of the image, in pixels.
        /// </summary>
        public readonly int height;

        /// <summary>
        /// Gets the width of the image, in pixels.
        /// </summary>
        public readonly int width;

        /// <summary>
        /// Gets the height of the preview image, in pixels,
        /// or <see langword="null"/> if the height is unknown.
        /// </summary>
        public readonly int? previewHeight;

        /// <summary>
        /// Gets the width of the preview image, in pixels,
        /// or <see langword="null"/> if the width is unknown.
        /// </summary>
        public readonly int? previewWidth;

        /// <summary>
        /// Gets the creation date of the post, or
        /// <see langword="null"/> if the date is unknown.
        /// </summary>
        public readonly DateTime? creation;

        /// <summary>
        /// Gets the original source of the file.
        /// </summary>
        public readonly string source;

        /// <summary>
        /// Gets the score of the post, or
        /// <see langword="null"/> if the score is unknown.
        /// </summary>
        public readonly int? score;

        /// <summary>
        /// Gets the MD5 hash of the file, represented as
        /// a sequence of 32 hexadecimal lowercase digits.
        /// </summary>
        public readonly string md5;
    }
}
