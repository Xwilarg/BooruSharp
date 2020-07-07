using BooruSharp.Search;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        public async Task AddFavoriteAsync(int postId)
        {
            if (_auth == null)
                throw new AuthentificationRequired();
            int res;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_baseUrl + "/public/addfav.php?id=" + postId);
            request.Headers["Cookie"] = "user_id=" + _auth.UserId + ";pass_hash=" + _auth.PasswordHash;
            request.UserAgent = "Mozilla/5.0 BooruSharp";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var answer = await reader.ReadToEndAsync();
                if (answer.Length == 0)
                    throw new InvalidPostId();
                res = int.Parse(answer);
            }
            if (res == 2)
                throw new AuthentificationInvalid();
        }

        public async Task RemoveFavoriteAsync(int postId)
        {
            if (_auth == null)
                throw new AuthentificationRequired();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_baseUrl + "/index.php?page=favorites&s=delete&id=" + postId);
            request.Headers["Cookie"] = "user_id=" + _auth.UserId + ";pass_hash=" + _auth.PasswordHash;
            request.UserAgent = "Mozilla/5.0 BooruSharp";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var answer = await reader.ReadToEndAsync();
                if (answer.Contains("Login")) // If the HTML contains the word "Login" we were probably sent back to the authentification form
                    throw new AuthentificationInvalid();
            }
        }
    }
}
