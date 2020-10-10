using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Gets the wiki page of a tag.
        /// </summary>
        /// <param name="query">The tag to get the wiki page for.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="Search.InvalidTags"/>
        public virtual async Task<Search.Wiki.SearchResult> GetWikiAsync(string query)
        {
            if (!HasWikiAPI)
                throw new Search.FeatureUnavailable();

            if (query is null)
                throw new ArgumentNullException(nameof(query));

            var array = await GetJsonAsync<JArray>(
                CreateUrl(_wikiUrl, SearchArg(_format == UrlFormat.Danbooru ? "title" : "query") + query));

            foreach (var token in array)
                if (token["title"].Value<string>() == query)
                    return GetWikiSearchResult(token);

            throw new Search.InvalidTags();
        }
    }
}
