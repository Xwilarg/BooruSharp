using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Gets information about a tag.
        /// </summary>
        /// <param name="name">The name of the tag to get the information about.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="Search.InvalidTags"/>
        public virtual async Task<Search.Tag.SearchResult> GetTagAsync(string name)
        {
            if (!HasTagByIdAPI)
                throw new Search.FeatureUnavailable();

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return await SearchTagAsync(name, null);
        }

        /// <summary>
        /// Gets information about a tag.
        /// </summary>
        /// <param name="id">The ID of the tag to get the information about.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="Search.InvalidTags"/>
        public virtual async Task<Search.Tag.SearchResult> GetTagAsync(int id)
        {
            if (!HasTagByIdAPI)
                throw new Search.FeatureUnavailable();

            return await SearchTagAsync(null, id);
        }

        /// <summary>
        /// Gets the tags similar to the tag specified by its <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the tag to find similar tags to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Tag.SearchResult[]> GetTagsAsync(string name)
        {
            if (!HasTagByIdAPI)
                throw new Search.FeatureUnavailable();

            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var urlTags = new List<string> { SearchArg("name") + name };

            if (_format == UrlFormat.PostIndexJson)
                urlTags.Add("limit=0");

            var url = CreateUrl(_tagUrl, urlTags.ToArray());

            if (TagsUseXml)
            {
                var xml = await GetXmlAsync(url);
                // Can't use LINQ with XmlNodes so let's use list here.
                var results = new List<Search.Tag.SearchResult>(xml.LastChild.ChildNodes.Count);

                foreach (XmlNode node in xml.LastChild)
                {
                    results.Add(GetTagSearchResult(node));
                }

                return results.ToArray();
            }
            else
            {
                var jsonArray = await GetJsonAsync<JArray>(url);
                return jsonArray.Select(GetTagSearchResult).ToArray();
            }
        }

        private async Task<Search.Tag.SearchResult> SearchTagAsync(string name, int? id)
        {
            var urlTags = new List<string>();

            urlTags.Add(name is null
                ? SearchArg("id") + id
                : SearchArg("name") + name);

            if (_format == UrlFormat.PostIndexJson)
                urlTags.Add("limit=0");

            var url = CreateUrl(_tagUrl, urlTags.ToArray());
            IEnumerable enumerable;

            if (TagsUseXml)
            {
                var xml = await GetXmlAsync(url);
                enumerable = xml.LastChild;
            }
            else
            {
                enumerable = await GetJsonAsync<JArray>(url);
            }

            foreach (object item in enumerable)
            {
                var result = TagsUseXml
                    ? GetTagSearchResult((XmlNode)item)
                    : GetTagSearchResult((JToken)item);

                if ((name is null && id == result.ID) || (!(name is null) && name == result.Name))
                    return result;
            }

            throw new Search.InvalidTags();
        }
    }
}
