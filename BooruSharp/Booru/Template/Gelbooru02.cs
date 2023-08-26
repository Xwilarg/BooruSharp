﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Xml;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Gelbooru 0.2. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Gelbooru02 : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru02"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Gelbooru02(string domain, BooruOptions options = BooruOptions.None) 
            : base(domain, UrlFormat.IndexPhp, options | BooruOptions.NoRelated | BooruOptions.NoWiki | BooruOptions.NoPostByMD5
                  | BooruOptions.CommentApiXml | BooruOptions.TagApiXml | BooruOptions.NoMultipleRandom)
        {
            _url = domain;
        }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            string baseUrl = BaseUrl.Scheme + "://" + _url;
            string directory = elem["directory"].Value<string>();
            string image = elem["image"].Value<string>();
            string hash = elem["hash"].Value<string>();
            int id = elem["id"].Value<int>();
            var hasSample = elem["sample"].Value<bool>();

            return new Search.Post.SearchResult(
                new Uri(baseUrl + "//images/" + directory + "/" + image),
                new Uri(baseUrl + "//thumbnails/" + directory + "/thumbnails_" + image),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                hasSample ? new Uri(baseUrl + "//samples/" + directory + "/sample_" + hash + ".jpg") : null,
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' '),
                null,
                id,
                null,
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                null,
                null,
                null,
                null,
                elem["score"].Value<int?>(),
                null
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array 
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (XmlNode)json;
            XmlNode creatorId = elem.Attributes.GetNamedItem("creator_id");
            return new Search.Comment.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                int.Parse(elem.Attributes.GetNamedItem("post_id").Value),
                creatorId.InnerText.Length > 0 ? int.Parse(creatorId.Value) : (int?)null,
                DateTime.ParseExact(elem.Attributes.GetNamedItem("created_at").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                elem.Attributes.GetNamedItem("creator").Value,
                elem.Attributes.GetNamedItem("body").Value
                );
        }

        // GetWikiSearchResult not available

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (XmlNode)json;
            return new Search.Tag.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                elem.Attributes.GetNamedItem("name").Value,
                (Search.Tag.TagType)int.Parse(elem.Attributes.GetNamedItem("type").Value),
                int.Parse(elem.Attributes.GetNamedItem("count").Value)
                );
        }

        // GetRelatedSearchResult not available

        private protected override Search.Autocomplete.SearchResult[] GetAutocompleteResultAsync(object json)
        {
            var elem = (JArray)json;
            var autoCompleteResults = new List<Search.Autocomplete.SearchResult>();
            foreach (var item in elem.Children())
            {
                string label = item["label"].Value<string>();
                string name = item["value"].Value<string>();
                int count = Convert.ToInt32(Regex.Match(label, @"\(([^()]*)\)$").Groups[1].Value); //this should always work
                autoCompleteResults.Add(new Search.Autocomplete.SearchResult(null, name, label, null, count, null));
            }
            return autoCompleteResults.ToArray();
        }

        private readonly string _url;
    }
}
