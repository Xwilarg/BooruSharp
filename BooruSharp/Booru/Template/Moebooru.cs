using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    public abstract class Moebooru : ABooru
    {
        [Obsolete(_deprecationMessage)]
        public Moebooru(string url, params BooruOptions[] options) : this(url, MergeOptions(options))
        { }

        public Moebooru(string url, BooruOptions options = BooruOptions.none) : base(url, UrlFormat.postIndexJson, options | BooruOptions.noPostByMd5 | BooruOptions.noPostById | BooruOptions.noFavorite)
        { }

        protected internal override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        protected internal override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            return new Search.Post.SearchResult(
                new Uri(elem["file_url"].Value<string>()),
                new Uri(elem["preview_url"].Value<string>()),
                new Uri(_baseUrl + "/post/show/" + elem["id"].Value<int>()),
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' '),
                elem["id"].Value<int>(),
                elem["file_size"].Value<int>(),
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                elem["preview_height"].Value<int>(),
                elem["preview_width"].Value<int>(),
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(elem["created_at"].Value<int>()),
                elem["source"].Value<string>(),
                elem["score"].Value<int>(),
                elem["md5"].Value<string>()
                );
        }

        protected internal override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        protected internal override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"].Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                elem["creator"].Value<string>(),
                elem["body"].Value<string>()
                );
        }

        protected internal override Search.Wiki.SearchResult GetWikiSearchResult(object json)
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

        protected internal override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        protected internal override Search.Related.SearchResult GetRelatedSearchResult(object json)
        {
            var elem = (JArray)json;
            return new Search.Related.SearchResult(
                elem[0].Value<string>(),
                elem[1].Value<int>()
                );
        }
    }
}
