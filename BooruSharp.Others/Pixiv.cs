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
        {
            AccessToken = null;
        }

        /// <summary>
        /// Sends a login API request using specified user name and password.
        /// </summary>
        /// <param name="username">Pixiv user name.</param>
        /// <param name="password">Pixiv user password.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="HttpRequestException"/>
        public async Task LoginAsync(string username, string password)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

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

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthentificationInvalid();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            var responseToken = jsonToken["response"];

            AccessToken = responseToken["access_token"].Value<string>();
            RefreshToken = responseToken["refresh_token"].Value<string>();
            _refreshTime = DateTime.Now.AddSeconds(responseToken["expires_in"].Value<int>());
        }

        /// <summary>
        /// Sends a login API request using specified refresh token.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="HttpRequestException"/>
        public async Task LoginAsync(string refreshToken)
        {
            RefreshToken = refreshToken;
            await UpdateTokenAsync();
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

            var response = await HttpClient.SendAsync(request);

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

            var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Checks if the <see cref="AccessToken"/> needs to be updated,
        /// and updates it if needed.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="HttpRequestException"/>
        public async Task CheckUpdateTokenAsync()
        {
            if (DateTime.Now > _refreshTime)
                await UpdateTokenAsync();
        }

        private async Task UpdateTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.secure.pixiv.net/auth/token");
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "get_secure_url", "1" },
                    { "client_id", _clientID },
                    { "client_secret", _clientSecret},
                    { "grant_type", "refresh_token" },
                    { "refresh_token", RefreshToken }
                });

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthentificationInvalid();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            var responseToken = jsonToken["response"];

            AccessToken = responseToken["access_token"].Value<string>();
            _refreshTime = DateTime.Now.AddSeconds(responseToken["expires_in"].Value<int>());
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <summary>
        /// Adds a post with the specified ID to favorites.
        /// </summary>
        /// <param name="postId">The ID of the post to add to favorites.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="InvalidPostId"/>
        public override async Task AddFavoriteAsync(int postId)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/v2/illust/bookmark/add");
            AddAuthorizationHeader(request);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "illust_id", postId.ToString() },
                    { "restrict", "public" }
                });

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidPostId();

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Removes a post with the specified ID from favorites.
        /// </summary>
        /// <param name="postId">The ID of the post to remove from favorites.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="HttpRequestException"/>
        /// <exception cref="InvalidPostId"/>
        public override async Task RemoveFavoriteAsync(int postId)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + "/v1/illust/bookmark/delete");
            AddAuthorizationHeader(request);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    { "illust_id", postId.ToString() }
                });

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidPostId("There is no post with this ID in your bookmarks");

            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        public override async Task<SearchResult> GetPostByIdAsync(int id)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "/v1/illust/detail?illust_id=" + id);
            AddAuthorizationHeader(request);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return ParseSearchResult(jsonToken["illust"]);
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            // GetPostCountAsync already check for UpdateToken and if parameters are valid
            int max = Math.Min(await GetPostCountAsync(tagsArg), 5000);

            if (max == 0)
                throw new InvalidTags();

            int id = Random.Next(1, max + 1);
            var requestUrl = BaseUrl + "/v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg) + "&offset=" + id;

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            AddAuthorizationHeader(request);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            var jsonArray = (JArray)jsonToken["illusts"];
            return ParseSearchResult(jsonArray[0]);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationRequired"/>
        public override async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            await CheckUpdateTokenAsync();

            var requestUrl = "https://www.pixiv.net/ajax/search/artworks/" + JoinTagsAndEscapeString(tagsArg);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return jsonToken["body"]["illustManga"]["total"].Value<int>();
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            await CheckUpdateTokenAsync();

            string requestUrl = BaseUrl + "/v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            AddAuthorizationHeader(request);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return ParseSearchResults((JArray)jsonToken["illusts"]);
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

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
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

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private readonly string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private readonly string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private readonly string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
