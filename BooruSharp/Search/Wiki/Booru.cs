using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Wiki.SearchResult> GetWikiAsync(string query)
        {
            if (wikiUrl == null)
                throw new Search.FeatureUnavailable();
            var jsons = JsonConvert.DeserializeObject<Search.Wiki.SearchResultJson[]>(await GetJsonAsync(CreateUrl(wikiUrl, SearchArg(wikiSearchUseTitle ? "title" : "query") + query)));
            foreach (var json in jsons)
            {
                if (json.title == query)
                    return new Search.Wiki.SearchResult(
                        json.id,
                        json.title,
                        ParseDateTime(json.creation),
                        ParseDateTime(json.lastUpdate),
                        json.body);
            }
            throw new Search.InvalidTags();
        }
    }
}
