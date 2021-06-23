using BooruSharp.Search;
using System.Net;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        private const int _invalidAuthErrorCode = 2;

        /// <summary>
        /// Adds a post to your favorites.
        /// </summary>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <param name="postId">The ID of the post you want to add to your favorites.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="InvalidPostId"/>
        public virtual async Task AddFavoriteAsync(int postId)
        {
            if (!HasFavoriteAPI)
                throw new FeatureUnavailable();

            if (Auth == null)
                throw new AuthentificationRequired();

            HttpWebRequest request = CreateAuthRequest(BaseUrl + "public/addfav.php?id=" + postId);
            string response = await GetAuthResponseAndReadToEndAsync(request);

            if (response.Length == 0)
                throw new InvalidPostId();

            int result = int.Parse(response);

            if (result == _invalidAuthErrorCode)
                throw new AuthentificationInvalid();
        }

        /// <summary>
        /// Removes a post from your favorites.
        /// </summary>
        /// <remarks>
        /// You must login using <see cref="Auth"/> property before calling this method.
        /// </remarks>
        /// <param name="postId">The ID of the post you want to remove from your favorites.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="AuthentificationInvalid"/>
        /// <exception cref="AuthentificationRequired"/>
        /// <exception cref="FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task RemoveFavoriteAsync(int postId)
        {
            if (!HasFavoriteAPI)
                throw new FeatureUnavailable();

            if (Auth == null)
                throw new AuthentificationRequired();

            HttpWebRequest request = CreateAuthRequest(BaseUrl + "index.php?page=favorites&s=delete&id=" + postId);
            string response = await GetAuthResponseAndReadToEndAsync(request);

            // If the HTML contains the word "Login" we were probably sent back to the authentification form
            if (response.Contains("Login")) 
                throw new AuthentificationInvalid();
        }
    }
}
