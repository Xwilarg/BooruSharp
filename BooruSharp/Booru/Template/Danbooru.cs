using BooruSharp.Utils;
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
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Danbooru(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Danbooru, options | BooruOptions.NoLastComments | BooruOptions.NoPostCount
                  | BooruOptions.NoFavorite)
        { }

        private protected override JToken ParseFirstPostSearchResult(JToken token)
        {
            JToken post = token is JArray array ? array.FirstOrDefault() : token;
            return post ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken token)
        {
            var url = token["file_url"];
            var previewUrl = token["preview_file_url"];
            var id = token["id"]?.Value<int>();
            var md5 = token["md5"];

            return new Search.Post.SearchResult(
                url != null ? new Uri(url.Value<string>()) : null,
                previewUrl != null ? new Uri(previewUrl.Value<string>()) : null,
                id.HasValue ? new Uri(BaseUrl + "posts/" + id.Value) : null,
                RatingUtils.Parse(token["rating"].Value<string>()),
                token["tag_string"].Value<string>().Split(' '),
                id ?? 0,
                token["file_size"].Value<int>(),
                token["image_height"].Value<int>(),
                token["image_width"].Value<int>(),
                null,
                null,
                token["created_at"].Value<DateTime>(),
                token["source"].Value<string>(),
                token["score"].Value<int>(),
                md5?.Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(JToken token)
        {
            if (token is JArray array)
                return array.Select(GetPostSearchResult).ToArray();
            else if (token["post"] is JToken post)
                return new[] { GetPostSearchResult(post) };
            else
                return Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(JToken token)
        {
            return new Search.Comment.SearchResult(
                token["id"].Value<int>(),
                token["post_id"].Value<int>(),
                token["creator_id"].Value<int?>(),
                token["created_at"].Value<DateTime>(),
                token["creator_name"]?.Value<string>(),
                token["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(JToken token)
        {
            return new Search.Wiki.SearchResult(
                token["id"].Value<int>(),
                token["title"].Value<string>(),
                token["created_at"].Value<DateTime>(),
                token["updated_at"].Value<DateTime>(),
                token["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(JToken token)
        {
            return new Search.Tag.SearchResult(
                token["id"].Value<int>(),
                token["name"].Value<string>(),
                (Search.Tag.TagType)token["category"].Value<int>(),
                token["post_count"].Value<int>()
                );
        }

        private protected override Search.Related.SearchResult GetRelatedSearchResult(JToken token)
        {
            var array = (JArray)token;
            return new Search.Related.SearchResult(
                array[0].Value<string>(),
                array[1].Value<int>()
                );
        }
    }
}
