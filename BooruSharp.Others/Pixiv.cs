﻿using BooruSharp.Booru;
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
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(time + _hashSecret));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                request.Headers.Add("X-Client-Hash", builder.ToString());
            }
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "get_secure_url", "1" },
                { "client_id", _clientID },
                { "client_secret", _clientSecret},
                { "grant_type", "password" },
                { "username", username },
                { "password", password }
            };
            request.Content = new FormUrlEncodedContent(data);
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthentificationInvalid();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            AccessToken = json["response"]["access_token"].Value<string>();
            RefreshToken = json["response"]["refresh_token"].Value<string>();
            _refreshTime = DateTime.Now.AddSeconds(json["response"]["expires_in"].Value<int>());
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
            return await (await HttpClient.SendAsync(request)).Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> PreviewToByteArrayAsync(SearchResult result)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, result.fileUrl);
            request.Headers.Add("Referer", result.previewUrl.AbsoluteUri);
            return await (await HttpClient.SendAsync(request)).Content.ReadAsByteArrayAsync();
        }

        public async Task CheckUpdateTokenAsync()
        {
            if (DateTime.Now > _refreshTime)
                await UpdateTokenAsync();
        }

        private async Task UpdateTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.secure.pixiv.net/auth/token");
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "get_secure_url", "1" },
                { "client_id", _clientID },
                { "client_secret", _clientSecret},
                { "grant_type", "refresh_token" },
                { "refresh_token", RefreshToken }
            };
            request.Content = new FormUrlEncodedContent(data);
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthentificationInvalid();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            AccessToken = json["response"]["access_token"].Value<string>();
            if (string.IsNullOrWhiteSpace(AccessToken))
                throw new AuthentificationInvalid();
            _refreshTime = DateTime.Now.AddSeconds(json["response"]["expires_in"].Value<int>());
        }

        public override bool IsSafe()
            => false;

        public override async Task AddFavoriteAsync(int postId)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v2/illust/bookmark/add");
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "illust_id", postId.ToString() },
                { "restrict", "public" }
            });
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidPostId();
        }

        public override async Task RemoveFavoriteAsync(int postId)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();
            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v1/illust/bookmark/delete");
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "illust_id", postId.ToString() }
            });
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidPostId("There is no post with this ID in your bookmarks");
        }

        public override async Task<SearchResult> GetPostByIdAsync(int id)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();
            await CheckUpdateTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/v1/illust/detail?illust_id=" + id);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            JToken post = json["illust"];
            return ParseSearchResult(post);
        }

        public override async Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            int max = await GetPostCountAsync(tagsArg); // GetPostCountAsync already check for UpdateToken and if parameters are valid
            if (max == 0)
                throw new InvalidTags();
            if (max > 5000)
                max = 5000;
            int id = _random.Next(1, max + 1);
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/v1/search/illust?word=" + string.Join("%20", tagsArg.Select(x => Uri.EscapeDataString(x))).ToLower() + "&offset=" + id);
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            return ParseSearchResult(((JArray)json["illusts"])[0]);
        }

        public override async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();
            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.");
            await CheckUpdateTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.pixiv.net/ajax/search/artworks/" + string.Join("%20", tagsArg.Select(x => Uri.EscapeDataString(x))).ToLower());
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            return json["body"]["illustManga"]["total"].Value<int>();
        }

        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            if (AccessToken == null)
                throw new AuthentificationRequired();
            if (tagsArg.Length == 0)
                throw new ArgumentException("You must provide at least one tag.");
            await CheckUpdateTokenAsync();
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl + "/v1/search/illust?word=" + string.Join("%20", tagsArg.Select(x => Uri.EscapeDataString(x))).ToLower());
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            var http = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            return ParseSearchResults((JArray)json["illusts"]);
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
            Daily,
            Weekly,
            Monthly,
            PopularAmongMaleUsers,
            PopularAmongFemaleUsers,
            Original,
            MangaRookie,
            MangaDaily,
            DailyR18,
            WeeklyR18,
            PopularAmongMaleUsersR18,
            PopularAmongFemaleUsersR18,
            WeeklyR18G
        }

        static private Dictionary<RankingMode, string> rankingModeValues = new Dictionary<RankingMode, string>
        {
            { RankingMode.Daily, "day" },
            { RankingMode.Weekly, "week" },
            { RankingMode.Monthly, "month" },
            { RankingMode.PopularAmongMaleUsers, "day_male" },
            { RankingMode.PopularAmongFemaleUsers, "day_female" },
            { RankingMode.Original, "week_original" },
            { RankingMode.MangaRookie, "week_rookie" },
            { RankingMode.MangaDaily, "day_manga" },
            { RankingMode.DailyR18, "day_r18" },
            { RankingMode.WeeklyR18, "week_r18" },
            { RankingMode.PopularAmongMaleUsersR18, "day_male_r18" },
            { RankingMode.PopularAmongFemaleUsersR18, "day_female_r18" },
            { RankingMode.WeeklyR18G, "week_r18g" }
        };

        private SearchResult[] ParseSearchResults(JArray array)
        {
            List<SearchResult> results = new List<SearchResult>();
            foreach (JToken token in array)
                results.Add(ParseSearchResult(token));
            return results.ToArray();
        }

        private SearchResult ParseSearchResult(JToken post)
        {
            List<string> tags = post["tags"].Select(x => x["name"].Value<string>()).ToList();
            bool isNsfw = false;
            if (tags.Contains("R-18"))
            {
                isNsfw = true;
                tags.Remove("R-18");
            }
            if (tags.Contains("R-18G"))
            {
                isNsfw = true;
                tags.Remove("R-18G");
            }
            return new SearchResult(new Uri(post["image_urls"]["large"].Value<string>()), new Uri(post["image_urls"]["medium"].Value<string>()), new Uri("https://www.pixiv.net/en/artworks/" + post["id"].Value<int>()),
                isNsfw ? Rating.Explicit : Rating.Safe, tags.ToArray(), post["id"].Value<int>(), null, post["height"].Value<int>(), post["width"].Value<int>(), null, null, post["create_date"].Value<DateTime>(),
                null, post["total_bookmarks"].Value<int>(), null);
        }

        public string AccessToken { private set; get; }
        public string RefreshToken { private set; get; }
        private DateTime _refreshTime;

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
