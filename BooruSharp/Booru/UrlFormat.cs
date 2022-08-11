namespace BooruSharp.Booru
{
    /// <summary>
    /// Indicates the API URL format that will be used to create API requests.
    /// </summary>
    public enum UrlFormat
    {
        /// <summary>
        /// Indicates that the API doesn't use any particular query
        /// scheme and requires custom logic to handle requests.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that the API uses <c>/post/index.json</c> query scheme.
        /// </summary>
        PostIndexJson,
        /// <summary>
        /// Indicates that the API uses <c>/index.php?page=dapi&amp;s=post&amp;q=index&amp;json=1</c> query scheme.
        /// </summary>
        IndexPhp,
        /// <summary>
        /// Indicates that the API uses <c>/posts.json</c> query scheme.
        /// </summary>
        Danbooru,
        /// <summary>
        /// Indicates that the API uses <c>/posts</c> query scheme.
        /// </summary>
        Sankaku,
        /// <summary>
        /// Indicate that the API uses <c>/api/v1/json/search/images</c> query scheme
        /// </summary>
        Philomena
    }
}
