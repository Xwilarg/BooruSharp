using BooruSharp.Utils;
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
            if (md5 is null)
                throw new ArgumentNullException(nameof(md5));

            // Create a URL that will redirect us to Gelbooru post URL containing post ID.
            var url = $"{BaseUrl}index.php?page=post&s=list&md5={md5}";

            using (var message = new HttpRequestMessage(HttpMethod.Head, url))
            using (var response = await GetResponseAsync(message))
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

        private protected override JToken ParseFirstPostSearchResult(JToken token)
        {
            JArray array = token as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken token)
        {
            const string gelbooruTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

            string directory = token["directory"].Value<string>();
            string hash = token["hash"].Value<string>();
            int id = token["id"].Value<int>();

            return new Search.Post.SearchResult(
                new Uri(token["file_url"].Value<string>()),
                new Uri("https://gelbooru.com/thumbnails/" + directory + "/thumbnail_" + hash + ".jpg"),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                RatingUtils.Parse(token["rating"].Value<string>()),
                token["tags"].Value<string>().Split(' '),
                id,
                null,
                token["height"].Value<int>(),
                token["width"].Value<int>(),
                null,
                null,
                DateTime.ParseExact(token["created_at"].Value<string>(), gelbooruTimeFormat, CultureInfo.InvariantCulture),
                token["source"].Value<string>(),
                token["score"].Value<int>(),
                hash
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(JToken token)
        {
            return token is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(XmlNode node)
        {
            XmlNode creatorId = node.Attributes.GetNamedItem("creator_id");
            return new Search.Comment.SearchResult(
                int.Parse(node.Attributes.GetNamedItem("id").Value),
                int.Parse(node.Attributes.GetNamedItem("post_id").Value),
                creatorId.InnerText.Length > 0 ? int.Parse(creatorId.Value) : (int?)null,
                DateTime.ParseExact(node.Attributes.GetNamedItem("created_at").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                node.Attributes.GetNamedItem("creator").Value,
                node.Attributes.GetNamedItem("body").Value
                );
        }

        // GetWikiSearchResult not available

        private protected override Search.Tag.SearchResult GetTagSearchResult(JToken token)
        {
            return new Search.Tag.SearchResult(
                token["id"].Value<int>(),
                token["tag"].Value<string>(),
                TagUtils.Parse(token["type"].Value<string>()),
                token["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available
    }
}
