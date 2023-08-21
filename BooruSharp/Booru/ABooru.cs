using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Defines basic capabilities of a booru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract partial class ABooru
    {
        public virtual bool CanSearchWithNoTag => true;
        public virtual bool HasPostByIdAPI => true;
        public virtual int MaxNumberOfTags => -1;

        public virtual Uri FileBaseUrl => APIBaseUrl;
        public virtual Uri PreviewBaseUrl => APIBaseUrl;
        public virtual Uri PostBaseUrl => APIBaseUrl;
        public virtual Uri SampleBaseUrl => APIBaseUrl;

        /// <inheritdoc/>
        public abstract bool IsSafe { get; }

        private protected virtual Search.Comment.SearchResult GetCommentSearchResultAsync(Uri uri)
            => throw new FeatureUnavailable();

        private protected virtual Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
            => throw new FeatureUnavailable();

        private protected virtual Search.Related.SearchResult GetRelatedSearchResultAsync(Uri uri)
            => throw new FeatureUnavailable();

        private protected virtual Search.Tag.TagSearchResult GetTagSearchResultAsync(Uri uri)
            => throw new FeatureUnavailable();

        private protected virtual Search.Wiki.SearchResult GetWikiSearchResultAsync(Uri uri)
            => throw new FeatureUnavailable();

        /// <inheritdoc/>
        public async Task CheckAvailabilityAsync()
        {
            await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, _imageUrl));
        }

        /// <summary>
        /// Add booru authentification to current request
        /// </summary>
        /// <param name="message">The request that is going to be sent</param>
        protected virtual void PreRequest(HttpRequestMessage message)
        { }

        /// <summary>
        /// Create an URL to request the specified API
        /// </summary>
        /// <param name="query">Main query</param>
        /// <param name="squery">Sub query</param>
        protected abstract Uri CreateQueryString(string query, string squery = "index");

        /// <summary>
        /// Create the Uri to request a post from the API
        /// </summary>
        /// <param name="tags">List of tags sent by the user</param>
        protected abstract Task<Uri> CreateRandomPostUriAsync(string[] tags);
        protected abstract Task<Uri> CreatePostByIdUriAsync(int id);

        protected virtual async Task<T[]> GetDataArray<T>(Uri url)
        {
            return JsonSerializer.Deserialize<T[]>(await GetJsonAsync(url), new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            });
        }

        protected virtual async Task<T> GetDataAsync<T>(Uri url)
        {
            var res = await GetJsonAsync(url);
            if (string.IsNullOrEmpty(res))
            {
                throw new InvalidPostException();
            }
            return JsonSerializer.Deserialize<T>(res, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            });
        }

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
        protected ABooru(string apiDomain)
        {
            Auth = null;
            HttpClient = null;

            APIBaseUrl = new Uri("https://" + apiDomain, UriKind.Absolute);
            _imageUrl = CreateQueryString("post");

            _tagUrl = CreateQueryString("tag");

            _wikiUrl = CreateQueryString("wiki");

            _relatedUrl = CreateQueryString("tag", "related");

            _commentUrl = CreateQueryString("comment");
        }

        // TODO: Handle limitrate

        private protected Task<string> GetJsonAsync(Uri url)
        {
            return GetJsonAsync(url.AbsoluteUri);
        }

        private protected async Task<string> GetJsonAsync(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var message = new HttpRequestMessage(HttpMethod.Get, url);
            PreRequest(message);
            var msg = await HttpClient.SendAsync(message);

            if (msg.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidPostException();
            }

            msg.EnsureSuccessStatusCode();

            return await msg.Content.ReadAsStringAsync();
        }

        /*
        private string SearchArg(string value)
        {
            return _format == UrlFormat.Danbooru
                ? "search[" + value + "]="
                : value + "=";
        }*/

        /// <summary>
        /// Gets or sets authentication credentials.
        /// </summary>
        public BooruAuth Auth { set; get; }

        /// <summary>
        /// Sets the <see cref="System.Net.Http.HttpClient"/> instance that will be used
        /// to make requests. If <see langword="null"/> or left unset, the default
        /// <see cref="System.Net.Http.HttpClient"/> instance will be used.
        /// <para>This property can only be read in <see cref="ABooru"/> subclasses.</para>
        /// We advice you to disable the cookies and set automatic decompression to GZip and Deflate
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
        /// Gets the instance of the thread-safe, pseudo-random number generator.
        /// </summary>
        protected static Random Random { get; } = new ThreadSafeRandom();

        /// <summary>
        /// Gets the base API request URL.
        /// </summary>
        public Uri APIBaseUrl { get; }

        private HttpClient _client;
        protected readonly Uri _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl; // URLs for differents endpoints
        // All options are stored in a bit field and can be retrieved using related methods/properties.
        private const string _userAgentHeaderValue = "Mozilla/5.0 BooruSharp";
        private protected readonly DateTime _unixTime = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly Lazy<HttpClient> _lazyClient = new Lazy<HttpClient>(() =>
        {
            var handler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", _userAgentHeaderValue);
            return client;
        });
    }
}
