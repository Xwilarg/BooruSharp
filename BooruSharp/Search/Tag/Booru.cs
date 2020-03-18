using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(tagUrl, urlTags.ToArray())));
            Search.Tag.SearchResult[] results = new Search.Tag.SearchResult[jsons.Count];
            int i = 0;
            foreach (var json in jsons)
            {
                results[i] = GetTagSearchResult(json);
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
            var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(tagUrl, urlTags.ToArray())));
            foreach (var json in jsons)
            {
                var result = GetTagSearchResult(json);
                if ((name == null && id == result.id) || (name != null && name == result.name))
                    return result;
            }
            throw new Search.InvalidTags();
        }

        private Search.Tag.TagType GetTagType(string value)
        {
            if (int.TryParse(value, out int valInt))
                return (Search.Tag.TagType)valInt;
            return (Search.Tag.TagType)Enum.Parse(typeof(Search.Tag.TagType), value, true);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Search.Tag.TagType StringToTagType(string value)
        {
            value = value.ToLower();
            if (value == "tag")
                return Search.Tag.TagType.Trivia; // BooruSharp rename the tag "Tag" by "Trivia" for more clarity
            for (Search.Tag.TagType i = 0; i < Enum.GetValues(typeof(Search.Tag.TagType)).Cast<Search.Tag.TagType>().Max(); i++)
            {
                if (i.ToString().ToLower() == value)
                    return i;
            }
            throw new ArgumentException("Invalid tag " + value);
        }
    }
}
