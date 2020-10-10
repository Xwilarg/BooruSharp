using BooruSharp.Search;
using BooruSharp.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        /// Gets a value indicating whether this booru is considered safe (that is, all
        /// posts on this booru have rating of <see cref="Search.Post.Rating.Safe"/>).
        /// </summary>
        public abstract bool IsSafe { get; }

        private protected virtual Search.Comment.SearchResult GetCommentSearchResult(JToken token)
            => throw new FeatureUnavailable();

        private protected virtual Search.Comment.SearchResult GetCommentSearchResult(XmlNode node)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult GetPostSearchResult(JToken token)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult GetPostSearchResult(XmlNode node)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult[] GetPostsSearchResult(JToken token)
            => throw new FeatureUnavailable();

        private protected virtual Search.Post.SearchResult[] GetPostsSearchResult(XmlNode node)
            => throw new FeatureUnavailable();

        private protected virtual Search.Related.SearchResult GetRelatedSearchResult(JToken token)
            => throw new FeatureUnavailable();

        private protected virtual Search.Related.SearchResult GetRelatedSearchResult(XmlNode node)
            => throw new FeatureUnavailable();

        private protected virtual Search.Tag.SearchResult GetTagSearchResult(JToken token)
            => throw new FeatureUnavailable();

        private protected virtual Search.Tag.SearchResult GetTagSearchResult(XmlNode node)
            => throw new FeatureUnavailable();

        private protected virtual Search.Wiki.SearchResult GetWikiSearchResult(JToken token)
            => throw new FeatureUnavailable();

        private protected virtual Search.Wiki.SearchResult GetWikiSearchResult(XmlNode node)
            => throw new FeatureUnavailable();

        private protected virtual JToken ParseFirstPostSearchResult(JToken token)
            => throw new FeatureUnavailable();

        /// <summary>
        /// Gets whether it is possible to search for related tags on this booru.
        /// </summary>
        public bool HasRelatedAPI => !_options.HasFlag(BooruOptions.NoRelated);

        /// <summary>
        /// Gets whether it is possible to search for wiki entries on this booru.
        /// </summary>
        public bool HasWikiAPI => !_options.HasFlag(BooruOptions.NoWiki);

        /// <summary>
        /// Gets whether it is possible to search for comments on this booru.
        /// </summary>
        public bool HasCommentAPI => !_options.HasFlag(BooruOptions.NoComment);

        /// <summary>
        /// Gets whether it is possible to search for tags by their IDs on this booru.
        /// </summary>
        public bool HasTagByIdAPI => !_options.HasFlag(BooruOptions.NoTagByID);

        /// <summary>
        /// Gets whether it is possible to search for the last comments on this booru.
        /// </summary>
        // As a failsafe also check for the availability of comment API.
        public bool HasSearchLastComment => HasCommentAPI && !_options.HasFlag(BooruOptions.NoLastComments);

        /// <summary>
        /// Gets whether it is possible to search for posts by their MD5 on this booru.
        /// </summary>
        public bool HasPostByMd5API => !_options.HasFlag(BooruOptions.NoPostByMD5);

        /// <summary>
        /// Gets whether it is possible to search for posts by their ID on this booru.
        /// </summary>
        public bool HasPostByIdAPI => !_options.HasFlag(BooruOptions.NoPostByID);

        /// <summary>
        /// Gets whether it is possible to get the total number of posts on this booru.
        /// </summary>
        public bool HasPostCountAPI => !_options.HasFlag(BooruOptions.NoPostCount);

        /// <summary>
        /// Gets whether it is possible to get multiple random images on this booru.
        /// </summary>
        public bool HasMultipleRandomAPI => !_options.HasFlag(BooruOptions.NoMultipleRandom);

        /// <summary>
        /// Gets whether this booru supports adding or removing favorite posts.
        /// </summary>
        public bool HasFavoriteAPI => !_options.HasFlag(BooruOptions.NoFavorite);

        /// <summary>
        /// Gets whether this booru can't call post functions without search arguments.
        /// </summary>
        public bool NoEmptyPostSearch => _options.HasFlag(BooruOptions.NoEmptyPostSearch);

        /// <summary>
        /// Gets a value indicating whether http:// scheme is used instead of https://.
        /// </summary>
        protected bool UsesHttp => _options.HasFlag(BooruOptions.UseHttp);

        /// <summary>
        /// Gets a value indicating whether tags API uses XML instead of JSON.
        /// </summary>
        protected bool TagsUseXml => _options.HasFlag(BooruOptions.TagApiXml);

        /// <summary>
        /// Gets a value indicating whether comments API uses XML instead of JSON.
        /// </summary>
        protected bool CommentsUseXml => _options.HasFlag(BooruOptions.CommentApiXml);

        /// <summary>
        /// Gets a value indicating whether searching by more than two tags at once is not allowed.
        /// </summary>
        protected bool NoMoreThanTwoTags => _options.HasFlag(BooruOptions.NoMoreThan2Tags);

        /// <summary>
        /// Gets a value indicating whether the max limit of posts per search is increased (used by Gelbooru).
        /// </summary>
        protected bool SearchIncreasedPostLimit => _options.HasFlag(BooruOptions.LimitOf20000);

        /// <summary>
        /// Initializes a new instance of the <see cref="ABooru"/> class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="format">The URL format to use.</param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected ABooru(string domain, UrlFormat format, BooruOptions options)
        {
            Auth = null;
            HttpClient = null;
            _options = options;

            bool useHttp = UsesHttp; // Cache returned value for faster access.
            BaseUrl = new Uri("http" + (useHttp ? "" : "s") + "://" + domain, UriKind.Absolute);
            _format = format;
            _imageUrl = CreateQueryString(format, "post");

            if (_format == UrlFormat.IndexPhp)
                _imageUrlXml = new Uri(_imageUrl.AbsoluteUri.Replace("json=1", "json=0"));
            else if (_format == UrlFormat.PostIndexJson)
                _imageUrlXml = new Uri(_imageUrl.AbsoluteUri.Replace("index.json", "index.xml"));
            else
                _imageUrlXml = null;

            _tagUrl = CreateQueryString(format, "tag");

            if (HasWikiAPI)
                _wikiUrl = format == UrlFormat.Danbooru
                    ? CreateQueryString(format, "wiki_page")
                    : CreateQueryString(format, "wiki");

            if (HasRelatedAPI)
                _relatedUrl = format == UrlFormat.Danbooru
                    ? CreateQueryString(format, "related_tag")
                    : CreateQueryString(format, "tag", "related");

            if (HasCommentAPI)
                _commentUrl = CreateQueryString(format, "comment");
        }

        /// <summary>
        /// Checks for the booru availability.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> object that, when awaited, will throw an
        /// <see cref="HttpRequestException"/> if service isn't available.
        /// </returns>
        public virtual async Task CheckAvailabilityAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Head, _imageUrlXml ?? _imageUrl);

            using (var response = await GetResponseAsync(request))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        private protected Uri CreateQueryString(UrlFormat format, string query, string squery = "index")
        {
            string queryString;

            switch (format)
            {
                case UrlFormat.PostIndexJson:
                    queryString = query + "/" + squery + ".json";
                    break;

                case UrlFormat.IndexPhp:
                    queryString = "index.php?page=dapi&s=" + query + "&q=index&json=1";
                    break;

                case UrlFormat.Danbooru:
                    queryString = query == "related_tag" ? query + ".json" : query + "s.json";
                    break;

                case UrlFormat.Sankaku:
                    queryString = query == "wiki" ? query : query + "s";
                    break;

                default:
                    return BaseUrl;
            }

            return new Uri(BaseUrl + queryString);
        }

        private protected async Task<T> GetJsonAsync<T>(HttpContent httpContent)
        {
            var jsonString = await httpContent.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        // TODO: Handle limitrate

        private async Task<T> GetJsonAsync<T>(string url)
        {
            using (var response = await GetResponseAsync(url))
            {
                // Must be awaited otherwise we'll get ObjectDisposed exception.
                return await GetJsonAsync<T>(response.Content);
            }
        }

        private Task<T> GetJsonAsync<T>(Uri url)
        {
            return GetJsonAsync<T>(url.AbsoluteUri);
        }

        private async Task<XmlDocument> GetXmlAsync(string url)
        {
            using (var response = await GetResponseAsync(url))
            {
                var xmlDoc = new XmlDocument();
                var xmlString = await response.Content.ReadAsStringAsync();
                xmlDoc.LoadXml(XmlUtils.ReplaceHtmlEntities(xmlString));

                return xmlDoc;
            }
        }

        private Task<XmlDocument> GetXmlAsync(Uri url)
        {
            return GetXmlAsync(url.AbsoluteUri);
        }

        private protected async Task<HttpResponseMessage> GetResponseAsync(string url)
        {
            var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.Forbidden)
                throw new AuthentificationRequired();

            response.EnsureSuccessStatusCode();

            return response;
        }

        private protected Task<HttpResponseMessage> GetResponseAsync(Uri url)
        {
            return GetResponseAsync(url.AbsoluteUri);
        }

        private protected Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage requestMessage)
        {
            return HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            var url = BaseUrl + "index.php?page=post&s=random&tags=" + tags;

            using (var response = await GetResponseAsync(url))
            {
                response.EnsureSuccessStatusCode();

                return HttpUtility.ParseQueryString(response.RequestMessage.RequestUri.Query).Get("id");
            }
        }

        private Uri CreateUrl(Uri url, params string[] args)
        {
            var builder = new UriBuilder(url);

            if (_format == UrlFormat.IndexPhp)
                builder.Query += "&" + string.Join("&", args);
            else
                builder.Query = "?" + string.Join("&", args);

            return builder.Uri;
        }

        private string TagsToString(string[] tags)
        {
            return tags is null ? "" : $"tags={TextUtils.JoinAndEscape(tags)}";
        }

        private string SearchArg(string value)
        {
            return _format == UrlFormat.Danbooru
                ? "search[" + value + "]="
                : value + "=";
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
                if (!(_client is null) && !_client.DefaultRequestHeaders.Contains("User-Agent"))
                    _client.DefaultRequestHeaders.Add("User-Agent", TextUtils.GetUserAgent());
            }
        }

        /// <summary>
        /// Gets the instance of the thread-safe, pseudo-random number generator.
        /// </summary>
        protected static Random Random { get; } = new ThreadSafeRandom();

        /// <summary>
        /// Gets the base API request URL.
        /// </summary>
        protected Uri BaseUrl { get; }

        private HttpClient _client;
        private readonly Uri _imageUrlXml, _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl; // URLs for differents endpoints
        // All options are stored in a bit field and can be retrieved using related methods/properties.
        private readonly BooruOptions _options;
        private readonly UrlFormat _format; // URL format
        private protected readonly DateTime _unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly Lazy<HttpClient> _lazyClient = new Lazy<HttpClient>(() =>
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", TextUtils.GetUserAgent());
            return client;
        });
    }
}
