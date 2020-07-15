using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Get the tags related to another one
        /// </summary>
        /// <param name="tag">The tag that the others must be related to</param>
        public virtual async Task<Search.Related.SearchResult[]> GetRelatedAsync(string tag)
        {
            if (_relatedUrl == null)
                throw new Search.FeatureUnavailable();
            var content = (JObject)JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(_relatedUrl, (_format == UrlFormat.danbooru ? "query" : "tags") + "=" + tag)));
            var jsons = (JArray)(_format == UrlFormat.danbooru ? content["tags"] : content[content.Properties().First().Name]);
            Search.Related.SearchResult[] results = new Search.Related.SearchResult[jsons.Count];
            int i = 0;
            foreach (var json in jsons)
            {
                results[i] = GetRelatedSearchResult(json);
                i++;
            }
            return results;
        }
    }
}
