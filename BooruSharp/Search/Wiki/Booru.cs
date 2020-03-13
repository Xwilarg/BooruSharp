using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Wiki.SearchResult> GetWikiAsync(string query)
        {
            if (wikiUrl == null)
                throw new Search.FeatureUnavailable();

            var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(wikiUrl, SearchArg(wikiSearchUseTitle ? "title" : "query") + query)));
            Search.Wiki.SearchResult[] results = new Search.Wiki.SearchResult[jsons.Count];
            int i = 0;
            foreach (var json in jsons)
            {
                if (((JObject)json)["title"].Value<string>() == query)
                    return GetWikiSearchResult(json);
                i++;
            }
            throw new Search.InvalidTags();
        }
    }
}
