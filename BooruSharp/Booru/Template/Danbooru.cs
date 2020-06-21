using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    public abstract class Danbooru : ABooru
    {
        public Danbooru(string url, params BooruOptions[] options) : base(url, UrlFormat.danbooru, CombineArrays(options, new[] { BooruOptions.noLastComments }))
        { }

        protected internal override string GetLoginString()
            => "login";

        public override bool CanLoginWithApiKey()
            => true;

        public override bool CanLoginWithPasswordHash()
            => true;

        protected internal override JToken ParseFirstPostSearchResult(object json)
        {
            var array = json as JArray;
            var elem = array == null ? ((JToken)json) : array.FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return elem;
        }

        protected internal override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            var url = elem["file_url"];
            var previewUrl = elem["preview_file_url"];
            var md5 = elem["md5"];
            return new Search.Post.SearchResult(
                    url == null ? null : new Uri(url.Value<string>()),
                    previewUrl == null ? null : new Uri(previewUrl.Value<string>()),
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
                    elem["score"].Value<int>(),
                    md5 == null ? null : md5.Value<string>()
                );
        }

        protected internal override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var arr = json as JArray;
            if (arr == null)
            {
                var token = (JToken)json;
                if (token == null)
                    return new Search.Post.SearchResult[0];
                return new Search.Post.SearchResult[1] { GetPostSearchResult(((JToken)json)["post"]) };
            }
            Search.Post.SearchResult[] res = new Search.Post.SearchResult[arr.Count];
            int i = 0;
            foreach (var elem in arr)
            {
                res[i] = GetPostSearchResult(elem);
                i++;
            }
            return res;
        }

        protected internal override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"].Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                elem["creator_name"]?.Value<string>(),
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
                (Search.Tag.TagType)elem["category"].Value<int>(),
                elem["post_count"].Value<int>()
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
