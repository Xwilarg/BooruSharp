using BooruSharp.Search.Post;
using BooruSharp.Search.Tag;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
        protected E621(string domain)
            : base(domain)
        { }

        protected override Uri CreateQueryString(string query, string squery = "index")
        {
            if (query == "tag" && squery == "related")
            {
                return new($"{APIBaseUrl}related_tag.json");
            }
            if (query == "tag" && squery == "wiki")
            {
                return new($"{APIBaseUrl}wiki_pages.json");
            }
            return new($"{APIBaseUrl}{query}s.json");
        }

        protected override Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            if (tags.Length > 2)
            {
                throw new Search.TooManyTags();
            }
            return Task.FromResult(CreateUrl(_imageUrl, "limit=1", "tags=" + string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant() + "+order:random"));
        }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(Auth.UserId + ":" + Auth.PasswordHash));
                message.Headers.Add("Authorization", $"Basic {encoded}");
            }
        }

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var posts = await GetDataAsync<SearchResult[]>(uri);
            if (!posts.Any())
            {
                throw new InvalidTags();
            }
            var parsingData = posts[0];

            return new PostSearchResult(
                fileUrl: parsingData.File.Url != null ? new Uri(parsingData.File.Url) : null,
                previewUrl: parsingData.Preview.Url != null ? new Uri(parsingData.Preview.Url) : null,
                postUrl: new Uri($"{PostBaseUrl}posts/{parsingData.Id}"),
                sampleUri: parsingData.Sample.Url != null ? new Uri(parsingData.Sample.Url) : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.Tags.General
                    .Concat(parsingData.Tags.Species)
                    .Concat(parsingData.Tags.Character)
                    .Concat(parsingData.Tags.Copyright)
                    .Concat(parsingData.Tags.Artist)
                    .Concat(parsingData.Tags.Invalid)
                    .Concat(parsingData.Tags.Lore)
                    .Concat(parsingData.Tags.Meta),
                detailedTags: parsingData.Tags.Species.Select(x => new TagSearchResult(-1, x, TagType.Species, -1))
                    .Concat(parsingData.Tags.Character.Select(x => new TagSearchResult(-1, x, TagType.Character, -1)))
                    .Concat(parsingData.Tags.Copyright.Select(x => new TagSearchResult(-1, x, TagType.Copyright, -1)))
                    .Concat(parsingData.Tags.Artist.Select(x => new TagSearchResult(-1, x, TagType.Artist, -1)))
                    .Concat(parsingData.Tags.Invalid.Select(x => new TagSearchResult(-1, x, TagType.Invalid, -1)))
                    .Concat(parsingData.Tags.Lore.Select(x => new TagSearchResult(-1, x, TagType.Lore, -1)))
                    .Concat(parsingData.Tags.Meta.Select(x => new TagSearchResult(-1, x, TagType.Metadata, -1))),
                id: parsingData.Id,
                size: parsingData.File.Size,
                height: parsingData.File.Height,
                width: parsingData.File.Width,
                previewHeight: parsingData.Preview.Height,
                previewWidth: parsingData.Preview.Width,
                creation: parsingData.CreatedAt,
                sources: parsingData.Sources ?? Array.Empty<string>(),
                score: parsingData.Score.Total,
                hash: parsingData.File.Md5
            );
        }

        public class DataContainer
        {
            public SearchResult[] Posts { init; get; }
        }

        public class SearchResult
        {
            public ImageData File { init; get; }
            public ImageData Preview { init; get; }
            public ImageData Sample { init; get; }
            public Tags Tags { init; get; }
            public int Id { init; get; }
            public string Rating { init; get; }
            public DateTime CreatedAt { init; get; }
            public string[] Sources { init; get; }
            public Score Score { init; get; }
        }

        public class ImageData
        {
            public string Url { init; get; }
            public int Width { init; get; }
            public int Height { init; get; }
            public int Size { init; get; }
            public string Md5 { init; get; }
        }

        public class Tags
        {
            public string[] General { init; get; }
            public string[] Species { init; get; }
            public string[] Character { init; get; }
            public string[] Copyright { init; get; }
            public string[] Artist { init; get; }
            public string[] Invalid { init; get; }
            public string[] Lore { init; get; }
            public string[] Meta { init; get; }
        }

        public class Score
        {
            public int Total { init; get; }
        }

        /*
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
            var detailedTags = new List<Search.Tag.SearchResult>();
            var tags = new List<string>();
            foreach(var cat in elem["tags"].OfType<JProperty>())
            {
                foreach(var tag in cat.Value.ToObject<string[]>())
                {
                    tags.Add(tag);
                    detailedTags.Add(new Search.Tag.SearchResult(-1, tag, GetTagType(cat.Name), -1));
                }
            }

            var fileToken = elem["file"];
            var previewToken = elem["preview"];
            var sampleToken = elem["sample"];

            string url = fileToken["url"].Value<string>();
            string previewUrl = previewToken["url"].Value<string>();
            string sampleUrl = sampleToken["has"].Value<bool>() ? sampleToken["url"].Value<string>() : null;
            
            int id = elem["id"].Value<int>();

            return new Search.Post.SearchResult(
                    url != null ? new Uri(url) : null,
                    previewUrl != null ? new Uri(previewUrl) : null,
                    new Uri(BaseUrl + "posts/" + id),
                    sampleUrl != null ? new Uri(sampleUrl) : null,
                    GetRating(elem["rating"].Value<string>()[0]),
                    tags,
                    detailedTags,
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
        
        private Search.Tag.TagType GetTagType(string typeName)
        {
            switch(typeName)
            {
                case "species": return Search.Tag.TagType.Species;
                case "invalid": return Search.Tag.TagType.Invalid;
                case "lore": return Search.Tag.TagType.Lore;
                case "general": return Search.Tag.TagType.Trivia;
                case "character": return Search.Tag.TagType.Character;
                case "copyright": return Search.Tag.TagType.Copyright;
                case "artist": return Search.Tag.TagType.Artist;
                case "meta": return Search.Tag.TagType.Metadata;
                default: return (Search.Tag.TagType)6;
            }
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
        */
    }
}
