using BooruSharp.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Moebooru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Moebooru : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Moebooru"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Moebooru(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.PostIndexJson, options | BooruOptions.NoPostByMD5 | BooruOptions.NoPostByID
                  | BooruOptions.NoFavorite)
        { }

        private protected override JToken ParseFirstPostSearchResult(JToken token)
        {
            JArray array = token as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken token)
        {
            int id = token["id"].Value<int>();

            return new Search.Post.SearchResult(
                new Uri(token["file_url"].Value<string>()),
                new Uri(token["preview_url"].Value<string>()),
                new Uri(BaseUrl + "post/show/" + id),
                RatingUtils.Parse(token["rating"].Value<string>()),
                token["tags"].Value<string>().Split(' '),
                id,
                token["file_size"].Value<int>(),
                token["height"].Value<int>(),
                token["width"].Value<int>(),
                token["preview_height"].Value<int>(),
                token["preview_width"].Value<int>(),
                _unixTime.AddSeconds(token["created_at"].Value<int>()),
                token["source"].Value<string>(),
                token["score"].Value<int>(),
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
            return new Search.Comment.SearchResult(
                token["id"].Value<int>(),
                token["post_id"].Value<int>(),
                token["creator_id"].Value<int?>(),
                token["created_at"].Value<DateTime>(),
                token["creator"].Value<string>(),
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
                (Search.Tag.TagType)token["type"].Value<int>(),
                token["count"].Value<int>()
                );
        }

        private protected override Search.Related.SearchResult GetRelatedSearchResult(JToken token)
        {
            var elem = (JArray)token;
            return new Search.Related.SearchResult(
                elem[0].Value<string>(),
                elem[1].Value<int>()
                );
        }
    }
}
