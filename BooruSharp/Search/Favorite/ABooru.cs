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
        public virtual async Task AddFavoriteAsync(int postId)
        {
            if (!HasFavoriteAPI())
                throw new FeatureUnavailable();

            if (Auth == null)
                throw new AuthentificationRequired();

            HttpWebRequest request = CreateAuthRequest(_baseUrl + "/public/addfav.php?id=" + postId);
            string response = await GetAuthResponseAndReadToEndAsync(request);

            if (response.Length == 0)
                throw new InvalidPostId();

            int result = int.Parse(response);

            if (result == 2)
                throw new AuthentificationInvalid();
        }

        /// <summary>
        /// Remove a post from your favorite
        /// </summary>
        /// <remarks>
        /// You must login using SetBooruAuth before using this function
        /// </remarks>
        /// <param name="postId">The ID of the post you want to remove from your favorite</param>
        public virtual async Task RemoveFavoriteAsync(int postId)
        {
            if (!HasFavoriteAPI())
                throw new FeatureUnavailable();

            if (Auth == null)
                throw new AuthentificationRequired();

            HttpWebRequest request = CreateAuthRequest(_baseUrl + "/index.php?page=favorites&s=delete&id=" + postId);
            string response = await GetAuthResponseAndReadToEndAsync(request);

            // If the HTML contains the word "Login" we were probably sent back to the authentification form
            if (response.Contains("Login")) 
                throw new AuthentificationInvalid();
        }

        private static async Task<string> GetAuthResponseAndReadToEndAsync(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private HttpWebRequest CreateAuthRequest(string requestUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Headers["Cookie"] = "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash;
            request.UserAgent = _userAgentHeaderValue;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }
    }
}
