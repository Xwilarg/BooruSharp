using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Comment.SearchResult[]> GetComment(int postId)
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            return (await GetCommentInternal(CreateUrl(commentUrl, SearchArg("post_id") + postId)));
        }

        public async Task<Search.Comment.SearchResult[]> GetLastComment()
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            return (await GetCommentInternal(CreateUrl(commentUrl)));
        }

        private async Task<Search.Comment.SearchResult[]> GetCommentInternal(string url)
        {
            var jsons = JsonConvert.DeserializeObject<Search.Comment.SearchResultJson[]>(await GetJsonAsync(url));
            Search.Comment.SearchResult[] results = new Search.Comment.SearchResult[jsons.Length];
            int i = 0;
            foreach (var json in jsons)
            {
                results[i] = new Search.Comment.SearchResult(
                    json.commentId,
                    json.postId,
                    json.authorId,
                    ParseDateTime(json.creation),
                    json.authorName,
                    json.body);
                i++;
            }
            return (results);
        }
    }
}
