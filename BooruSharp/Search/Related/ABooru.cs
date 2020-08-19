using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
            if (!HasRelatedAPI())
                throw new Search.FeatureUnavailable();

            if (tag == null)
                throw new ArgumentNullException("Argument can't be null");

            bool isDanbooruFormat = _format == UrlFormat.danbooru;

            var content = JsonConvert.DeserializeObject<JObject>(
                await GetJsonAsync(CreateUrl(_relatedUrl, (isDanbooruFormat ? "query" : "tags") + "=" + tag)));

            var jsonArray = (JArray)(isDanbooruFormat
                ? content["tags"]
                : content[content.Properties().First().Name]);

            return jsonArray.Select(GetRelatedSearchResult).ToArray();
        }
    }
}
