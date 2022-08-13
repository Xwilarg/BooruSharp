using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Philomena https://github.com/ZizzyDizzyMC/philomena . This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Philomena : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Philomena"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Philomena(string domain, UrlFormat format, BooruOptions options = BooruOptions.None)
            : base(domain, format, options | BooruOptions.NoFavorite | BooruOptions.NoPostByMD5)
        { }

        public abstract string PostsKeyName { get; }

        protected override void AddAuth(HttpRequestMessage message)
        {
            message.RequestUri = new Uri($"{message.RequestUri.AbsoluteUri}&key={Auth.PasswordHash}");
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            var token = (JToken)json;
            return ((JArray)token[PostsKeyName])?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }


        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var token = ((JToken)json)[PostsKeyName];
            return ((JArray)token).Select(GetPostSearchResult).ToArray();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            var tags = elem["tags"].ToObject<string[]>();
            Search.Post.Rating rating;
            if (tags.Contains("explicit")) rating = Search.Post.Rating.Explicit;
            else if (tags.Contains("questionable")) rating = Search.Post.Rating.Questionable;
            else if (tags.Contains("suggestive")) rating = Search.Post.Rating.Safe;
            else if (tags.Contains("safe")) rating = Search.Post.Rating.General;
            else rating = (Search.Post.Rating)(-1); // Some images doesn't have a rating

            var id = elem["id"].Value<int>();

            return new Search.Post.SearchResult(
                new Uri(elem["representations"]["full"].Value<string>()),
                null,
                new Uri($"{BaseUrl}/images/{id}"),
                new Uri(elem["representations"]["thumb"].Value<string>()),
                rating,
                tags,
                null,
                id,
                elem["size"].Value<int>(),
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                null,
                null,
                elem["created_at"].Value<DateTime>(),
                elem["source_url"].Value<string>(),
                elem["score"].Value<int?>(),
                elem["sha512_hash"].Value<string>()
                );
        }
    }
}
