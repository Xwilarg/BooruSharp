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
    public class Pixiv : ABooru
    {
        public Pixiv() : base("app-api.pixiv.net", (UrlFormat)(-1), BooruOptions.noComment | BooruOptions.noLastComments | BooruOptions.noMultipleRandom |
                BooruOptions.noPostByMd5 | BooruOptions.noRelated | BooruOptions.noTagById | BooruOptions.noWiki | BooruOptions.noEmptyPostSearch)
        {
            AccessToken = null;
        }

        public async Task LoginAsync(string username, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.secure.pixiv.net/auth/token");

            string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
            request.Headers.Add("X-Client-Time", time);

            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(time + _hashSecret));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", null).ToLower();

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

        public async Task LoginAsync(string refreshToken)
        {
            RefreshToken = refreshToken;
            await UpdateTokenAsync();
        }

        public async Task<byte[]> ImageToByteArrayAsync(SearchResult result)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, result.fileUrl);
            request.Headers.Add("Referer", result.postUrl.AbsoluteUri);

            var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> PreviewToByteArrayAsync(SearchResult result)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, result.previewUrl);
            request.Headers.Add("Referer", result.postUrl.AbsoluteUri);

            var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

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

            // Failback
            if (string.IsNullOrWhiteSpace(AccessToken))
                throw new AuthentificationInvalid();

            _refreshTime = DateTime.Now.AddSeconds(responseToken["expires_in"].Value<int>());
        }

        public override bool IsSafe()
            => false;

        public override async Task AddFavoriteAsync(int postId)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v2/illust/bookmark/add");
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
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

        public override async Task RemoveFavoriteAsync(int postId)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v1/illust/bookmark/delete");
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
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

        public override async Task<SearchResult> GetPostByIdAsync(int id)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/v1/illust/detail?illust_id=" + id);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return ParseSearchResult(jsonToken["illust"]);
        }

        public override async Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            // GetPostCountAsync already check for UpdateToken and if parameters are valid
            int max = Math.Min(await GetPostCountAsync(tagsArg), 5000);

            if (max == 0)
                throw new InvalidTags();

            int id = _random.Next(1, max + 1);

            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg) + "&offset=" + id);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            var jsonArray = (JArray)jsonToken["illusts"];
            return ParseSearchResult(jsonArray[0]);
        }

        public override async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.pixiv.net/ajax/search/artworks/" + JoinTagsAndEscapeString(tagsArg));

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return jsonToken["body"]["illustManga"]["total"].Value<int>();
        }

        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();

            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.", nameof(tagsArg));

            await CheckUpdateTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/v1/search/illust?word=" + JoinTagsAndEscapeString(tagsArg));
            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();

            response.EnsureSuccessStatusCode();

            var jsonToken = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            return ParseSearchResults((JArray)jsonToken["illusts"]);
        }

        private static string JoinTagsAndEscapeString(string[] tags)
        {
            string joined = string.Join(" ", tags).ToLower();
            return Uri.EscapeDataString(joined);
        }

        public async Task<SearchResult[]> GetRankingAsync(RankingMode mode = RankingMode.Daily, int offset = 0, DateTime? date = null)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();
            await CheckUpdateTokenAsync();
            string url = _baseUrl + "/v1/illust/ranking?mode=" + rankingModeValues[mode];
            if (offset != 0)
                url += "&offset=" + offset;
            if (date.HasValue)
                url += "&date=" + date.Value.ToString("yyyy-MM-dd");
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!http.IsSuccessStatusCode)
                throw new Exception(http.StatusCode.ToString()); //ToDo: Implement exception
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            return ParseSearchResults((JArray)json["illusts"]);
        }

        public enum RankingMode
        {
            Daily = 0,
            Weekly = 1,
            Monthly = 2,
            Popular_among_male_users = 3,
            Popular_among_female_users = 4,
            Original = 5,
            Manga_Rookie = 6,
            Manga_Daily = 7,
            R18_Daily = 8,
            R18_Weekly = 9,
            R18_Popular_among_male_users = 10,
            R18_Popular_among_female_users = 11,
            R18G_Weekly = 12
        }

        static private Dictionary<RankingMode, string> rankingModeValues = new Dictionary<RankingMode, string>
        {
            { RankingMode.Daily, "day" },
            { RankingMode.Weekly, "week" },
            { RankingMode.Monthly, "month" },
            { RankingMode.Popular_among_male_users, "day_male" },
            { RankingMode.Popular_among_female_users, "day_female" },
            { RankingMode.Original, "week_original" },
            { RankingMode.Manga_Rookie, "week_rookie" },
            { RankingMode.Manga_Daily, "day_manga" },
            { RankingMode.R18_Daily, "day_r18" },
            { RankingMode.R18_Weekly, "week_r18" },
            { RankingMode.R18_Popular_among_male_users, "day_male_r18" },
            { RankingMode.R18_Popular_among_female_users, "day_female_r18" },
            { RankingMode.R18G_Weekly, "week_r18g" }
        };

        private SearchResult[] ParseSearchResults(JArray array)
        {
            return array.Select(ParseSearchResult).ToArray();
        }

        private SearchResult ParseSearchResult(JToken post)
        {
            var tags = post["tags"].Select(x => x["name"].Value<string>()).ToList();

            vai isNsfw = false;
            if (tags.Contains("R-18"))
            {
                tags.Remove("R-18");
                isNsfw = true;
            }
            if (tags.Contains("R-18G"))
            {
                tags.Remove("R-18G");
                isNsfw = true;
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
                tags.ToArray(),
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

        public string AccessToken { private set; get; }
        public string RefreshToken { private set; get; }

        private DateTime _refreshTime;

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private readonly string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private readonly string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private readonly string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
