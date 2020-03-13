using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Comment.SearchResult[]> GetCommentsAsync(int postId)
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            return await GetCommentsInternalAsync(CreateUrl(commentUrl, SearchArg("post_id") + postId));
        }

        public async Task<Search.Comment.SearchResult[]> GetLastCommentsAsync()
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            return await GetCommentsInternalAsync(CreateUrl(commentUrl));
        }

        private async Task<Search.Comment.SearchResult[]> GetCommentsInternalAsync(string url)
        {
            var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(url));
            Search.Comment.SearchResult[] results = new Search.Comment.SearchResult[jsons.Count];
            int i = 0;
            foreach (var json in jsons)
            {
                results[i] = GetCommentSearchResult(json);
                i++;
            }
            return results;
        }
    }
}
