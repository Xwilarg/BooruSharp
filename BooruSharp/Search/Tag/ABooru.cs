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

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return await SearchTagAsync(name, null);
        }

        /// <summary>
        /// Get information about a tag
        /// </summary>
        /// <param name="name">The ID of the tag you want information about</param>
        public virtual async Task<Search.Tag.SearchResult> GetTagAsync(int id)
        {
            if (!HasTagByIdAPI())
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

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var urlTags = new List<string>() { SearchArg("name") + name };

            if (_format == UrlFormat.postIndexJson)
                urlTags.Add("limit=0");

            string url = CreateUrl(_tagUrl, urlTags.ToArray());

            if (TagsUseXml())
            {
                var xml = await GetXmlAsync(url);
                // Can't use LINQ with XmlNodes so let's use list here.
                var results = new List<Search.Tag.SearchResult>(xml.LastChild.ChildNodes.Count);

                foreach (var node in xml.LastChild)
                {
                    results.Add(GetTagSearchResult(node));
                }

                return results.ToArray();
            }
            else
            {
                var jsonArray = JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));
                return jsonArray.Select(GetTagSearchResult).ToArray();
            }
        }

        private async Task<Search.Tag.SearchResult> SearchTagAsync(string name, int? id)
        {
            var urlTags = new List<string>();

            urlTags.Add(name == null
                ? SearchArg("id") + id
                : SearchArg("name") + name);

            if (_format == UrlFormat.postIndexJson)
                urlTags.Add("limit=0");

            string url = CreateUrl(_tagUrl, urlTags.ToArray());

            if (TagsUseXml())
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
                var jsonArray = JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));

                foreach (var json in jsonArray)
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
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (comparer.Equals(value, "tag"))
                return Search.Tag.TagType.Trivia; // BooruSharp rename the tag "Tag" by "Trivia" for more clarity

            foreach (Search.Tag.TagType type in Enum.GetValues(typeof(Search.Tag.TagType)))
                if (comparer.Equals(value, type.ToString()))
                    return type;

            throw new ArgumentException($"Invalid tag '{value}'.", nameof(value));
        }
    }
}
