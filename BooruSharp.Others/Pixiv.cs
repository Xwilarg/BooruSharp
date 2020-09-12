using BooruSharp.Booru;
using BooruSharp.Search;
using BooruSharp.Search.Post;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BooruSharp.Others
{
    /// <summary>
    /// Pixiv.
    /// <para>https://www.pixiv.net/</para>
    /// </summary>
    public class Pixiv : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pixiv"/> class.
        /// </summary>
        public Pixiv()
            : base("app-api.pixiv.net", UrlFormat.None, BooruOptions.NoComment | BooruOptions.NoLastComments
                  | BooruOptions.NoMultipleRandom | BooruOptions.NoPostByMD5 | BooruOptions.NoRelated | BooruOptions.NoTagByID
                  | BooruOptions.NoWiki | BooruOptions.NoEmptyPostSearch)
        { }

        private readonly struct SessionInfo
        {
            public SessionInfo(string accessToken, string refreshToken, DateTime expirationDate)
            {
                AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
                RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
                ExpirationDate = expirationDate;
            }

            public string AccessToken { get; }

            public string RefreshToken { get; }

            public DateTime ExpirationDate { get; }
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <summary>
        /// Sends a login API request using specified refresh token.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="HttpRequestException"/>
        public Task LoginAsync(string refreshToken)
        {
            lock (_sessionInfoLock)
            {
                _sessionInfoTask = Task.Run(() => GetSessionInfoAsync(refreshToken));
                return _sessionInfoTask;
            }
        }

        /// <summary>
        /// Downloads the <paramref name="result"/>'s image as an array of bytes.
        /// </summary>
        /// <param name="result">The post to get the image from.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException"/>
        public async Task<byte[]> ImageToByteArrayAsync(SearchResult result)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, result.FileUrl);
            request.Headers.Add("Referer", result.PostUrl.AbsoluteUri);

            var response = await GetResponseAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Downloads the <paramref name="result"/>'s preview image as an array of bytes.
        /// </summary>
        /// <param name="result">The post to get the preview image from.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException"/>
        public async Task<byte[]> PreviewToByteArrayAsync(SearchResult result)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, result.PreviewUrl);
            request.Headers.Add("Referer", result.PostUrl.AbsoluteUri);

            var response = await GetResponseAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Checks if the authentication token needs to be updated,
        /// and updates it if needed.
        /// </summary>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="HttpRequestException"/>
        // TODO: does this have to be public?
        public async Task CheckUpdateTokenAsync()
        {
            // Create a local copy here in case session info somehow becomes
            // null after the null check but before awaiting the task.
            Task<SessionInfo> localCopy;
            lock (_sessionInfoLock)
            {
                localCopy = _sessionInfoTask;
            }

            if (localCopy == null)
                throw new AuthentificationRequired();

            var sessionInfo = await localCopy;

            if (DateTime.Now > sessionInfo.ExpirationDate)
            {
                lock (_sessionInfoLock)
                {
                    _sessionInfoTask = Task.Run(() => GetSessionInfoAsync(sessionInfo.RefreshToken));
                }
            }
        }

        /// <inheritdoc/>
        public override async Task AddFavoriteAsync(int postId)
        {
            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "v2/illust/bookmark/add");
            await AddAuthorizationHeaderAsync(request);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "illust_id", postId.ToString() },
                    { "restrict", "public" }
                });

            var response = await GetResponseAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidPostId();

            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidPostId"/>
        public override async Task RemoveFavoriteAsync(int postId)
        {
            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "v1/illust/bookmark/delete");
            await AddAuthorizationHeaderAsync(request);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "illust_id", postId.ToString() }
                });

            var response = await GetResponseAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidPostId("There is no post with this ID in your bookmarks");

            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult> GetPostByIdAsync(int id)
        {
            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "v1/illust/detail?illust_id=" + id);
            await AddAuthorizationHeaderAsync(request);

            var response = await GetResponseAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return ParseSearchResult(jsonToken["illust"]);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            // GetPostCountAsync already check for UpdateToken and if parameters are valid
            int max = Math.Min(await GetPostCountAsync(tagsArg), 5000);

            if (max == 0)
                throw new InvalidTags();

            int id = Random.Next(1, max + 1);
            var requestUrl = BaseUrl + "v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg) + "&offset=" + id;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            await AddAuthorizationHeaderAsync(request);

            var response = await GetResponseAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            var jsonArray = (JArray)jsonToken["illusts"];
            return ParseSearchResult(jsonArray[0]);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        public override async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            // TODO: does this have to be updated?
            // Doesn't seem like session info is used in this method.
            await CheckUpdateTokenAsync();

            var requestUrl = "https://www.pixiv.net/ajax/search/artworks/" + JoinTagsAndEscapeString(tagsArg);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await GetResponseAsync(request);

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return jsonToken["body"]["illustManga"]["total"].Value<int>();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            await CheckUpdateTokenAsync();

            string requestUrl = BaseUrl + "v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            await AddAuthorizationHeaderAsync(request);

            var response = await GetResponseAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return ParseSearchResults((JArray)jsonToken["illusts"]);
        }

        private Task<SessionInfo> GetSessionInfoAsync(string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.secure.pixiv.net/auth/token");
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "get_secure_url", "1" },
                    { "client_id", _clientID },
                    { "client_secret", _clientSecret},
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                });

            return SendLoginRequestAsync(request);
        }

        private Task<SessionInfo> GetSessionInfoAsync(string userId, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.secure.pixiv.net/auth/token");

            string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
            request.Headers.Add("X-Client-Time", time);

            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(time + _hashSecret));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", null).ToLowerInvariant();

                request.Headers.Add("X-Client-Hash", hashString);
            }

            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "get_secure_url", "1" },
                    { "client_id", _clientID },
                    { "client_secret", _clientSecret},
                    { "grant_type", "password" },
                    { "username", userId },
                    { "password", password }
                });

            return SendLoginRequestAsync(request);
        }

        private async Task<SessionInfo> SendLoginRequestAsync(HttpRequestMessage requestMessage)
        {
            var response = await GetResponseAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthentificationInvalid();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            var responseToken = jsonToken["response"];

            return new SessionInfo(
                responseToken["access_token"].Value<string>(),
                responseToken["refresh_token"].Value<string>(),
                DateTime.Now.AddSeconds(responseToken["expires_in"].Value<int>()));
        }

        private Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage requestMessage)
        {
            return HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        }

        // Make sure to call and await CheckUpdateTokenAsync
        // first before calling this method.
        private async Task AddAuthorizationHeaderAsync(HttpRequestMessage request)
        {
            Task<SessionInfo> localCopy;
            lock (_sessionInfoLock)
            {
                localCopy = _sessionInfoTask;
            }

            var sessionInfo = await localCopy;
            request.Headers.Add("Authorization", "Bearer " + sessionInfo.AccessToken);
        }

        private static string JoinTagsAndEscapeString(string[] tags)
        {
            string joined = string.Join(" ", tags).ToLowerInvariant();
            return Uri.EscapeDataString(joined);
        }

        private SearchResult[] ParseSearchResults(JArray array)
        {
            return array.Select(ParseSearchResult).ToArray();
        }

        private SearchResult ParseSearchResult(JToken post)
        {
            var tags = post["tags"].Select(x => x["name"].Value<string>()).ToList();

            bool isNsfw = tags.Contains("R-18");
            if (isNsfw)
            {
                tags.Remove("R-18");
            }

            var originalImageUrlToken =
                 // If there's multiple image URLs, get the first one.
                 (post["meta_pages"]?.FirstOrDefault()?["image_urls"]?["original"])
                 // If there's only one original image URL, use that one.
                 ?? (post["meta_single_page"]?["original_image_url"])
                 // Fallback to large image in case neither of the above succeeds.
                 ?? (post["image_urls"]["large"]);

            return new SearchResult(
                new Uri(originalImageUrlToken.Value<string>()),
                new Uri(post["image_urls"]["medium"].Value<string>()),
                new Uri("https://www.pixiv.net/en/artworks/" + post["id"].Value<int>()),
                isNsfw ? Rating.Explicit : Rating.Safe,
                tags,
                post["id"].Value<int>(),
                null,
                post["height"].Value<int>(),
                post["width"].Value<int>(),
                null,
                null,
                post["create_date"].Value<DateTime>(),
                null,
                post["total_bookmarks"].Value<int>(),
                null);
        }

        /// <inheritdoc/>
        public override BooruAuth Auth
        {
            get => base.Auth;
            set
            {
                lock (_sessionInfoLock)
                {
                    if (value != null)
                    {
                        // Don't start a new task if the new user ID and password are the same.
                        if (!value.Equals(Auth))
                        {
                            _sessionInfoTask = Task.Run(() => GetSessionInfoAsync(value.UserId, value.PasswordHash));
                        }
                    }
                    else
                    {
                        _sessionInfoTask = null;
                    }
                }

                base.Auth = value;
            }
        }

        private Task<SessionInfo> _sessionInfoTask;
        private readonly object _sessionInfoLock = new object();

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private readonly string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private readonly string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private readonly string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
