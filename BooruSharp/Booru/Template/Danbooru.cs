using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Danbooru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Danbooru : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Danbooru"/> template class.
        /// </summary>
        /// <param name="url">The base URL to use. This should be a host name.</param>
        /// <param name="options">The collection of option values.</param>
        [Obsolete(_deprecationMessage)]
        public Danbooru(string url, params BooruOptions[] options) : this(url, MergeOptions(options))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Danbooru"/> template class.
        /// </summary>
        /// <param name="url">The base URL to use. This should be a host name.</param>
        /// <param name="options">The options to use. Use | (bitwise OR) operator to combine multiple options.</param>
        public Danbooru(string url, BooruOptions options = BooruOptions.none) : base(url, UrlFormat.danbooru, options | BooruOptions.noLastComments | BooruOptions.noPostCount | BooruOptions.noFavorite)
        { }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JToken token = json is JArray array
                ? array.FirstOrDefault()
                : json as JToken;

            return token ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            var url = elem["file_url"];
            var previewUrl = elem["preview_file_url"];
            var id = elem["id"];
            var md5 = elem["md5"];
            return new Search.Post.SearchResult(
                    url != null ? new Uri(url.Value<string>()) : null,
                    previewUrl != null ? new Uri(previewUrl.Value<string>()) : null,
                    id != null ? new Uri(_baseUrl + "/posts/" + id.Value<int>()) : null,
                    GetRating(elem["rating"].Value<string>()[0]),
                    elem["tag_string"].Value<string>().Split(' '),
                    id?.Value<int>() ?? 0,
                    elem["file_size"].Value<int>(),
                    elem["image_height"].Value<int>(),
                    elem["image_width"].Value<int>(),
                    null,
                    null,
                    elem["created_at"].Value<DateTime>(),
                    elem["source"].Value<string>(),
                    elem["score"].Value<int>(),
                    md5?.Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            if (json is JArray array)
                return array.Select(GetPostSearchResult).ToArray();
            else if (json is JToken token)
                return new[] { GetPostSearchResult(token["post"]) };
            else
                return Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"].Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                elem["creator_name"]?.Value<string>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                elem["created_at"].Value<DateTime>(),
                elem["updated_at"].Value<DateTime>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["category"].Value<int>(),
                elem["post_count"].Value<int>()
                );
        }

        private protected override Search.Related.SearchResult GetRelatedSearchResult(object json)
        {
            var elem = (JArray)json;
            return new Search.Related.SearchResult(
                elem[0].Value<string>(),
                elem[1].Value<int>()
                );
        }
    }
}
