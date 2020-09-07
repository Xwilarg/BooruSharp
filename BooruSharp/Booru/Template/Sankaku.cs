using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Sankaku. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Sankaku : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sankaku"/> template class.
        /// </summary>
        /// <param name="url">The base URL to use. This should be a host name.</param>
        /// <param name="options">The options to use. Use | (bitwise OR) operator to combine multiple options.</param>
        protected Sankaku(string url, BooruOptions options = BooruOptions.None)
            : base(url, UrlFormat.Sankaku, options | BooruOptions.NoRelated | BooruOptions.NoPostByMD5 | BooruOptions.NoPostByID
                  | BooruOptions.NoPostCount | BooruOptions.NoFavorite | BooruOptions.NoTagByID)
        { }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            string[] tags = (from tag in (JArray)elem["tags"]
                             select tag["name"].Value<string>()).ToArray();

            return new Search.Post.SearchResult(
                    new Uri(elem["file_url"].Value<string>()),
                    new Uri(elem["preview_url"].Value<string>()),
                    new Uri(_baseUrl.Replace("capi-v2", "beta") + "/post/show/" + elem["id"].Value<int>()),
                    GetRating(elem["rating"].Value<string>()[0]),
                    tags,
                    elem["id"].Value<int>(),
                    elem["file_size"].Value<int>(),
                    elem["height"].Value<int>(),
                    elem["width"].Value<int>(),
                    elem["preview_height"].Value<int>(),
                    elem["preview_width"].Value<int>(),
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(elem["created_at"]["s"].Value<int>()),
                    elem["source"].Value<string>(),
                    elem["total_score"].Value<int>(),
                    elem["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["author"]["id"].Value<int?>(),
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(elem["created_at"]["s"].Value<int>()),
                elem["author"]["name"].Value<string>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(elem["created_at"]["s"].Value<int>()),
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(elem["updated_at"]["s"].Value<int>()),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json) // TODO: Fix TagType values
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available
    }
}
