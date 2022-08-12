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
        protected Philomena(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Philomena, options | BooruOptions.NoPostCount | BooruOptions.NoFavorite | BooruOptions.NoPostByMD5)
        { }

        protected override void AddAuth(HttpRequestMessage message)
        {
            message.RequestUri = new Uri($"{message.RequestUri.AbsoluteUri}&key={Auth.PasswordHash}");
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            var token = (JToken)json;
            return ((JArray)token["images"])?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }


        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var token = ((JToken)json)["images"];
            return ((JArray)token).Select(GetPostSearchResult).ToArray();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            var tags = elem["tags"].ToObject<string[]>();
            Search.Post.Rating rating;
            if (tags.Contains("explicit")) rating = Search.Post.Rating.Explicit;
            else if (tags.Contains("questionable")) rating = Search.Post.Rating.Questionable;
            else if (tags.Contains("sensitive")) rating = Search.Post.Rating.Safe;
            else if (tags.Contains("safe")) rating = Search.Post.Rating.General;
            else throw new ArgumentException("No image rating found", nameof(elem));

            var sourceUrl = elem["source_url"].Value<string>();

            return new Search.Post.SearchResult(
                new Uri(elem["representations"]["full"].Value<string>()),
                null,
                sourceUrl == null ? null : new Uri(sourceUrl),
                new Uri(elem["representations"]["thumb"].Value<string>()),
                rating,
                tags,
                null,
                elem["id"].Value<int>(),
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
