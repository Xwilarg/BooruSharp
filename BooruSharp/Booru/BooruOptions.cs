namespace BooruSharp.Booru
{
    public enum BooruOptions
    {
        useHttp, // http instead of https
        // Missing API
        noWiki,
        noRelated,
        noComment,
        noTagById,
        noPostByMd5,
        noPostById,
        noLastComments,
        noPostCount,
        noMultipleRandom,
        noFavorite,
        // API using XML instead of JSON
        commentApiXml,
        tagApiXml,
        limitOf20000, // Limit of 20000 posts per search, used for Gelbooru
        noMoreThan2Tags
    }
}
