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
using System.Threading;
using System.Threading.Tasks;

namespace BooruSharp.Others
{
    /// <summary>
    /// Pixiv.
    /// <para>https://www.pixiv.net/</para>
    /// </summary>
    public class Pixiv : ABooru, IDisposable
    {
        private const string _appVersion = "5.0.212";

        /// <summary>
        /// Initializes a new instance of the <see cref="Pixiv"/> class.
        /// </summary>
        public Pixiv()
            : base("app-api.pixiv.net", UrlFormat.None, BooruOptions.NoComment | BooruOptions.NoLastComments
                  | BooruOptions.NoMultipleRandom | BooruOptions.NoPostByMD5 | BooruOptions.NoRelated | BooruOptions.NoTagByID
                  | BooruOptions.NoWiki | BooruOptions.NoEmptyPostSearch)
        {
            AccessToken = null;
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        public async override Task CheckAvailabilityAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Head, "https://www.pixiv.net/ajax/search/artworks/スク水");

            using (var response = await GetResponseAsync(request))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        /// Sends a login API request using specified user name and password.
        /// </summary>
        /// <param name="username">Pixiv user name.</param>
        /// <param name="password">Pixiv user password.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="HttpRequestException"/>
        public Task LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or whitespace", nameof(password));

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
                    { "username", username },
                    { "password", password }
                });

            return SendLoginRequestAsync(request);
        }

        /// <summary>
        /// Sends a login API request using specified refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token for your current Pixiv session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="HttpRequestException"/>
        public Task LoginAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException($"'{nameof(refreshToken)}' cannot be null or whitespace", nameof(refreshToken));

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

            using (var response = await GetResponseAsync(request))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
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

            using (var response = await GetResponseAsync(request))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        /// <summary>
        /// Adds a post with the specified ID to favorites.
        /// </summary>
        /// <remarks>
        /// You must login once using <see cref="LoginAsync(string)"/> or
        /// <see cref="LoginAsync(string, string)"/> before calling this method.
        /// </remarks>
        /// <param name="postId">The ID of the post to add to favorites.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="InvalidPostId"/>
        public override async Task AddFavoriteAsync(int postId)
        {
            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "v2/illust/bookmark/add");
            AddAuthorizationHeader(request);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "illust_id", postId.ToString() },
                    { "restrict", "public" }
                });

            using (var response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidPostId();

                response.EnsureSuccessStatusCode();
            }
        }

        /// <summary>
        /// Removes a post with the specified ID from favorites.
        /// </summary>
        /// <remarks>
        /// You must login once using <see cref="LoginAsync(string)"/> or
        /// <see cref="LoginAsync(string, string)"/> before calling this method.
        /// </remarks>
        /// <param name="postId">The ID of the post to remove from favorites.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="InvalidPostId"/>
        public override async Task RemoveFavoriteAsync(int postId)
        {
            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "v1/illust/bookmark/delete");
            AddAuthorizationHeader(request);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "illust_id", postId.ToString() }
                });

            using (var response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidPostId("There is no post with this ID in your bookmarks");

                response.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login once using <see cref="LoginAsync(string)"/> or
        /// <see cref="LoginAsync(string, string)"/> before calling this method.
        /// </remarks>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="InvalidPostId"/>
        public override async Task<SearchResult> GetPostByIdAsync(int id)
        {
            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "v1/illust/detail?illust_id=" + id);
            AddAuthorizationHeader(request);

            using (var response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidPostId();

                response.EnsureSuccessStatusCode();

                var jsonToken = await GetJsonAsync<JToken>(response.Content);
                return ParseSearchResult(jsonToken["illust"]);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login once using <see cref="LoginAsync(string)"/> or
        /// <see cref="LoginAsync(string, string)"/> before calling this method.
        /// </remarks>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            await CheckUpdateTokenAsync();

            int max = Math.Min(await GetPostCountAsync(tagsArg), 5000);

            if (max == 0)
                throw new InvalidTags();

            int id = Random.Next(1, max + 1);
            var requestUrl = BaseUrl + "v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg) + "&offset=" + id;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            AddAuthorizationHeader(request);

            using (var response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidTags();

                response.EnsureSuccessStatusCode();

                var jsonToken = await GetJsonAsync<JToken>(response.Content);
                var jsonArray = (JArray)jsonToken["illusts"];
                return ParseSearchResult(jsonArray[0]);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        public override async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            var requestUrl = "https://www.pixiv.net/ajax/search/artworks/" + JoinTagsAndEscapeString(tagsArg);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            using (var response = await GetResponseAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var jsonToken = await GetJsonAsync<JToken>(response.Content);
                return jsonToken["body"]["illustManga"]["total"].Value<int>();
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// You must login once using <see cref="LoginAsync(string)"/> or
        /// <see cref="LoginAsync(string, string)"/> before calling this method.
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
            AddAuthorizationHeader(request);

            using (var response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidTags();

                response.EnsureSuccessStatusCode();

                var jsonToken = await GetJsonAsync<JToken>(response.Content);
                return ParseSearchResults((JArray)jsonToken["illusts"]);
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Pixiv"/> class.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Pixiv"/> class
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _loginSemaphore.Dispose();
                }

                _disposedValue = true;
            }
        }

        // Make sure to call and await CheckUpdateTokenAsync
        // first before calling this method.
        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            _loginSemaphore.Wait();
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            _loginSemaphore.Release();
        }

        private static void AddUserAgentHeader(HttpRequestMessage request)
        {
            // <https://github.com/akameco/pixiv-app-api/pull/42/commits/9a16f5f80e483a3518b58d4e519a7cc09e51309f#diff-a2a171449d862fe29692ce031981047d7ab755ae7f84c707aef80701b3ea0c80>
            request.Headers.Add("User-Agent", $"PixivAndroidApp/{_appVersion} (Android 6.0; PixivBot)");
        }

        private async Task CheckUpdateTokenAsync()
        {
            await _loginSemaphore.WaitAsync();
            var refreshToken = RefreshToken;
            _loginSemaphore.Release();

            if (refreshToken == null)
                throw new AuthentificationRequired();

            if (DateTime.Now > _refreshTime)
                await LoginAsync(refreshToken);
        }

        private async Task SendLoginRequestAsync(HttpRequestMessage requestMessage)
        {
            AddUserAgentHeader(requestMessage);

            try
            {
                await _loginSemaphore.WaitAsync();

                JToken responseToken;

                using (var response = await GetResponseAsync(requestMessage))
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                        throw new AuthentificationInvalid();

                    response.EnsureSuccessStatusCode();

                    var jsonToken = await GetJsonAsync<JToken>(response.Content);
                    responseToken = jsonToken["response"];
                }

                AccessToken = responseToken["access_token"].Value<string>();
                RefreshToken = responseToken["refresh_token"].Value<string>();
                _refreshTime = DateTime.Now.AddSeconds(responseToken["expires_in"].Value<int>());
            }
            finally
            {
                // Make sure semaphore is released in case exception is thrown.
                _loginSemaphore.Release();
            }
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

        /// <summary>
        /// Gets the access token associated with the current Pixiv session.
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the refresh token associated with the current Pixiv session.
        /// </summary>
        public string RefreshToken { get; private set; }

        private DateTime _refreshTime;
        private bool _disposedValue;
        private readonly SemaphoreSlim _loginSemaphore = new SemaphoreSlim(1);

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private readonly string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private readonly string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private readonly string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
