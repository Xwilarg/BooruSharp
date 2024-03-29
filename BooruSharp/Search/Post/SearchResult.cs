﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BooruSharp.Search.Post
{
    /// <summary>
    /// Represents a post API search result.
    /// </summary>
    public readonly struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="fileUrl">The URI of the file.</param>
        /// <param name="previewUrl">The URI of the image thumbnail.</param>
        /// <param name="postUrl">The URI of the post.</param>
        /// <param name="sampleUri">The URI of the sample image. A sample is a lighter version of the main file.</param>
        /// <param name="rating">The post's rating.</param>
        /// <param name="tags">An array containing all the tags associated with the file.</param>
        /// <param name="detailedTags">An array containing detailed information on tags associated with the file.</param>
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
        public SearchResult(
            Uri fileUrl, Uri previewUrl, Uri postUrl, Uri sampleUri, Rating rating, IList<string> tags,
            IList<Tag.SearchResult> detailedTags, int id, int? size, int height, int width, int? previewHeight, int? previewWidth,
            DateTime? creation, string source, int? score, string md5)
        {
            FileUrl = fileUrl;
            PreviewUrl = previewUrl;
            PostUrl = postUrl;
            SampleUri = sampleUri;
            Rating = rating;
            Tags = new ReadOnlyCollection<string>(tags);
            DetailedTags = detailedTags != null ? new ReadOnlyCollection<Tag.SearchResult>(detailedTags) : null;
            ID = id;
            Size = size;
            Height = height;
            Width = width;
            PreviewHeight = previewHeight;
            PreviewWidth = previewWidth;
            Creation = creation;
            Source = source;
            Score = score;
            MD5 = md5;
        }

        /// <summary>
        /// Gets the URI of the file.
        /// </summary>
        public Uri FileUrl { get; }

        /// <summary>
        /// Gets the URI of the preview image.
        /// </summary>
        public Uri PreviewUrl { get; }

        /// <summary>
        /// Gets the URI of the post.
        /// </summary>
        public Uri PostUrl { get; }

        /// <summary>
        /// Gets the URI of the sample image. A sample is a lighter version of the main file.
        /// </summary>
        public Uri SampleUri { get; }

        /// <summary>
        /// Gets the post's rating.
        /// </summary>
        public Rating Rating { get; }

        /// <summary>
        /// Gets a read-only collection containing all the tags associated with the file.
        /// </summary>
        public ReadOnlyCollection<string> Tags { get; }
        
        /// <summary>
        /// Gets a read-only collection containing all the tags associated with the file with additional detail.
        /// </summary>
        public ReadOnlyCollection<Tag.SearchResult> DetailedTags { get; }

        /// <summary>
        /// Gets the ID of the post.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the size of the file, in bytes, or
        /// <see langword="null"/> if file size is unknown.
        /// </summary>
        public int? Size { get; }

        /// <summary>
        /// Gets the height of the image, in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the width of the image, in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the preview image, in pixels,
        /// or <see langword="null"/> if the height is unknown.
        /// </summary>
        public int? PreviewHeight { get; }

        /// <summary>
        /// Gets the width of the preview image, in pixels,
        /// or <see langword="null"/> if the width is unknown.
        /// </summary>
        public int? PreviewWidth { get; }

        /// <summary>
        /// Gets the creation date of the post, or
        /// <see langword="null"/> if the date is unknown.
        /// </summary>
        public DateTime? Creation { get; }

        /// <summary>
        /// Gets the original source of the file.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the score of the post, or
        /// <see langword="null"/> if the score is unknown.
        /// </summary>
        public int? Score { get; }

        /// <summary>
        /// Gets the MD5 hash of the file, represented as
        /// a sequence of 32 hexadecimal lowercase digits.
        /// </summary>
        public string MD5 { get; }
    }
}
