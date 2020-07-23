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
        public Pixiv(string username, string password) : base("app-api.pixiv.net", (UrlFormat)(-1),
            new[] { BooruOptions.noComment, BooruOptions.noFavorite, BooruOptions.noLastComments, BooruOptions.noMultipleRandom, BooruOptions.noPostById,
                BooruOptions.noPostByMd5, BooruOptions.noPostCount, BooruOptions.noRelated, BooruOptions.noTagById, BooruOptions.noWiki })
        {
            var request = new HttpRequestMessage(new HttpMethod("POST"), "https://oauth.secure.pixiv.net/auth/token");
            string time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
            request.Headers.Add("User-Agent", "Mozilla/5.0 BooruSharp");
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
            var http = HttpClient.SendAsync(request).GetAwaiter().GetResult();
            if (http.StatusCode == HttpStatusCode.BadRequest)
                throw new AuthentificationInvalid();
            JToken json = (JToken)JsonConvert.DeserializeObject(http.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            _token = json["response"]["access_token"].Value<string>();
        }

        public override bool IsSafe()
            => false;

        public override async Task<SearchResult> GetPostByIdAsync(int id)
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), _baseUrl + "/v1/illust/detail?illust_id=" + id);
            request.Headers.Add("Authorization", "Bearer " + _token);
            var http = await HttpClient.SendAsync(request);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            JToken post = json["illust"];
            return ParseSearchResult(post);
        }

        public override Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            throw new NotImplementedException();
        }

        public override Task<SearchResult[]> GetRandomPostsAsync(int limit, params string[] tagsArg)
        {
            throw new NotImplementedException();
        }

        public override Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            throw new NotImplementedException();
        }

        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            if (tagsArg.Length == 0)
                throw new InvalidTags();
            var request = new HttpRequestMessage(new HttpMethod("GET"), _baseUrl + "/v1/search/illust?word=" + string.Join("%20", tagsArg.Select(x => Uri.EscapeDataString(x))).ToLower());
            request.Headers.Add("Authorization", "Bearer " + _token);
            var http = await HttpClient.SendAsync(request);
            if (http.StatusCode == HttpStatusCode.NotFound)
                throw new InvalidTags();
            JToken json = (JToken)JsonConvert.DeserializeObject(await http.Content.ReadAsStringAsync());
            Console.WriteLine(json);
            return ParseSearchResults((JArray)json["illusts"]);
        }

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
            return new SearchResult(new Uri(post["image_urls"]["large"].Value<string>()), new Uri(post["image_urls"]["medium"].Value<string>()), new Uri("https://www.pixiv.net/en/artworks/" + post["id"].Value<int>()),
                isNsfw ? Rating.Explicit : Rating.Safe, tags.ToArray(), post["id"].Value<int>(), null, post["height"].Value<int>(), post["width"].Value<int>(), null, null, post["create_date"].Value<DateTime>(),
                null, post["total_bookmarks"].Value<int>(), null);
        }

        private string _token;

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
