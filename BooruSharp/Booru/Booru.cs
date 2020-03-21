using BooruSharp.Search;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public abstract bool IsSafe();

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual Search.Comment.SearchResult GetCommentSearchResult(object json)
            => throw new FeatureUnavailable();

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual Search.Post.SearchResult GetPostSearchResult(object json)
            => throw new FeatureUnavailable();

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual Search.Related.SearchResult GetRelatedSearchResult(object json)
            => throw new FeatureUnavailable();

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual Search.Tag.SearchResult GetTagSearchResult(object json)
            => throw new FeatureUnavailable();

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual Search.Wiki.SearchResult GetWikiSearchResult(object json)
            => throw new FeatureUnavailable();

        public bool HaveRelatedAPI()
            => relatedUrl != null;
        public bool HaveWikiAPI()
            => wikiUrl != null;
        public bool HaveCommentAPI()
            => commentUrl != null;

        public bool HaveTagByIdAPI()
            => searchTagById;
        public bool HaveSearchLastComment()
            => searchLastComment;
        public bool HavePostByMd5API()
            => searchPostByMd5;

        /// <exception cref="HttpRequestException">Service not available</exception>
        public async Task CheckAvailabilityAsync()
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                await hc.SendAsync(new HttpRequestMessage(HttpMethod.Head, imageUrl));
            }
        }

        protected Booru(string baseUrl, BooruAuth auth, UrlFormat format, params BooruOptions[] options)
        {
            this.auth = auth;
            useHttp = options.Contains(BooruOptions.useHttp);
            maxLimit = options.Contains(BooruOptions.limitOf20000);
            this.baseUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl;
            this.format = format;
            imageUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "post");
            imageUrlXml = imageUrl.Replace("json=1", "json=0"); // Only needed for websites with UrlFormat.indexPhp
            searchTagById = !options.Contains(BooruOptions.noTagById);
            searchLastComment = !options.Contains(BooruOptions.noLastComments);
            searchPostByMd5 = !options.Contains(BooruOptions.noPostByMd5);
            tagUseXml = options.Contains(BooruOptions.tagApiXml);
            commentUseXml = options.Contains(BooruOptions.commentApiXml);
            tagUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag");
            if (options.Contains(BooruOptions.noWiki))
                wikiUrl = null;
            else if (format == UrlFormat.danbooru)
                wikiUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki_page");
            else
                wikiUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki");
            if (options.Contains(BooruOptions.noRelated))
                relatedUrl = null;
            else if (format == UrlFormat.danbooru)
                relatedUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "related_tag");
            else
                relatedUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag", "related");
            if (options.Contains(BooruOptions.noComment))
            {
                commentUrl = null;
                searchLastComment = false;
            }
            else
                commentUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "comment");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static string GetUrl(UrlFormat format, string query, string squery = "index")
        {
            switch (format)
            {
                case UrlFormat.postIndexJson:
                    return query + "/" + squery + ".json";

                case UrlFormat.indexPhp:
                    return "index.php?page=dapi&s=" + query + "&q=index&json=1";

                case UrlFormat.danbooru:
                    if (query == "related_tag")
                        return query + ".json";
                    return query + "s.json";

                case UrlFormat.sankaku:
                    if (query == "wiki")
                        return query;
                    return query + "s";

                default:
                    throw new ArgumentException("Invalid URL format " + format);
            }
        }

        // TODO: Handle limitrate

        private async Task<string> GetJsonAsync(string url)
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                HttpResponseMessage msg = await hc.GetAsync(url);
                if (msg.StatusCode == HttpStatusCode.Forbidden)
                    throw new AuthentificationRequired();
                return await msg.Content.ReadAsStringAsync();
            }
        }

        private async Task<XmlDocument> GetXmlAsync(string url)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(await GetJsonAsync(url));
            return xml;
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                HttpResponseMessage msg = await hc.GetAsync(baseUrl + "/" + "index.php?page=post&s=random&tags=" + tags);
                return HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id");
            }
        }

        private string CreateUrl(string url, params string[] args)
        {
            string authArgs = "";
            if (auth != null)
                authArgs = "&login=" + auth.Login + "&password_hash=" + auth.PasswordHash;
            if (format == UrlFormat.indexPhp)
                return (url + "&" + string.Join("&", args) + authArgs);
            else
                return (url + "?" + string.Join("&", args) + authArgs);
        }

        private string TagsToString(string[] tags)
        {
            return tags == null ? "" : "tags=" + string.Join("+", tags.Select(x => Uri.EscapeDataString(x))).ToLower();
        }

        private string SearchArg(string value)
        {
            if (format == UrlFormat.danbooru)
                return "search[" + value + "]=";
            else
                return value + "=";
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected static BooruOptions[] CombineArrays(BooruOptions[] arr1, BooruOptions[] arr2)
        {
            var arr = new BooruOptions[arr1.Length + arr2.Length];
            arr1.CopyTo(arr, 0);
            arr2.CopyTo(arr, arr1.Length);
            return arr;
        }

        private readonly BooruAuth auth; // Authentification
        private readonly string baseUrl; // Booru's base URL
        private readonly string imageUrlXml, imageUrl, tagUrl, wikiUrl, relatedUrl, commentUrl; // URLs for differents endpoints
        private readonly bool searchTagById, searchLastComment, searchPostByMd5; // Differents services availability
        private readonly bool tagUseXml, commentUseXml; // APIs use XML instead of JSON
        private readonly bool maxLimit; // Have max limit (used by Gelbooru)
        private readonly UrlFormat format; // URL format
        protected readonly bool useHttp; // Use http instead of https
        private static readonly Random random = new Random();
    }
}
