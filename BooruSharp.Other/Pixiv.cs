using BooruSharp.Booru;
using BooruSharp.Search;
using BooruSharp.Search.Post;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            request.Headers.Add("User-Agent", "PixivAndroidApp/5.0.64 (Android 6.0)");
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
            using (HttpClient hc = new HttpClient())
            {
                var http = hc.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
                if (http.StatusCode == HttpStatusCode.BadRequest)
                    throw new AuthentificationInvalid();
                JToken json = http.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }

        public override bool IsSafe()
            => false;

        public override Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            return base.GetRandomPostAsync(tagsArg);
        }

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
