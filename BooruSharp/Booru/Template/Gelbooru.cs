using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

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
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Gelbooru(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.IndexPhp, options | BooruOptions.NoWiki | BooruOptions.NoRelated | BooruOptions.LimitOf20000
                  | BooruOptions.CommentApiXml)
        { }

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
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            const string gelbooruTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

            string directory = elem["directory"].Value<string>();
            string hash = elem["hash"].Value<string>();
            int id = elem["id"].Value<int>();

            return new Search.Post.SearchResult(
                new Uri(elem["file_url"].Value<string>()),
                new Uri("https://gelbooru.com/thumbnails/" + directory + "/thumbnail_" + hash + ".jpg"),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' '),
                id,
                null,
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                null,
                null,
                DateTime.ParseExact(elem["created_at"].Value<string>(), gelbooruTimeFormat, CultureInfo.InvariantCulture),
                elem["source"].Value<string>(),
                elem["score"].Value<int>(),
                hash
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
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["tag"].Value<string>(),
                StringToTagType(elem["type"].Value<string>()),
                elem["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available
    }
}
