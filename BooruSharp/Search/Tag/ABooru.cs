﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /*
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

            if (name == null)
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

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var urlTags = new List<string> { SearchArg(_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "q" : "name") + name };

            if (_format == UrlFormat.PostIndexJson)
                urlTags.Add("limit=0");

            var url = CreateUrl(_tagUrl, urlTags.ToArray());

            var enumerable = await GetTagEnumerableSearchResultAsync(url);
            if (TagsUseXml)
            {
                // Can't use LINQ with XmlNodes so let's use list here.
                var results = new List<Search.Tag.SearchResult>(((XmlElement)enumerable).ChildNodes.Count);

                foreach (var node in enumerable)
                {
                    results.Add(GetTagSearchResult(node));
                }

                return results.ToArray();
            }
            else
            {
                return ((JArray)enumerable).Select(GetTagSearchResult).ToArray();
            }
        }

        private async Task<Search.Tag.SearchResult> SearchTagAsync(string name, int? id)
        {
            if (_format == UrlFormat.Philomena)
            {
                var tagToken = JsonConvert.DeserializeObject<JToken>(await GetJsonAsync($"{BaseUrl}api/v1/json/tags/{name}"))["tag"];
                return GetTagSearchResult(tagToken);
            }
            else
            {
                var urlTags = new List<string>();

                if (name == null)
                {
                    urlTags.Add(SearchArg("id") + id);
                }
                else
                {
                    urlTags.Add(SearchArg(_format == UrlFormat.BooruOnRails ? "q" : "name") + name);
                }

                if (_format == UrlFormat.PostIndexJson)
                    urlTags.Add("limit=0");

                var url = CreateUrl(_tagUrl, urlTags.ToArray());
                IEnumerable enumerable = await GetTagEnumerableSearchResultAsync(url);

                foreach (object item in enumerable)
                {
                    var result = GetTagSearchResult(item);

                    if ((name == null && id == result.ID) || (name != null && name == result.Name))
                        return result;
                }
            }

            throw new Search.InvalidTags();
        }

        private protected Search.Tag.TagType StringToTagType(string value)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (comparer.Equals(value, "tag"))
                return Search.Tag.TagType.Trivia; // BooruSharp rename the tag "Tag" by "Trivia" for more clarity

            foreach (Search.Tag.TagType type in Enum.GetValues(typeof(Search.Tag.TagType)))
                if (comparer.Equals(value, type.ToString()))
                    return type;

            throw new ArgumentException($"Invalid tag '{value}'.", nameof(value));
        }
        */
    }
}
