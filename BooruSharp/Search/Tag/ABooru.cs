using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Get information about a tag
        /// </summary>
        /// <param name="name">The name of the tag you want information about</param>
        public virtual async Task<Search.Tag.SearchResult> GetTagAsync(string name)
        {
            if (!HasTagByIdAPI())
                throw new Search.FeatureUnavailable();
            return await SearchTagAsync(name, null);
        }

        /// <summary>
        /// Get information about a tag
        /// </summary>
        /// <param name="name">The ID of the tag you want information about</param>
        public virtual async Task<Search.Tag.SearchResult> GetTagAsync(int id)
        {
            if (!_searchTagById)
                throw new Search.FeatureUnavailable();
            return await SearchTagAsync(null, id);
        }

        /// <summary>
        /// Get the similar tags of the one given
        /// </summary>
        /// <param name="name">The name of the tag you want others similar</param>
        public virtual async Task<Search.Tag.SearchResult[]> GetTagsAsync(string name)
        {
            if (!HasTagByIdAPI())
                throw new Search.FeatureUnavailable();
            List<string> urlTags = new List<string>() { SearchArg("name") + name };
            if (_format == UrlFormat.postIndexJson)
                urlTags.Add("limit=0");
            string url = CreateUrl(_tagUrl, urlTags.ToArray());
            Search.Tag.SearchResult[] results;
            if (_tagUseXml)
            {
                var xml = await GetXmlAsync(url);
                results = new Search.Tag.SearchResult[xml.LastChild.ChildNodes.Count];
                int i = 0;
                foreach (var node in xml.LastChild)
                {
                    results[i] = GetTagSearchResult(node);
                    i++;
                }
            }
            else
            {
                var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(url));
                results = new Search.Tag.SearchResult[jsons.Count];
                int i = 0;
                foreach (var json in jsons)
                {
                    results[i] = GetTagSearchResult(json);
                    i++;
                }
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
            if (_format == UrlFormat.postIndexJson)
                urlTags.Add("limit=0");
            string url = CreateUrl(_tagUrl, urlTags.ToArray());
            if (_tagUseXml)
            {
                var xml = await GetXmlAsync(url);
                foreach (var node in xml.LastChild)
                {
                    var result = GetTagSearchResult(node);
                    if ((name == null && id == result.id) || (name != null && name == result.name))
                        return result;
                }
            }
            else
            {
                var jsons = (JArray)JsonConvert.DeserializeObject(await GetJsonAsync(url));
                foreach (var json in jsons)
                {
                    var result = GetTagSearchResult(json);
                    if ((name == null && id == result.id) || (name != null && name == result.name))
                        return result;
                }
            }
            throw new Search.InvalidTags();
        }

        protected internal Search.Tag.TagType StringToTagType(string value)
        {
            value = value.ToLower();
            if (value == "tag")
                return Search.Tag.TagType.Trivia; // BooruSharp rename the tag "Tag" by "Trivia" for more clarity
            for (Search.Tag.TagType i = 0; i <= Enum.GetValues(typeof(Search.Tag.TagType)).Cast<Search.Tag.TagType>().Max(); i++)
            {
                if (i.ToString().ToLower() == value)
                    return i;
            }
            throw new ArgumentException("Invalid tag " + value);
        }
    }
}
