using BooruSharp.Search;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        public async Task AddFavorite(int postId)
        {
            if (_httpClient == null)
                _httpClient = new HttpClient();
            if (_auth == null)
                throw new AuthentificationRequired();
            _httpClient.DefaultRequestHeaders.Add("Cookie", "user_id=" + _auth.UserId + ";pass_hash=" + _auth.PasswordHash);
            System.Console.WriteLine(_baseUrl + "/public/addfav.php?id=" + postId);
            int res = int.Parse(await _httpClient.GetStringAsync(_baseUrl + "/public/addfav.php?id=" + postId));
            _httpClient.DefaultRequestHeaders.Clear();
            if (res == 2)
                throw new AuthentificationInvalid();
        }
    }
}
