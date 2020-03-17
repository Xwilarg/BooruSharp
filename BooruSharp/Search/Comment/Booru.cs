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
            if (commentUrl == null || !searchLastComment)
                throw new Search.FeatureUnavailable();
            return await GetCommentsInternalAsync(CreateUrl(commentUrl));
        }

        private async Task<Search.Comment.SearchResult[]> GetCommentsInternalAsync(string url)
        {
            Search.Comment.SearchResult[] results;
            if (format == UrlFormat.indexPhp)
            {
                var xml = await GetXmlAsync(url);
                results = new Search.Comment.SearchResult[xml.LastChild.ChildNodes.Count];
                int i = 0;
                foreach (var node in xml.LastChild)
                {
                    results[i] = GetCommentSearchResult(node);
                    i++;
                }
            }
            else
            {
                var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(url));
                results = new Search.Comment.SearchResult[jsons.Count];
                int i = 0;
                foreach (var json in jsons)
                {
                    results[i] = GetCommentSearchResult(json);
                    i++;
                }
            }
            return results;
        }
    }
}
