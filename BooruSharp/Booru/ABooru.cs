using BooruSharp.Search;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        public abstract bool IsSafe();

        protected internal virtual Search.Comment.SearchResult GetCommentSearchResult(object json)
            => throw new FeatureUnavailable();

        protected internal virtual Search.Post.SearchResult GetPostSearchResult(JToken obj)
            => throw new FeatureUnavailable();

        protected internal virtual Search.Post.SearchResult[] GetPostsSearchResult(object json)
            => throw new FeatureUnavailable();

        protected internal virtual JToken ParseFirstPostSearchResult(object json)
            => throw new FeatureUnavailable();

        protected internal virtual Search.Related.SearchResult GetRelatedSearchResult(object json)
            => throw new FeatureUnavailable();

        protected internal virtual Search.Tag.SearchResult GetTagSearchResult(object json)
            => throw new FeatureUnavailable();

        protected internal virtual Search.Wiki.SearchResult GetWikiSearchResult(object json)
            => throw new FeatureUnavailable();

        /// <summary>
        /// Is it possible to search for related tag with this booru
        /// </summary>
        public bool HaveRelatedAPI()
            => _relatedUrl != null;
        /// <summary>
        /// Is it possible to search for wiki with this booru
        /// </summary>
        public bool HaveWikiAPI()
            => _wikiUrl != null;
        /// <summary>
        /// Is it possible to search for comments with this booru
        /// </summary>
        public bool HaveCommentAPI()
            => _commentUrl != null;
        /// <summary>
        /// Is it possible to search for tags using their ID with this booru
        /// </summary>
        public bool HaveTagByIdAPI()
            => _searchTagById;
        /// <summary>
        /// Is it possible to search for the lasts comments this booru
        /// </summary>
        public bool HaveSearchLastComment()
            => _searchLastComment;
        /// <summary>
        /// Is it possible to search for posts using their MD5 with this booru
        /// </summary>
        public bool HavePostByMd5API()
            => _searchPostByMd5;
        /// <summary>
        /// Is it possible to get the total number of post
        /// </summary>
        public bool HavePostCountAPI()
            => _imageUrlXml != null;
        /// <summary>
        /// Is it possible to get multiple random images
        /// </summary>
        public bool HaveMultipleRandomAPI()
            => !(this is Template.Gelbooru02);
        public bool HaveFavoriteAPI()
            => !(_format == UrlFormat.indexPhp);

        /// <summary>
        /// Is the booru available
        /// </summary>
        /// <exception cref="HttpRequestException">Service not available</exception>
        public async Task CheckAvailabilityAsync()
        {
            if (_httpClient == null)
                _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
            await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _imageUrl));
        }

        protected ABooru(string baseUrl, UrlFormat format, params BooruOptions[] options)
        {
            _auth = null;
            _httpClient = null;
            _useHttp = options.Contains(BooruOptions.useHttp);
            _maxLimit = options.Contains(BooruOptions.limitOf20000);
            _baseUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl;
            _format = format;
            _imageUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "post");
            if (_format == UrlFormat.indexPhp)
                _imageUrlXml = _imageUrl.Replace("json=1", "json=0");
            else if (_format == UrlFormat.postIndexJson)
                _imageUrlXml = _imageUrl.Replace("index.json", "index.xml");
            else
                _imageUrlXml = null;
            _searchTagById = !options.Contains(BooruOptions.noTagById);
            _searchLastComment = !options.Contains(BooruOptions.noLastComments);
            _searchPostByMd5 = !options.Contains(BooruOptions.noPostByMd5);
            _tagUseXml = options.Contains(BooruOptions.tagApiXml);
            _commentUseXml = options.Contains(BooruOptions.commentApiXml);
            _noMoreThan2Tags = options.Contains(BooruOptions.noMoreThan2Tags);
            _tagUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag");
            if (options.Contains(BooruOptions.noWiki))
                _wikiUrl = null;
            else if (format == UrlFormat.danbooru)
                _wikiUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki_page");
            else
                _wikiUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki");
            if (options.Contains(BooruOptions.noRelated))
                _relatedUrl = null;
            else if (format == UrlFormat.danbooru)
                _relatedUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "related_tag");
            else
                _relatedUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag", "related");
            if (options.Contains(BooruOptions.noComment))
            {
                _commentUrl = null;
                _searchLastComment = false;
            }
            else
                _commentUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "comment");
        }

        public void SetBooruAuth(BooruAuth auth)
            => _auth = auth;

        public void SetHttpClient(HttpClient httpClient)
            => _httpClient = httpClient;

        protected internal static string GetUrl(UrlFormat format, string query, string squery = "index")
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
            if (_httpClient == null)
                _httpClient = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
            HttpResponseMessage msg = await _httpClient.GetAsync(url);
            if (msg.StatusCode == HttpStatusCode.Forbidden)
                throw new AuthentificationRequired();
            return await msg.Content.ReadAsStringAsync();
        }

        private async Task<XmlDocument> GetXmlAsync(string url)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(await GetJsonAsync(url));
            return xml;
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            if (_httpClient == null)
                _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
            HttpResponseMessage msg = await _httpClient.GetAsync(_baseUrl + "/" + "index.php?page=post&s=random&tags=" + tags);
            return HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id");
        }

        private string CreateUrl(string url, params string[] args)
        {
            if (_format == UrlFormat.indexPhp)
                return (url + "&" + string.Join("&", args));
            else
                return (url + "?" + string.Join("&", args));
        }

        private string TagsToString(string[] tags)
        {
            return tags == null ? "" : "tags=" + string.Join("+", tags.Select(x => Uri.EscapeDataString(x))).ToLower();
        }

        private string SearchArg(string value)
        {
            if (_format == UrlFormat.danbooru)
                return "search[" + value + "]=";
            else
                return value + "=";
        }

        protected internal static BooruOptions[] CombineArrays(BooruOptions[] arr1, BooruOptions[] arr2)
        {
            var arr = new BooruOptions[arr1.Length + arr2.Length];
            arr1.CopyTo(arr, 0);
            arr2.CopyTo(arr, arr1.Length);
            return arr;
        }

        private BooruAuth _auth; // Authentification
        protected readonly string _baseUrl; // Booru's base URL
        private readonly string _imageUrlXml, _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl; // URLs for differents endpoints
        private readonly bool _searchTagById, _searchLastComment, _searchPostByMd5; // Differents services availability
        private readonly bool _tagUseXml, _commentUseXml; // APIs use XML instead of JSON
        private readonly bool _noMoreThan2Tags;
        private readonly bool _maxLimit; // Have max limit (used by Gelbooru)
        private readonly UrlFormat _format; // URL format
        protected readonly bool _useHttp; // Use http instead of https
        private static readonly Random _random = new Random();
        private HttpClient _httpClient;
    }
}
