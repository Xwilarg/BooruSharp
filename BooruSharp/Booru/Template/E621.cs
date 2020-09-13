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
                  | BooruOptions.NoTagByID | BooruOptions.NoPostByID | BooruOptions.NoPostCount | BooruOptions.NoFavorite)
        { }

        private protected override JToken ParseFirstPostSearchResult(JToken token)
        {
            var post = token["posts"] is JArray posts ? posts.FirstOrDefault() : token["post"];
            return post ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken token)
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

            var fileToken = token["file"];
            var previewToken = token["preview"];

            string url = fileToken["url"].Value<string>();
            string previewUrl = previewToken["url"].Value<string>();
            int id = token["id"].Value<int>();
            string[] tags = categories
                .SelectMany(category => token["tags"][category].ToObject<string[]>())
                .ToArray();

            return new Search.Post.SearchResult(
                    url != null ? new Uri(url) : null,
                    previewUrl != null ? new Uri(previewUrl) : null,
                    new Uri(BaseUrl + "posts/" + id),
                    GetRating(token["rating"].Value<string>()[0]),
                    tags,
                    id,
                    fileToken["size"].Value<int>(),
                    fileToken["height"].Value<int>(),
                    fileToken["width"].Value<int>(),
                    previewToken["height"].Value<int>(),
                    previewToken["width"].Value<int>(),
                    token["created_at"].Value<DateTime>(),
                    token["sources"].FirstOrDefault()?.Value<string>(),
                    token["score"]["total"].Value<int>(),
                    fileToken["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(JToken token)
        {
            if (token["posts"] is JArray array)
                return array.Select(GetPostSearchResult).ToArray();
            else if (token["post"] is JToken post)
                return new[] { GetPostSearchResult(post) };
            else
                return Array.Empty<Search.Post.SearchResult>();
        }

        // GetCommentSearchResult not available

        // GetWikiSearchResult not available

        private protected override Search.Tag.SearchResult GetTagSearchResult(JToken token)
        {
            return new Search.Tag.SearchResult(
                token["id"].Value<int>(),
                token["name"].Value<string>(),
                (Search.Tag.TagType)token["category"].Value<int>(),
                token["post_count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available // TODO: Available with credentials?
    }
}
