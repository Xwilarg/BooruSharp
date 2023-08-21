using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Gelbooru 0.2. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Gelbooru02 : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru02"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        protected Gelbooru02(string domain) 
            : base(domain)
        { }

        protected override Uri CreateQueryString(string query, string squery = "index")
        {
            return new($"{APIBaseUrl}index.php?page=dapi&s={query}&q=index");
        }

        protected override Task<Uri> CreatePostByIdUriAsync(int id)
        {
            return Task.FromResult(new Uri($"{_imageUrl}&limit=1&id={id}&json=1"));
        }

        protected override async Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            if (!tags.Any())
            {
                // We need to request /index.php?page=post&s=random and get the id given by the redirect
                HttpResponseMessage msg = await HttpClient.GetAsync($"{APIBaseUrl}index.php?page=post&s=random");
                msg.EnsureSuccessStatusCode();
                return new Uri($"{_imageUrl}&limit=1&id={HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id")}&json=1");
            }
            var url = new Uri($"{_imageUrl}&limit=1&tags={string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant()}&json=0");
            XmlDocument xml = await GetXmlAsync(url.AbsoluteUri);
            int max = int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);

            if (max == 0)
                throw new InvalidPostException();

            return new Uri($"{_imageUrl}&limit=1&tags={string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant()}&pid={Random.Next(0, max)}&json=1");
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
            var posts = await GetDataAsync<SearchResult[]>(uri);
            if (!posts.Any())
            {
                throw new InvalidPostException();
            }
            var parsingData = posts[0];

            return new PostSearchResult(
                fileUrl: new($"{FileBaseUrl}images/{parsingData.Directory}/{parsingData.Image}"),
                previewUrl: new($"{PreviewBaseUrl}thumbnails/{parsingData.Directory}/thumbnail_{parsingData.Image.Split('.')[0]}.jpg"),
                postUrl: new($"{PostBaseUrl}index.php?page=post&s=view&id={parsingData.Id}"),
                sampleUri: parsingData.Sample ? new($"{SampleBaseUrl}samples/{parsingData.Directory}/sample_{parsingData.Image.Split('.')[0]}.jpg") : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.Tags.Split(),
                detailedTags: null,
                id: parsingData.Id,
                size: null,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: null,
                previewWidth: null,
                creation: null,
                sources: null,
                score: parsingData.Score,
                hash: parsingData.Hash
            );
        }

        public class SearchResult
        {
            public string Directory { init; get; }
            public string Image { init; get; }
            public int Id { init; get; }
            public bool Sample { init; get; }
            public string Rating { init; get; }
            public string Tags { init; get; }
            public int Height { init; get; }
            public int Width { init; get; }
            public int? Score { init; get; }
            public string Hash { init; get; }
        }

        protected async Task<XmlDocument> GetXmlAsync(string url)
        {
            var xmlDoc = new XmlDocument();
            var xmlString = await GetJsonAsync(url);
            // https://www.key-shortcut.com/en/all-html-entities/all-entities/
            xmlDoc.LoadXml(Regex.Replace(xmlString, "&([a-zA-Z]+);", HttpUtility.HtmlDecode("$1")));
            return xmlDoc;
        }

        /*

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            string baseUrl = BaseUrl.Scheme + "://" + _url;
            string directory = elem["directory"].Value<string>();
            string image = elem["image"].Value<string>();
            string hash = elem["hash"].Value<string>();
            int id = elem["id"].Value<int>();
            var hasSample = elem["sample"].Value<bool>();

            return new Search.Post.SearchResult(
                new Uri(baseUrl + "//images/" + directory + "/" + image),
                new Uri(baseUrl + "//thumbnails/" + directory + "/thumbnails_" + image),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                hasSample ? new Uri(baseUrl + "//samples/" + directory + "/sample_" + hash + ".jpg") : null,
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' '),
                null,
                id,
                null,
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                null,
                null,
                null,
                null,
                elem["score"].Value<int?>(),
                null
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array 
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

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (XmlNode)json;
            return new Search.Tag.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                elem.Attributes.GetNamedItem("name").Value,
                (Search.Tag.TagType)int.Parse(elem.Attributes.GetNamedItem("type").Value),
                int.Parse(elem.Attributes.GetNamedItem("count").Value)
                );
        }

        // GetRelatedSearchResult not available

        private readonly string _url;
        */
    }
}
