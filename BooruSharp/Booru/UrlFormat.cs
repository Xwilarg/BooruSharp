namespace BooruSharp.Booru
{
    /// <summary>
    /// Indicates the API URL format that will be used to create API requests.
    /// </summary>
    public enum UrlFormat
    {
        /// <summary>
        /// <c>post/index.json</c>
        /// </summary>
        postIndexJson,
        /// <summary>
        /// <c>index.php?page=dapi&amp;s=post&amp;q=index&amp;json=1</c>
        /// </summary>
        indexPhp,
        /// <summary>
        /// <c>posts.json</c>
        /// </summary>
        danbooru,
        /// <summary>
        /// <c>posts</c>
        /// </summary>
        sankaku
    }
}
