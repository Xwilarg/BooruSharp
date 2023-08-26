﻿using BooruSharp.Search;
using BooruSharp.Search.Tag;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Gelbooru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Gelbooru : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Gelbooru(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.IndexPhp, options | BooruOptions.NoWiki | BooruOptions.NoRelated | BooruOptions.LimitOf20000
                  | BooruOptions.CommentApiXml)
        { }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        /// <inheritdoc/>
        public async override Task<Search.Post.SearchResult> GetPostByMd5Async(string md5)
        {
            if (md5 == null)
                throw new ArgumentNullException(nameof(md5));

            // Create a URL that will redirect us to Gelbooru post URL containing post ID.
            string url = $"{BaseUrl}index.php?page=post&s=list&md5={md5}";

            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Head, url))
            using (HttpResponseMessage response = await HttpClient.SendAsync(message))
            {
                response.EnsureSuccessStatusCode();

                // If HEAD message doesn't actually redirect us then ID here will be null...
                Uri redirectUri = response.RequestMessage.RequestUri;
                string id = HttpUtility.ParseQueryString(redirectUri.Query).Get("id");

                // ...which will then throw NullReferenceException here.
                // Danbooru does the same when it doesn't find a post with matching MD5,
                // though I suppose throwing exception with more meaningful message
                // would be better.
                return await GetPostByIdAsync(int.Parse(id));
            }
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = ((JToken)json)["post"] as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            const string gelbooruTimeFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

            int id = elem["id"].Value<int>();
            var sampleUrl = elem["sample_url"].Value<string>();

            return new Search.Post.SearchResult(
                new Uri(elem["file_url"].Value<string>()),
                new Uri(elem["preview_url"].Value<string>()),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                string.IsNullOrWhiteSpace(sampleUrl) ? null : new Uri(sampleUrl),
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' ').Select(HttpUtility.HtmlDecode).ToArray(),
                null,
                id,
                null,
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                null,
                null,
                DateTime.ParseExact(elem["created_at"].Value<string>(), gelbooruTimeFormat, CultureInfo.InvariantCulture),
                elem["source"].Value<string>(),
                elem["score"].Value<int>(),
                elem["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return ((JToken)json)["post"] is JArray array
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

        private protected override SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new SearchResult(
                elem["id"].Value<int>(),
                HttpUtility.HtmlDecode(elem["name"].Value<string>()),
                (TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available

        private protected override async Task<IEnumerable> GetTagEnumerableSearchResultAsync(Uri url)
        {
            if (JsonConvert.DeserializeObject<JObject>(await GetJsonAsync(url)).TryGetValue("tag", out JToken token))
            {
                return (JArray)token;
            }
            throw new InvalidTags();
        }

        /// <summary>
        /// Gets a result of autocomplete options from a tag slice
        /// <para>(GelBooru variant)</para>
        /// </summary>
        /// <param name="query">The tag slice to autocomplete</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public override async Task<Search.Autocomplete.SearchResult[]> AutocompleteAsync(string query) //I commited to adding GelBooru one way or another, so here we are.
        {
            //No need to check for autocomplete API because this is an override.

            Uri url = new Uri(BaseUrl + $"index.php?page=autocomplete2&term={query}&type=tag_query&limit=10");

            var array = JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));

            return GetAutocompleteResultAsync(array);
        }

        private protected override Search.Autocomplete.SearchResult[] GetAutocompleteResultAsync(object json)
        {
            var elem = (JArray)json;
            var autoCompleteResults = new List<Search.Autocomplete.SearchResult>();
            foreach (var item in elem.Children())
            {
                string label = item["label"].Value<string>();
                string name = item["value"].Value<string>();
                int count = item["post_count"].Value<int>();
                var type = GetTagType(item["category"].Value<string>());
                autoCompleteResults.Add(new Search.Autocomplete.SearchResult(null, name, label, type, count, null));
            }
            return autoCompleteResults.ToArray();
        }
        private Search.Tag.TagType GetTagType(string typeName)
        {
            switch (typeName)
            {
                case "tag": return Search.Tag.TagType.Trivia;
                case "character": return Search.Tag.TagType.Character;
                case "copyright": return Search.Tag.TagType.Copyright;
                case "artist": return Search.Tag.TagType.Artist;
                case "metadata": return Search.Tag.TagType.Metadata;
                default: return (Search.Tag.TagType)6; //i won't question...
            }
        }
    }
}
