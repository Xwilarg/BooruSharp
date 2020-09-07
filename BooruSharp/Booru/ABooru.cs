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
    /// <summary>
    /// Defines basic capabilities of a booru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract partial class ABooru
    {
        /// <summary>
        /// Gets whether this booru is considered safe (that is, all posts on
        /// this booru have rating of <see cref="Search.Post.Rating.Safe"/>).
        /// </summary>
        public abstract bool IsSafe();

        private protected virtual Search.Comment.SearchResult GetCommentSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult GetPostSearchResult(JToken obj)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult[] GetPostsSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual JToken ParseFirstPostSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Related.SearchResult GetRelatedSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Tag.SearchResult GetTagSearchResult(object json)
            => throw new FeatureUnavailable();

        private protected virtual Search.Wiki.SearchResult GetWikiSearchResult(object json)
            => throw new FeatureUnavailable();

        // TODO: these flag checking methods need to be turned into properties at some point,
        // using properties for these kind of checks expresses the intent behind them more clearly.

        /// <summary>
        /// Gets whether it is possible to search for related tags on this booru.
        /// </summary>
        public bool HasRelatedAPI() => !_options.HasFlag(BooruOptions.noRelated);

        /// <summary>
        /// Gets whether it is possible to search for wiki entries on this booru.
        /// </summary>
        public bool HasWikiAPI() => !_options.HasFlag(BooruOptions.noWiki);

        /// <summary>
        /// Gets whether it is possible to search for comments on this booru.
        /// </summary>
        public bool HasCommentAPI() => !_options.HasFlag(BooruOptions.noComment);

        /// <summary>
        /// Gets whether it is possible to search for tags by their IDs on this booru.
        /// </summary>
        public bool HasTagByIdAPI() => !_options.HasFlag(BooruOptions.noTagById);

        /// <summary>
        /// Gets whether it is possible to search for the last comments on this booru.
        /// </summary>
            // As a failsafe also check for the availability of comment API.
        public bool HasSearchLastComment() => HasCommentAPI() && !_options.HasFlag(BooruOptions.noLastComments);

        /// <summary>
        /// Gets whether it is possible to search for posts by their MD5 on this booru.
        /// </summary>
        public bool HasPostByMd5API() => !_options.HasFlag(BooruOptions.noPostByMd5);

        /// <summary>
        /// Gets whether it is possible to search for posts by their ID on this booru.
        /// </summary>
        public bool HasPostByIdAPI() => !_options.HasFlag(BooruOptions.noPostById);

        /// <summary>
        /// Gets whether it is possible to get the total number of posts on this booru.
        /// </summary>
        public bool HasPostCountAPI() => !_options.HasFlag(BooruOptions.noPostCount);

        /// <summary>
        /// Gets whether it is possible to get multiple random images on this booru.
        /// </summary>
        public bool HasMultipleRandomAPI() => !_options.HasFlag(BooruOptions.noMultipleRandom);

        /// <summary>
        /// Gets whether this booru supports adding or removing favorite posts.
        /// </summary>
        public bool HasFavoriteAPI() => !_options.HasFlag(BooruOptions.noFavorite);

        /// <summary>
        /// Gets whether this booru can't call post functions without search arguments.
        /// </summary>
        public bool NoEmptyPostSearch() => _options.HasFlag(BooruOptions.noEmptyPostSearch);

        /// <summary>
        /// Gets a value indicating whether http:// scheme is used instead of https://.
        /// </summary>
        protected bool UsesHttp() => _options.HasFlag(BooruOptions.useHttp);

        /// <summary>
        /// Gets a value indicating whether tags API uses XML instead of JSON.
        /// </summary>
        protected bool TagsUseXml() => _options.HasFlag(BooruOptions.tagApiXml);

        /// <summary>
        /// Gets a value indicating whether comments API uses XML instead of JSON.
        /// </summary>
        protected bool CommentsUseXml() => _options.HasFlag(BooruOptions.commentApiXml);

        /// <summary>
        /// Gets a value indicating whether searching by more than two tags at once is not allowed.
        /// </summary>
        protected bool NoMoreThanTwoTags() => _options.HasFlag(BooruOptions.noMoreThan2Tags);

        /// <summary>
        /// Gets a value indicating whether the max limit of posts per search is increased (used by Gelbooru).
        /// </summary>
        protected bool SearchIncreasedPostLimit() => _options.HasFlag(BooruOptions.limitOf20000);

        /// <summary>
        /// Checks for the booru availability.
        /// Throws <see cref="HttpRequestException"/> if service isn't available.
        /// </summary>
        public async Task CheckAvailabilityAsync()
        {
            await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _imageUrl));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABooru"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to use. This should be a host name.</param>
        /// <param name="format">The URL format to use.</param>
        /// <param name="options">The collection of option values.</param>
        [Obsolete(_deprecationMessage)]
        protected ABooru(string baseUrl, UrlFormat format, params BooruOptions[] options)
            : this(baseUrl, format, MergeOptions(options))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABooru"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL to use. This should be a host name.</param>
        /// <param name="format">The URL format to use.</param>
        /// <param name="options">The options to use. Use | (bitwise OR) operator to combine multiple options.</param>
        protected ABooru(string baseUrl, UrlFormat format, BooruOptions options)
        {
            Auth = null;
            HttpClient = null;
            _options = options;

            bool useHttp = UsesHttp(); // Cache returned value for faster access.
#pragma warning disable CS0618 // Keep this field for a while in case someone still depends on it.
            _useHttp = useHttp;
#pragma warning restore CS0618
            _baseUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl;
            _format = format;
            _imageUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "post");

            if (_format == UrlFormat.indexPhp)
                _imageUrlXml = _imageUrl.Replace("json=1", "json=0");
            else if (_format == UrlFormat.postIndexJson)
                _imageUrlXml = _imageUrl.Replace("index.json", "index.xml");

            _tagUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag");

            if (HasWikiAPI())
                _wikiUrl = format == UrlFormat.danbooru
                    ? "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki_page")
                    : "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki");

            if (HasRelatedAPI())
                _relatedUrl = format == UrlFormat.danbooru
                    ? "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "related_tag")
                    : "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag", "related");

            if (HasCommentAPI())
                _commentUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "comment");
        }

        private protected static string GetUrl(UrlFormat format, string query, string squery = "index")
        {
            switch (format)
            {
                case UrlFormat.postIndexJson:
                    return query + "/" + squery + ".json";

                case UrlFormat.indexPhp:
                    return "index.php?page=dapi&s=" + query + "&q=index&json=1";

                case UrlFormat.danbooru:
                    return query == "related_tag" ? query + ".json" : query + "s.json";

                case UrlFormat.sankaku:
                    return query == "wiki" ? query : query + "s";

                default:
                    return null;
            }
        }

        // TODO: Handle limitrate

        private async Task<string> GetJsonAsync(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpResponseMessage msg = await HttpClient.GetAsync(url);

            if (msg.StatusCode == HttpStatusCode.Forbidden)
                throw new AuthentificationRequired();

            msg.EnsureSuccessStatusCode();

            return await msg.Content.ReadAsStringAsync();
        }

        private async Task<XmlDocument> GetXmlAsync(string url)
        {
            var xmlDoc = new XmlDocument();
            var xmlString = await GetJsonAsync(url);
            xmlDoc.LoadXml(XmlEntity.ReplaceAll(xmlString));
            return xmlDoc;
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            HttpResponseMessage msg = await HttpClient.GetAsync(_baseUrl + "/" + "index.php?page=post&s=random&tags=" + tags);
            msg.EnsureSuccessStatusCode();
            return HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id");
        }

        private string CreateUrl(string url, params string[] args)
        {
            return _format == UrlFormat.indexPhp
                ? url + "&" + string.Join("&", args)
                : url + "?" + string.Join("&", args);
        }

        private string TagsToString(string[] tags)
        {
            return tags != null
                ? "tags=" + string.Join("+", tags.Select(Uri.EscapeDataString)).ToLower()
                : "";
        }

        private string SearchArg(string value)
        {
            return _format == UrlFormat.danbooru
                ? "search[" + value + "]="
                : value + "=";
        }

        [Obsolete]
        // TODO: remove this method after removing obsolete constructors.
        private protected static BooruOptions[] CombineArrays(BooruOptions[] arr1, BooruOptions[] arr2)
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
                options |= array[i];

            return options;
        }

        /// <summary>
        /// Gets or sets authentication credentials.
        /// </summary>
        public BooruAuth Auth { set; get; } // Authentification

        /// <summary>
        /// Sets the <see cref="System.Net.Http.HttpClient"/> instance that will be used
        /// to make requests. If <see langword="null"/> or left unset, the default
        /// <see cref="System.Net.Http.HttpClient"/> instance will be used.
        /// <para>This property can only be read in <see cref="ABooru"/> subclasses.</para>
        /// </summary>
        public HttpClient HttpClient
        {
            protected get
            {
                // If library consumers didn't provide their own client,
                // initialize and use singleton client instead.
                return _client ?? _lazyClient.Value;
            }
            set
            {
                _client = value;

                // Add our User-Agent if client's User-Agent header is empty.
                if (_client != null && !_client.DefaultRequestHeaders.Contains("User-Agent"))
                    _client.DefaultRequestHeaders.Add("User-Agent", _userAgentHeaderValue);
            }
        }

        /// <summary>
        /// The instance of <see cref="Random"/> class used for generating random post IDs.
        /// </summary>
        protected static readonly Random _random = new Random();
        /// <summary>
        /// Contains a value indicating whether http:// scheme is used instead of https://.
        /// </summary>
        [Obsolete("UsesHttp method should be used instead.")]
        protected readonly bool _useHttp; // Use http instead of https
        /// <summary>
        /// Contains the base request URL of this booru.
        /// </summary>
        protected readonly string _baseUrl;
        // TODO: remove this message after removing obsolete constructors.
        private protected const string _deprecationMessage = "Use a contructor that accepts single BooruOptions parameter. Use | (bitwise OR) operator to combine multiple options.";
        private HttpClient _client;
        private readonly string _imageUrlXml, _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl; // URLs for differents endpoints
        // All options are stored in a bit field and can be retrieved using related methods/properties.
        private readonly BooruOptions _options;
        private readonly UrlFormat _format; // URL format
        private const string _userAgentHeaderValue = "Mozilla/5.0 BooruSharp";
        private static readonly Lazy<HttpClient> _lazyClient = new Lazy<HttpClient>(() =>
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", _userAgentHeaderValue);
            return client;
        });
    }
}
