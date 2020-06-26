using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Get the wiki of a tag
        /// </summary>
        /// <param name="query">The tag you want to get the wiki</param>
        public async Task<Search.Wiki.SearchResult> GetWikiAsync(string query)
        {
            if (_wikiUrl == null)
                throw new Search.FeatureUnavailable();
            var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(_wikiUrl, SearchArg(_format == UrlFormat.danbooru ? "title" : "query") + query)));
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
