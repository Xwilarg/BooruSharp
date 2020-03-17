using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    public abstract class Danbooru : Booru
    {
        public Danbooru(string url, BooruAuth auth = null) : base(url, auth, UrlFormat.danbooru, BooruOptions.wikiSearchUseTitle, BooruOptions.noLastComment)
        { }

        protected internal override Search.Post.SearchResult GetPostSearchResult(object json)
        {
            var elem = ((JArray)json).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return new Search.Post.SearchResult(
                    new Uri(elem["file_url"].Value<string>()),
                    new Uri(elem["preview_file_url"].Value<string>()),
                    GetRating(elem["rating"].Value<string>()[0]),
                    elem["tag_string"].Value<string>().Split(' '),
                    elem["id"].Value<int>(),
                    elem["file_size"].Value<int>(),
                    elem["image_height"].Value<int>(),
                    elem["image_width"].Value<int>(),
                    null,
                    null,
                    elem["created_at"].Value<DateTime>(),
                    elem["source"].Value<string>(),
                    elem["score"].Value<int>()
                );
        }

        protected internal override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"].Value<int>(),
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
            var elem = (JObject)json;
            return new Search.Related.SearchResult(
                elem["name"].Value<string>(),
                elem["count"].Value<int>()
                );
        }
    }
}
