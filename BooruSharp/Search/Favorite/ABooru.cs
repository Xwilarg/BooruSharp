using BooruSharp.Search;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Add a post to your favorite
        /// </summary>
        /// <remarks>
        /// You must login using SetBooruAuth before using this function
        /// </remarks>
        /// <param name="postId">The ID of the post you want to add to your favorite</param>
        public async Task AddFavoriteAsync(int postId)
        {
            if (_format != UrlFormat.indexPhp)
                throw new FeatureUnavailable();
            if (Auth == null)
                throw new AuthentificationRequired();
            int res;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_baseUrl + "/public/addfav.php?id=" + postId);
            request.Headers["Cookie"] = "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash;
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

        /// <summary>
        /// Remove a post from your favorite
        /// </summary>
        /// <remarks>
        /// You must login using SetBooruAuth before using this function
        /// </remarks>
        /// <param name="postId">The ID of the post you want to remove from your favorite</param>
        public async Task RemoveFavoriteAsync(int postId)
        {
            if (_format != UrlFormat.indexPhp)
                throw new FeatureUnavailable();
            if (Auth == null)
                throw new AuthentificationRequired();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_baseUrl + "/index.php?page=favorites&s=delete&id=" + postId);
            request.Headers["Cookie"] = "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash;
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
