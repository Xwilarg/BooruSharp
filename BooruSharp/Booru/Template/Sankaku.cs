using BooruSharp.Utils;
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
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Sankaku(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Sankaku, options | BooruOptions.NoRelated | BooruOptions.NoPostByMD5 | BooruOptions.NoPostByID
                  | BooruOptions.NoPostCount | BooruOptions.NoFavorite | BooruOptions.NoTagByID)
        { }

        private protected override JToken ParseFirstPostSearchResult(JToken token)
        {
            JArray array = token as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken token)
        {
            int id = token["id"].Value<int>();

            var postUriBuilder = new UriBuilder(BaseUrl)
            {
                Host = BaseUrl.Host.Replace("capi-v2", "beta"),
                Path = $"/post/show/{id}",
            };

            string[] tags = (from tag in (JArray)token["tags"]
                             select tag["name"].Value<string>()).ToArray();

            return new Search.Post.SearchResult(
                new Uri(token["file_url"].Value<string>()),
                new Uri(token["preview_url"].Value<string>()),
                postUriBuilder.Uri,
                RatingUtils.Parse(token["rating"].Value<string>()),
                tags,
                id,
                token["file_size"].Value<int>(),
                token["height"].Value<int>(),
                token["width"].Value<int>(),
                token["preview_height"].Value<int>(),
                token["preview_width"].Value<int>(),
                _unixTime.AddSeconds(token["created_at"]["s"].Value<int>()),
                token["source"].Value<string>(),
                token["total_score"].Value<int>(),
                token["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(JToken token)
        {
            return token is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(JToken token)
        {
            var authorToken = token["author"];

            return new Search.Comment.SearchResult(
                token["id"].Value<int>(),
                token["post_id"].Value<int>(),
                authorToken["id"].Value<int?>(),
                _unixTime.AddSeconds(token["created_at"]["s"].Value<int>()),
                authorToken["name"].Value<string>(),
                token["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(JToken token)
        {
            return new Search.Wiki.SearchResult(
                token["id"].Value<int>(),
                token["title"].Value<string>(),
                _unixTime.AddSeconds(token["created_at"]["s"].Value<int>()),
                _unixTime.AddSeconds(token["updated_at"]["s"].Value<int>()),
                token["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(JToken token) // TODO: Fix TagType values
        {
            return new Search.Tag.SearchResult(
                token["id"].Value<int>(),
                token["name"].Value<string>(),
                (Search.Tag.TagType)token["type"].Value<int>(),
                token["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available
    }
}
