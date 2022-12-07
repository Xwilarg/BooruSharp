using BooruSharp.Booru.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Danbooru. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Danbooru : ABooru<EmptyParsing, EmptyParsing, EmptyParsing, EmptyParsing, EmptyParsing>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Danbooru"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Danbooru(string domain, BooruOptions options = BooruOptions.None)
            : base(domain, UrlFormat.Danbooru, options | BooruOptions.NoLastComments | BooruOptions.NoPostCount
                  | BooruOptions.NoFavorite)
        { }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message)
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        /*
        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JToken token = json is JArray array
                ? array.FirstOrDefault()
                : json as JToken;

            return token ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            var url = elem["file_url"];
            var previewUrl = elem["preview_file_url"];
            var sampleUrl = elem["large_file_url"];
            var id = elem["id"]?.Value<int>();
            var md5 = elem["md5"];

            var detailedtags = new List<Search.Tag.SearchResult>();
            GetTags("tag_string_general", Search.Tag.TagType.Trivia);
            GetTags("tag_string_character", Search.Tag.TagType.Character);
            GetTags("tag_string_copyright", Search.Tag.TagType.Copyright);
            GetTags("tag_string_meta", Search.Tag.TagType.Metadata);
            GetTags("tag_string_artist", Search.Tag.TagType.Artist);
            
            void GetTags(string objectName, Search.Tag.TagType tagType)
            {
                var obj = elem[objectName];
                if(obj == null)
                    return;
                
                foreach(var x in obj.Value<string>().Split())
                {
                    detailedtags.Add(new Search.Tag.SearchResult(
                        -1,
                        x,
                        tagType,
                        -1));
                }
            }
            
            return new Search.Post.SearchResult(
                    url != null ? new Uri(url.Value<string>()) : null,
                    previewUrl != null ? new Uri(previewUrl.Value<string>()) : null,
                    id.HasValue ? new Uri(BaseUrl + "posts/" + id.Value) : null,
                    sampleUrl != null ? new Uri(sampleUrl.Value<string>()) : null,
                    GetRating(elem["rating"].Value<string>()[0]),
                    elem["tag_string"].Value<string>().Split(' '),
                    detailedtags,
                    id ?? 0,
                    elem["file_size"].Value<int>(),
                    elem["image_height"].Value<int>(),
                    elem["image_width"].Value<int>(),
                    null,
                    null,
                    elem["created_at"].Value<DateTime>(),
                    elem["source"].Value<string>(),
                    elem["score"].Value<int>(),
                    md5?.Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            if (json is JArray array)
                return array.Select(GetPostSearchResult).ToArray();
            else if (json is JToken token && token["post"] is JToken post)
                return new[] { GetPostSearchResult(post) };
            else
                return Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"]?.Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                elem["creator_name"]?.Value<string>(),
                elem["body"]?.Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                elem["created_at"].Value<DateTime>(),
                elem["updated_at"].Value<DateTime>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["category"].Value<int>(),
                elem["post_count"].Value<int>()
                );
        }

        private protected override Search.Related.SearchResult GetRelatedSearchResult(object json)
        {
            var elem = (JArray)json;
            return new Search.Related.SearchResult(
                elem[0].Value<string>(),
                elem[1].Value<int>()
                );
        }*/
    }
}
