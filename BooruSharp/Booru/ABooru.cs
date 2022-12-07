﻿using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Defines basic capabilities of a booru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract partial class ABooru<TComment, TPost, TRelated, TTag, TWiki> : IBooru
    {
        /// <inheritdoc/>
        public abstract bool IsSafe { get; }

        private protected virtual Search.Comment.SearchResult GetCommentSearchResult(TComment parsingData)
            => throw new FeatureUnavailable();

        private protected virtual PostSearchResult GetPostSearchResult(TPost parsingData)
            => throw new FeatureUnavailable();

        private protected virtual Search.Related.SearchResult GetRelatedSearchResult(TRelated parsingData)
            => throw new FeatureUnavailable();

        private protected virtual Search.Tag.TagSearchResult GetTagSearchResult(TTag parsingData)
            => throw new FeatureUnavailable();

        private protected virtual Search.Wiki.SearchResult GetWikiSearchResult(TWiki parsingData)
            => throw new FeatureUnavailable();

        /*
        private protected virtual async Task<IEnumerable> GetTagEnumerableSearchResultAsync(Uri url)
        {
            if (TagsUseXml)
            {
                var xml = await GetXmlAsync(url);
                return xml.LastChild;
            }
            else
            {
                return JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));
            }
        }*/

        /// <inheritdoc/>
        public bool HasRelatedAPI => !_options.HasFlag(BooruOptions.NoRelated);

        /// <inheritdoc/>
        public bool HasWikiAPI => !_options.HasFlag(BooruOptions.NoWiki);

        /// <inheritdoc/>
        public bool HasCommentAPI => !_options.HasFlag(BooruOptions.NoComment);

        /// <inheritdoc/>
        public bool HasTagByIdAPI => !_options.HasFlag(BooruOptions.NoTagByID);

        // As a failsafe also check for the availability of comment API.
        /// <inheritdoc/>
        public bool HasSearchLastComment => HasCommentAPI && !_options.HasFlag(BooruOptions.NoLastComments);

        /// <inheritdoc/>
        public bool HasPostByMd5API => !_options.HasFlag(BooruOptions.NoPostByMD5);

        /// <inheritdoc/>
        public bool HasPostByIdAPI => !_options.HasFlag(BooruOptions.NoPostByID);

        /// <inheritdoc/>
        public bool HasPostCountAPI => !_options.HasFlag(BooruOptions.NoPostCount);

        /// <inheritdoc/>
        public bool HasMultipleRandomAPI => !_options.HasFlag(BooruOptions.NoMultipleRandom);

        /// <inheritdoc/>
        public bool HasFavoriteAPI => !_options.HasFlag(BooruOptions.NoFavorite);

        /// <inheritdoc/>
        public bool NoEmptyPostSearch => _options.HasFlag(BooruOptions.NoEmptyPostSearch);

        /// <inheritdoc/>
        public bool NoMoreThanTwoTags => _options.HasFlag(BooruOptions.NoMoreThan2Tags);

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
        /// Gets a value indicating whether the max limit of posts per search is increased (used by Gelbooru).
        /// </summary>
        protected bool SearchIncreasedPostLimit => _options.HasFlag(BooruOptions.LimitOf20000);

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
            _imageUrl = CreateQueryString(format, format == UrlFormat.Philomena ? string.Empty : "post");

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

                case UrlFormat.Philomena:
                    queryString = $"api/v1/json/search/{query}{(string.IsNullOrEmpty(query) ? string.Empty : "s")}";
                    break;

                case UrlFormat.BooruOnRails:
                    queryString = $"api/v3/search/{query}s";
                    break;

                default:
                    return BaseUrl;
            }

            return new Uri(BaseUrl + queryString);
        }

        // TODO: Handle limitrate

        private protected async Task<string> GetJsonAsync(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var message = new HttpRequestMessage(HttpMethod.Get, url);
            PreRequest(message);
            var msg = await HttpClient.SendAsync(message);

            if (msg.StatusCode == HttpStatusCode.Forbidden)
                throw new AuthentificationRequired();

            if (msg.StatusCode == (HttpStatusCode)422)
                throw new TooManyTags();

            msg.EnsureSuccessStatusCode();

            return await msg.Content.ReadAsStringAsync();
        }

        private protected Task<string> GetJsonAsync(Uri url)
        {
            return GetJsonAsync(url.AbsoluteUri);
        }

        private async Task<XmlDocument> GetXmlAsync(string url)
        {
            var xmlDoc = new XmlDocument();
            var xmlString = await GetJsonAsync(url);
            // https://www.key-shortcut.com/en/all-html-entities/all-entities/
            xmlDoc.LoadXml(Regex.Replace(xmlString, "&([a-zA-Z]+);", HttpUtility.HtmlDecode("$1")));
            return xmlDoc;
        }

        private Task<XmlDocument> GetXmlAsync(Uri url)
        {
            return GetXmlAsync(url.AbsoluteUri);
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            HttpResponseMessage msg = await HttpClient.GetAsync(BaseUrl + "index.php?page=post&s=random&" + tags);
            msg.EnsureSuccessStatusCode();
            return HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id");
        }

        private Uri CreateUrl(Uri url, params string[] args)
        {
            var builder = new UriBuilder(url);

            if (builder.Query?.Length > 1)
                builder.Query = builder.Query.Substring(1) + "&" + string.Join("&", args);
            else
                builder.Query = string.Join("&", args);

            return builder.Uri;
        }

        private string TagsToString(string[] tags)
        {
            if (tags == null || !tags.Any())
            {
                // Philomena doesn't support search with no tag so we search for all posts with ID > 0
                return _format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "q=id.gte:0" : "tags=";
            }
            return (_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "q=" : "tags=")
                + string.Join(_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "," : "+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant();
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
        public Uri BaseUrl { get; }

        private HttpClient _client;
        private readonly Uri _imageUrlXml, _imageUrl, _tagUrl, _wikiUrl, _relatedUrl, _commentUrl; // URLs for differents endpoints
        // All options are stored in a bit field and can be retrieved using related methods/properties.
        private readonly BooruOptions _options;
        private readonly UrlFormat _format; // URL format
        private const string _userAgentHeaderValue = "Mozilla/5.0 BooruSharp";
        private protected readonly DateTime _unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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
