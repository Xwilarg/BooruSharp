using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Get the wiki of a tag
        /// </summary>
        /// <param name="query">The tag you want to get the wiki</param>
        public virtual async Task<Search.Wiki.SearchResult> GetWikiAsync(string query)
        {
            if (!HasWikiAPI())
                throw new Search.FeatureUnavailable();

            if (query == null)
                throw new ArgumentNullException("Argument can't be null");

            var array = JsonConvert.DeserializeObject<JArray>(
                await GetJsonAsync(CreateUrl(_wikiUrl, SearchArg(_format == UrlFormat.danbooru ? "title" : "query") + query)));

            foreach (var token in array)
                if (token["title"].Value<string>() == query)
                    return GetWikiSearchResult(token);

            throw new Search.InvalidTags();
        }
    }
}
