using BooruSharp.Search.Post;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Gelbooru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Gelbooru : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        protected Gelbooru(string domain)
            : base(domain)
        { }

        protected override Uri CreateQueryString(string query, string squery = "index")
        {
            return new($"{APIBaseUrl}index.php?page=dapi&s={query}&q=index&json=1");
        }

        protected override Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            return Task.FromResult(CreateUrl(_imageUrl, "limit=1", "tags=" + string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant() + "+sort:random"));
        }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var parsingData = (await GetDataAsync<DataContainer>(uri)).Post[0];

            return new PostSearchResult(
                fileUrl: new(parsingData.FileUrl),
                previewUrl: new(parsingData.PreviewUrl),
                postUrl: new Uri($"{PostBaseUrl}index.php?page=post&s=view&id={parsingData.Id}"),
                sampleUri: !string.IsNullOrEmpty(parsingData.SampleUrl) ? new Uri(parsingData.SampleUrl) : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.Tags.Split().Select(HttpUtility.HtmlDecode),
                detailedTags: null,
                id: parsingData.Id,
                size: null,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: null,
                previewWidth: null,
                creation: DateTime.ParseExact(parsingData.CreatedAt, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture),
                sources: string.IsNullOrEmpty(parsingData.Source) ? Array.Empty<string>() : new[] { parsingData.Source },
                score: parsingData.Score,
                hash: parsingData.Md5
            );
        }

        public class DataContainer
        {
            public SearchResult[] Post { init; get; }
        }

        public class SearchResult
        {
            public string FileUrl { init; get; }
            public string PreviewUrl { init; get; }
            public string SampleUrl { init; get; }
            public int Id { init; get; }
            public string Rating { init; get; }
            public string Tags { init; get; }
            public int Height { init; get; }
            public int Width { init; get; }
            public string CreatedAt { init; get; }
            public string Source { init; get; }
            public int Score { init; get; }
            public string Md5 { init; get; }
        }

        /*
        /// <inheritdoc/>
        public async override Task<Search.Post.SearchResult> GetPostByMd5Async(string md5)
        {
            if (md5 == null)
                throw new ArgumentNullException(nameof(md5));

            // Create a URL that will redirect us to Gelbooru post URL containing post ID.
            string url = $"{BaseUrl}index.php?page=post&s=list&md5={md5}";

            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Head, url))
            using (HttpResponseMessage response = await HttpClient.SendAsync(message))
            {
                response.EnsureSuccessStatusCode();

                // If HEAD message doesn't actually redirect us then ID here will be null...
                Uri redirectUri = response.RequestMessage.RequestUri;
                string id = HttpUtility.ParseQueryString(redirectUri.Query).Get("id");

                // ...which will then throw NullReferenceException here.
                // Danbooru does the same when it doesn't find a post with matching MD5,
                // though I suppose throwing exception with more meaningful message
                // would be better.
                return await GetPostByIdAsync(int.Parse(id));
            }
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = ((JToken)json)["post"] as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            const string gelbooruTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

            int id = elem["id"].Value<int>();
            var sampleUrl = elem["sample_url"].Value<string>();

            return new Search.Post.SearchResult(
                new Uri(elem["file_url"].Value<string>()),
                new Uri(elem["preview_url"].Value<string>()),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                string.IsNullOrWhiteSpace(sampleUrl) ? null : new Uri(sampleUrl),
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' ').Select(HttpUtility.HtmlDecode).ToArray(),
                null,
                id,
                null,
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                null,
                null,
                DateTime.ParseExact(elem["created_at"].Value<string>(), gelbooruTimeFormat, CultureInfo.InvariantCulture),
                elem["source"].Value<string>(),
                elem["score"].Value<int>(),
                elem["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return ((JToken)json)["post"] is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (XmlNode)json;
            XmlNode creatorId = elem.Attributes.GetNamedItem("creator_id");
            return new Search.Comment.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                int.Parse(elem.Attributes.GetNamedItem("post_id").Value),
                creatorId.InnerText.Length > 0 ? int.Parse(creatorId.Value) : (int?)null,
                DateTime.ParseExact(elem.Attributes.GetNamedItem("created_at").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                elem.Attributes.GetNamedItem("creator").Value,
                elem.Attributes.GetNamedItem("body").Value
                );
        }

        // GetWikiSearchResult not available

        private protected override SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new SearchResult(
                elem["id"].Value<int>(),
                HttpUtility.HtmlDecode(elem["name"].Value<string>()),
                (TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available

        private protected override async Task<IEnumerable> GetTagEnumerableSearchResultAsync(Uri url)
        {
            if (JsonConvert.DeserializeObject<JObject>(await GetJsonAsync(url)).TryGetValue("tag", out JToken token))
            {
                return (JArray)token;
            }
            throw new InvalidTags();
        }
        */
    }
}
