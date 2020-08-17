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
        public bool HasRelatedAPI()
            => _relatedUrl != null;
        /// <summary>
        /// Is it possible to search for wiki with this booru
        /// </summary>
        public bool HasWikiAPI()
            => _wikiUrl != null;
        /// <summary>
        /// Is it possible to search for comments with this booru
        /// </summary>
        public bool HasCommentAPI()
            => _commentUrl != null;
        /// <summary>
        /// Is it possible to search for tags using their ID with this booru
        /// </summary>
        public bool HasTagByIdAPI()
            => _searchTagById;
        /// <summary>
        /// Is it possible to search for the lasts comments this booru
        /// </summary>
        public bool HasSearchLastComment()
            => _searchLastComment;
        /// <summary>
        /// Is it possible to search for posts using their MD5 with this booru
        /// </summary>
        public bool HasPostByMd5API()
            => _searchPostByMd5;
        /// <summary>
        /// Is it possible to search for posts using their ID with this booru
        /// </summary>
        public bool HasPostByIdAPI()
            => _searchPostById;
        /// <summary>
        /// Is it possible to get the total number of post
        /// </summary>
        public bool HasPostCountAPI()
            => _postCount;
        /// <summary>
        /// Is it possible to get multiple random images
        /// </summary>
        public bool HasMultipleRandomAPI()
            => _multipleRandom;
        /// <summary>
        /// Is it possible to add/remove favorites
        /// </summary>
        public bool HasFavoriteAPI()
            => _addFavorite;

        /// <summary>
        /// Is the booru available
        /// </summary>
        /// <exception cref="HttpRequestException">Service not available</exception>
        public async Task CheckAvailabilityAsync()
        {
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
            await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _imageUrl));
        }

        [Obsolete(_deprecationMessage)]
        protected ABooru(string baseUrl, UrlFormat format, params BooruOptions[] options)
            : this(baseUrl, format, MergeOptions(options))
        { }

        protected ABooru(string baseUrl, UrlFormat format, BooruOptions options)
        {
            Auth = null;
            HttpClient = null;
            _useHttp = options.HasFlag(BooruOptions.useHttp);
            _maxLimit = options.HasFlag(BooruOptions.limitOf20000);
            _baseUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl;
            _format = format;
            _imageUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "post");
            if (_format == UrlFormat.indexPhp)
                _imageUrlXml = _imageUrl.Replace("json=1", "json=0");
            else if (_format == UrlFormat.postIndexJson)
                _imageUrlXml = _imageUrl.Replace("index.json", "index.xml");
            else
                _imageUrlXml = null;
            _searchTagById = !options.HasFlag(BooruOptions.noTagById);
            _searchLastComment = !options.HasFlag(BooruOptions.noLastComments);
            _searchPostByMd5 = !options.HasFlag(BooruOptions.noPostByMd5);
            _searchPostById = !options.HasFlag(BooruOptions.noPostById);
            _postCount = !options.HasFlag(BooruOptions.noPostCount);
            _multipleRandom = !options.HasFlag(BooruOptions.noMultipleRandom);
            _addFavorite = !options.HasFlag(BooruOptions.noFavorite);
            _tagUseXml = options.HasFlag(BooruOptions.tagApiXml);
            _commentUseXml = options.HasFlag(BooruOptions.commentApiXml);
            _noMoreThan2Tags = options.HasFlag(BooruOptions.noMoreThan2Tags);
            _tagUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag");
            if (options.HasFlag(BooruOptions.noWiki))
                _wikiUrl = null;
            else if (format == UrlFormat.danbooru)
                _wikiUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki_page");
            else
                _wikiUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki");
            if (options.HasFlag(BooruOptions.noRelated))
                _relatedUrl = null;
            else if (format == UrlFormat.danbooru)
                _relatedUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "related_tag");
            else
                _relatedUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag", "related");
            if (options.HasFlag(BooruOptions.noComment))
            {
                _commentUrl = null;
                _searchLastComment = false;
            }
            else
                _commentUrl = "http" + (_useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "comment");
        }

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
                    return null;
            }
        }

        // TODO: Handle limitrate

        private async Task<string> GetJsonAsync(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
            HttpResponseMessage msg = await HttpClient.GetAsync(url);
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
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
            HttpResponseMessage msg = await HttpClient.GetAsync(_baseUrl + "/" + "index.php?page=post&s=random&tags=" + tags);
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

        [Obsolete]
        // TODO: remove this method after removing obsolete constructors.
        protected internal static BooruOptions[] CombineArrays(BooruOptions[] arr1, BooruOptions[] arr2)
        {
            var arr = new BooruOptions[arr1.Length + arr2.Length];
            arr1.CopyTo(arr, 0);
            arr2.CopyTo(arr, arr1.Length);
            return arr;
        }

        /// <summary>
        /// Method for compatibility with old API. Combines multiple options
        /// into singular <see cref="BooruOptions"/> object.
        /// </summary>
        // TODO: remove this method after removing obsolete constructors.
        [Obsolete("This method will be removed in the future version of the API.")]
        protected static BooruOptions MergeOptions(BooruOptions[] array)
        {
            BooruOptions options = BooruOptions.none;

            for (int i = 0; i < array.Length; i++)
            {
                options |= array[i];
            }

            return options;
        }

        public BooruAuth Auth { set; get; } // Authentification
        public HttpClient HttpClient { set { _client = value; }
            protected get
            {
                if (_client == null)
                    _client = new HttpClient();
                return _client;
            } }
        private HttpClient _client;
        protected readonly string _baseUrl; // Booru's base URL
        private readonly string _imageUrlXml, _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl; // URLs for differents endpoints
        private readonly bool _searchTagById, _searchLastComment, _searchPostByMd5, _searchPostById, _postCount, _multipleRandom, _addFavorite; // Differents services availability
        private readonly bool _tagUseXml, _commentUseXml; // APIs use XML instead of JSON
        private readonly bool _noMoreThan2Tags;
        private readonly bool _maxLimit; // Have max limit (used by Gelbooru)
        private readonly UrlFormat _format; // URL format
        protected readonly bool _useHttp; // Use http instead of https
        protected static readonly Random _random = new Random();
        // TODO: remove this message after removing obsolete constructors.
        protected const string _deprecationMessage = "Use a contructor that accepts single BooruOptions parameter. Use | (bitwise OR) operator to combine multiple options.";
    }
}
