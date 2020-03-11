using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Tag.SearchResult> GetTagAsync(string name)
        {
            return await SearchTagAsync(name, null);
        }

        public async Task<Search.Tag.SearchResult> GetTagAsync(int id)
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
                        int.Parse(json.id),
                        json.name,
                        (Search.Tag.TagType)Enum.Parse(typeof(Search.Tag.TagType), json.type, true),
                        int.Parse(json.count));
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
                if ((name == null && id.ToString() == json.id) || (name != null && name == json.name))
                    return new Search.Tag.SearchResult(
                        int.Parse(json.id),
                        json.name,
                        (Search.Tag.TagType)Enum.Parse(typeof(Search.Tag.TagType), json.type, true),
                        int.Parse(json.count));
            }
            throw new Search.InvalidTags();
        }

        private Search.Tag.TagType GetTagType(string value)
        {
            if (int.TryParse(value, out int valInt))
                return (Search.Tag.TagType)valInt;
            return (Search.Tag.TagType)Enum.Parse(typeof(Search.Tag.TagType), value, true);
        }
    }
}
