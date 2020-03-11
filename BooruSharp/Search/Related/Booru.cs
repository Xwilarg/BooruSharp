using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Related.SearchResult[]> GetRelated(string tag)
        {
            if (relatedUrl == null)
                throw new Search.FeatureUnavailable();
            return await GetRelatedAsync(tag);
        }

        private async Task<Search.Related.SearchResult[]> GetRelatedAsync(string tag)
        {
            var jsons = JsonConvert.DeserializeObject<Search.Related.SearchResultJson[]>(await GetJsonAsync(CreateUrl(relatedUrl, SearchArg("tags") + tag)));
            int i = 0;
            Search.Related.SearchResult[] results = new Search.Related.SearchResult[jsons.Length];
            foreach (var json in jsons)
            {
                results[i] = new Search.Related.SearchResult(json.name, json.count);
                i++;
            }
            return (results);
        }
    }
}
