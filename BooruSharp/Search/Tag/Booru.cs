using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Tag.SearchResult> GetTag(string name)
        {
            return await SearchTagAsync(name, null);
        }

        public async Task<Search.Tag.SearchResult> GetTag(int id)
        {
            if (!searchTagById)
                throw new Search.FeatureUnavailable();
            return await SearchTagAsync(null, id);
        }

        public async Task<Search.Tag.SearchResult[]> GetTagsAsync(string name)
        {
            List<string> urlTags = new List<string>() { SearchArg("name") + name };
            if (format != UrlFormat.danbooru)
                urlTags.Add("limit=0");
            var jsons = JsonConvert.DeserializeObject<Search.Tag.SearchResultJson[]>(await GetJsonAsync(CreateUrl(tagUrl, urlTags.ToArray())));
            int i = 0;
            Search.Tag.SearchResult[] results = new Search.Tag.SearchResult[jsons.Length];
            foreach (var json in jsons)
            {
                results[i] = new Search.Tag.SearchResult(
                        json.id,
                        json.name,
                        (Search.Tag.TagType)json.type,
                        json.count.Value);
                i++;
            }
            return results;
        }

        private async Task<Search.Tag.SearchResult> SearchTagAsync(string name, int? id)
        {
            List<string> urlTags = new List<string>();
            if (name == null)
                urlTags.Add(SearchArg("id") + id);
            else
                urlTags.Add(SearchArg("name") + name);
            if (format != UrlFormat.danbooru)
                urlTags.Add("limit=0");
            var jsons = JsonConvert.DeserializeObject<Search.Tag.SearchResultJson[]>(await GetJsonAsync(CreateUrl(tagUrl, urlTags.ToArray())));
            foreach (var json in jsons)
            {
                if ((name == null && id == json.id) || (name != null && name == json.name))
                    return new Search.Tag.SearchResult(
                        json.id,
                        json.name,
                        (Search.Tag.TagType)json.type,
                        json.count.Value);
            }
            throw new Search.InvalidTags();
        }
    }
}
