using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        public async Task<Search.Comment.SearchResult[]> GetCommentsAsync(int postId)
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            string url = CreateUrl(commentUrl, SearchArg("post_id") + postId);
            List<Search.Comment.SearchResult> results = new List<Search.Comment.SearchResult>();
            if (commentUseXml)
            {
                var xml = await GetXmlAsync(url);
                foreach (var node in xml.LastChild)
                {
                    var result = GetCommentSearchResult(node);
                    if (result.postId == postId)
                        results.Add(result);
                }
            }
            else
            {
                var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(url));
                foreach (var json in jsons)
                {
                    var result = GetCommentSearchResult(json);
                    if (result.postId == postId)
                        results.Add(result);
                }
            }
            return results.ToArray();
        }

        public async Task<Search.Comment.SearchResult[]> GetLastCommentsAsync()
        {
            if (commentUrl == null || !searchLastComment)
                throw new Search.FeatureUnavailable();
            string url = CreateUrl(commentUrl);
            Search.Comment.SearchResult[] results;
            if (commentUseXml)
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
