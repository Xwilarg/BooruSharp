using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Related.SearchResult[]> GetRelatedAsync(string tag)
        {
            if (relatedUrl == null)
                throw new Search.FeatureUnavailable();
            return await GetRelatedInternalAsync(tag);
        }

        private async Task<Search.Related.SearchResult[]> GetRelatedInternalAsync(string tag)
        {
            var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(relatedUrl, SearchArg("tags") + tag)));
            Search.Related.SearchResult[] results = new Search.Related.SearchResult[jsons.Count];
            int i = 0;
            foreach (var json in jsons)
            {
                results[i] = GetRelatedSearchResult(json);
                i++;
            }
            return (results);
        }
    }
}
