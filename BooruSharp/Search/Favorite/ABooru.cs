using BooruSharp.Search;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        public async Task AddFavorite(int postId)
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
                res = int.Parse(await reader.ReadToEndAsync());
            }
            if (res == 2)
                throw new AuthentificationInvalid();
        }
    }
}
