using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on E621. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class E621 : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="E621"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected E621(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Danbooru, options | BooruOptions.NoWiki | BooruOptions.NoRelated | BooruOptions.NoComment 
                  | BooruOptions.NoTagByID | BooruOptions.NoPostCount | BooruOptions.NoFavorite)
        { }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JObject jObject = (JObject)json;

            JToken token = jObject["posts"] is JArray posts
                ? posts.FirstOrDefault()
                : jObject["post"];

            return token ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            // TODO: Check others tags
            string[] categories =
            {
                "general",
                "species",
                "character",
                "copyright",
                "artist",
                "meta",
            };

            var fileToken = elem["file"];
            var previewToken = elem["preview"];

            string url = fileToken["url"].Value<string>();
            string previewUrl = previewToken["url"].Value<string>();
            int id = elem["id"].Value<int>();
            string[] tags = categories
                .SelectMany(category => elem["tags"][category].ToObject<string[]>())
                .ToArray();

            return new Search.Post.SearchResult(
                    url != null ? new Uri(url) : null,
                    previewUrl != null ? new Uri(previewUrl) : null,
                    new Uri(BaseUrl + "posts/" + id),
                    null,
                    GetRating(elem["rating"].Value<string>()[0]),
                    tags,
                    null,
                    id,
                    fileToken["size"].Value<int>(),
                    fileToken["height"].Value<int>(),
                    fileToken["width"].Value<int>(),
                    previewToken["height"].Value<int>(),
                    previewToken["width"].Value<int>(),
                    elem["created_at"].Value<DateTime>(),
                    elem["sources"].FirstOrDefault()?.Value<string>(),
                    elem["score"]["total"].Value<int>(),
                    fileToken["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            JObject obj = (JObject)json;

            if (obj["posts"] is JArray array)
                return array.Select(GetPostSearchResult).ToArray();
            else if (obj["post"] is JToken token)
                return new[] { GetPostSearchResult(token) };
            else
                return Array.Empty<Search.Post.SearchResult>();
        }

        // GetCommentSearchResult not available

        // GetWikiSearchResult not available

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

        // GetRelatedSearchResult not available // TODO: Available with credentials?
    }
}
