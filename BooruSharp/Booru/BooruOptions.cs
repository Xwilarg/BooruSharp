using System;

namespace BooruSharp.Booru
{
    [Flags]
    public enum BooruOptions
    {
        none = 0,
        useHttp = 1 << 0, // http instead of https
        // Missing API
        noWiki = 1 << 1,
        noRelated = 1 << 2,
        noComment = 1 << 3,
        noTagById = 1 << 4,
        noPostByMd5 = 1 << 5,
        noPostById = 1 << 6,
        noLastComments = 1 << 7,
        noPostCount = 1 << 8,
        noMultipleRandom = 1 << 9,
        noFavorite = 1 << 10,
        // API using XML instead of JSON
        commentApiXml = 1 << 11,
        tagApiXml = 1 << 12,
        limitOf20000 = 1 << 13, // Limit of 20000 posts per search, used for Gelbooru
        noMoreThan2Tags = 1 << 14,
        noEmptyPostSearch = 1 << 15, // Post functions can't be called without tags
    }
}
