using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

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
            : base(domain, UrlFormat.Philomena, options | BooruOptions.NoFavorite | BooruOptions.NoPostByMD5 | BooruOptions.NoPostByID
                  | BooruOptions.NoLastComments | BooruOptions.NoWiki | BooruOptions.NoRelated)
        { }

        /// <summary>
        /// ID used to set filter and have access to as many posts as possible
        /// </summary>
        protected abstract int FilterID { get; }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            var uriBuilder = new UriBuilder(message.RequestUri.AbsoluteUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["filter_id"] = $"{FilterID}";
            if (Auth != null)
            {
                query["key"] = Auth.PasswordHash;
            }
            uriBuilder.Query = query.ToString();
            message.RequestUri = new Uri(uriBuilder.ToString());
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            var token = (JToken)json;
            if (token["images"] is JArray arr)
            {
                return arr?.FirstOrDefault() ?? throw new Search.InvalidTags();
            }
            return token["image"];
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
            else if (tags.Contains("suggestive")) rating = Search.Post.Rating.Safe;
            else if (tags.Contains("safe")) rating = Search.Post.Rating.General;
            else rating = (Search.Post.Rating)(-1); // Some images doesn't have a rating

            var id = elem["id"].Value<int>();

            return new Search.Post.SearchResult(
                new Uri(elem["representations"]["full"].Value<string>()),
                null,
                new Uri($"{BaseUrl}images/{id}"),
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

        private protected override async Task<IEnumerable> GetTagEnumerableSearchResultAsync(Uri url)
        {
            return (JArray)JsonConvert.DeserializeObject<JToken>(await GetJsonAsync(url))["tags"];
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var token = (JToken)json;
            return new Search.Tag.SearchResult(
                token["id"].Value<int>(),
                token["name"].Value<string>(),
                GetTagType(token["category"].Value<string>()),
                token["images"].Value<int>()
                );
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var token = (JToken)json;
            return new Search.Comment.SearchResult(
                token["id"].Value<int>(),
                token["image_id"].Value<int>(),
                token["user_id"].Value<int?>(),
                token["created_at"].Value<DateTime>(),
                token["author"].Value<string>(),
                token["body"].Value<string>()
                );
        }

        private Search.Tag.TagType GetTagType(string typeName)
        {
            switch (typeName) // TODO: https://ponybooru.org/tags
            {
                case "rating": return Search.Tag.TagType.Rating;
                case "species": return Search.Tag.TagType.Species;
                case null: return Search.Tag.TagType.Trivia;
                case "character": return Search.Tag.TagType.Character;
                default: return (Search.Tag.TagType)6;
            }
        }
    }
}
