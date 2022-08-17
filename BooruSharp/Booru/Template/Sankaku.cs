using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Sankaku. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Sankaku : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sankaku"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Sankaku(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Sankaku, options | BooruOptions.NoRelated | BooruOptions.NoPostByMD5 | BooruOptions.NoPostByID
                  | BooruOptions.NoPostCount | BooruOptions.NoFavorite | BooruOptions.NoTagByID)
        { }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message) // TODO: Doesn't work rn
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
            int id = elem["id"].Value<int>();

            var postUriBuilder = new UriBuilder(BaseUrl)
            {
                Host = BaseUrl.Host.Replace("capi-v2", "beta"),
                Path = $"/post/show/{id}",
            };

            var detailedTags = new List<Search.Tag.SearchResult>();
            var tags = new List<string>();
            foreach(var tag in (JArray)elem["tags"])
            {
                var name = tag["name"].Value<string>();
                tags.Add(name);
                
                detailedTags.Add(new Search.Tag.SearchResult(
                    tag["id"].Value<int>(),
                    name,
                    GetTagType(tag["type"].Value<int>()),
                    tag["post_count"].Value<int>()
                    ));
            }

            var url = elem["file_url"].Value<string>();
            var previewUrl = elem["preview_url"].Value<string>();
            var sampleUrl = elem["sample_url"].Value<string>();
            
            return new Search.Post.SearchResult(
                url == null ? null : new Uri(url),
                previewUrl == null ? null : new Uri(previewUrl),
                postUriBuilder.Uri,
                sampleUrl != null && sampleUrl.Contains("/preview/") ? new Uri(sampleUrl) : null,
                GetRating(elem["rating"].Value<string>()[0]),
                tags,
                detailedTags,
                id,
                elem["file_size"].Value<int>(),
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                elem["preview_height"].Value<int?>(),
                elem["preview_width"].Value<int?>(),
                _unixTime.AddSeconds(elem["created_at"]["s"].Value<int>()),
                elem["source"].Value<string>(),
                elem["total_score"].Value<int>(),
                elem["md5"].Value<string>()
                );
        }

        private Search.Tag.TagType GetTagType(int type)
        {
            switch(type)
            {
                case 0: return Search.Tag.TagType.Trivia;
                case 1: return Search.Tag.TagType.Artist;
                case 3: return Search.Tag.TagType.Copyright;
                case 4: return Search.Tag.TagType.Character;
                case 8: return Search.Tag.TagType.Metadata;
                default: return (Search.Tag.TagType)6;
            }
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            var authorToken = elem["author"];

            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                authorToken["id"].Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                authorToken["name"].Value<string>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                _unixTime.AddSeconds(elem["created_at"]["s"].Value<int>()),
                _unixTime.AddSeconds(elem["updated_at"]["s"].Value<int>()),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json) // TODO: Fix TagType values
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available
    }
}
