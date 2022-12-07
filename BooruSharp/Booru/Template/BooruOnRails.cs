using BooruSharp.Booru.Parsing;
using BooruSharp.Search.Post;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Booru-on-rails https://github.com/derpibooru/booru-on-rails . This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class BooruOnRails : ABooru<EmptyParsing, BooruOnRails.SearchResult, EmptyParsing, EmptyParsing, EmptyParsing>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Philomena"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        protected BooruOnRails(string domain)
            : base(domain)
        { }

        protected override Uri CreateQueryString(string query, string squery = "index")
        {
            return new($"{BaseUrl}api/v3/search/{query}s");
        }

        protected override Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            if (!tags.Any())
            {
                return Task.FromResult(CreateUrl(_imageUrl, "per_page=1", "q=id.gte:0", "sf=random"));
            }
            return Task.FromResult(CreateUrl(_imageUrl, "per_page=1", "q=" + string.Join(",", tags.Select(Uri.EscapeDataString)).ToLowerInvariant(), "sf=random"));
        }

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

        protected override async Task<PostSearchResult> GetPostFromUriAsync(Uri url)
        {
            return GetPostSearchResult(JsonSerializer.Deserialize<PostContainer>(await GetJsonAsync(url), new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            }).Posts.FirstOrDefault());
        }

        private protected override PostSearchResult GetPostSearchResult(SearchResult parsingData)
        {
            Rating rating;
            if (parsingData.Tags.Contains("explicit")) rating = Rating.Explicit;
            else if (parsingData.Tags.Contains("questionable")) rating = Rating.Questionable;
            else if (parsingData.Tags.Contains("suggestive")) rating = Rating.Safe;
            else if (parsingData.Tags.Contains("safe")) rating = Rating.General;
            else rating = (Rating)(-1); // Some images doesn't have a rating
            return new PostSearchResult(
                fileUrl: new(parsingData.Representations.Full),
                previewUrl: null,
                postUrl: new($"{BaseUrl}images/{parsingData.Id}"),
                sampleUri: new(parsingData.Representations.Thumb),
                rating: rating,
                tags: parsingData.Tags,
                detailedTags: null,
                id: parsingData.Id,
                size: parsingData.Size,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: null,
                previewWidth: null,
                creation: parsingData.CreatedAt,
                sources: string.IsNullOrEmpty(parsingData.SourceUrl) ? Array.Empty<string>() : new[] { parsingData.SourceUrl },
                score: parsingData.Score,
                hash: parsingData.Sha512Hash
            );
        }

        public class PostContainer
        {
            public SearchResult[] Posts { init; get; }
        }

        public class SearchResult
        {
            public Representations Representations { init; get; }
            public int Id { init; get; }
            public string[] Tags { init; get; }
            public int Size { init; get; }
            public int Width { init; get; }
            public int Height { init; get; }
            public DateTime CreatedAt { init; get; }
            public string SourceUrl { init; get; }
            public int Score { init; get; }
            public string Sha512Hash { init; get; }
        }

        public class Representations
        {
            public string Full { init; get; }
            public string Thumb { init; get; }
        }

        /*
        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            var token = (JToken)json;
            if (token["posts"] is JArray arr)
            {
                return arr?.FirstOrDefault() ?? throw new Search.InvalidTags();
            }
            return token["post"];
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var token = ((JToken)json)["posts"];
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
                token["posts"].Value<int>()
                );
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var token = (JToken)json;
            return new Search.Comment.SearchResult(
                token["id"].Value<int>(),
                0,
                null,
                token["created_at"].Value<DateTime>(),
                null,
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
        */
    }
}
