using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Represents options for creating an <see cref="ABooru"/> object.
    /// <para>This enumeration has a <see cref="FlagsAttribute"/> attribute
    /// that allows a bitwise combination of its member values.</para>
    /// </summary>
    [Flags]
    public enum BooruOptions
    {
        /// <summary>
        /// Indicates that no additional options should be used when creating an <see cref="ABooru"/> object.
        /// </summary>
        none = 0,
        /// <summary>
        /// Indicates that http:// URI scheme should be used instead of https:// URI scheme.
        /// </summary>
        useHttp = 1 << 0,
        /// <summary>
        /// Indicates that wiki API is not available.
        /// </summary>
        noWiki = 1 << 1,
        /// <summary>
        /// Indicates that related post API is not available.
        /// </summary>
        noRelated = 1 << 2,
        /// <summary>
        /// Indicates that comments API is not available.
        /// </summary>
        noComment = 1 << 3,
        /// <summary>
        /// Indicates that searching for tags by ID is not available.
        /// </summary>
        noTagById = 1 << 4,
        /// <summary>
        /// Indicates that searching for posts by MD5 hash is not available.
        /// </summary>
        noPostByMd5 = 1 << 5,
        /// <summary>
        /// Indicates that searching for posts by ID is not available.
        /// </summary>
        noPostById = 1 << 6,
        /// <summary>
        /// Indicates that latest comments API is not available.
        /// </summary>
        noLastComments = 1 << 7,
        /// <summary>
        /// Indicates that total post count API is not available.
        /// </summary>
        noPostCount = 1 << 8,
        /// <summary>
        /// Indicates that retrieving multiple random posts API is not available.
        /// </summary>
        noMultipleRandom = 1 << 9,
        /// <summary>
        /// Indicates that favoriting API is not available.
        /// </summary>
        noFavorite = 1 << 10,
        /// <summary>
        /// Indicates that comments API respond with XML instead of JSON.
        /// </summary>
        commentApiXml = 1 << 11,
        /// <summary>
        /// Indicates that tags API respond with XML instead of JSON.
        /// </summary>
        tagApiXml = 1 << 12,
        /// <summary>
        /// Indicates that maximum limit of posts per search is increased.
        /// </summary>
        limitOf20000 = 1 << 13,
        /// <summary>
        /// Indicates that at most 2 tags can be used for post searching.
        /// </summary>
        noMoreThan2Tags = 1 << 14,
        /// <summary>
        /// Indicates that search parameter must be provided in order for post search functions to work.
        /// </summary>
        noEmptyPostSearch = 1 << 15,
    }
}
