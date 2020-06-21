using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    public abstract class E621 : ABooru
    {
        public E621(string url, params BooruOptions[] options) : base(url, UrlFormat.danbooru, CombineArrays(options, new[] { BooruOptions.noWiki, BooruOptions.noRelated, BooruOptions.noComment, BooruOptions.noTagById }))
        { }

        protected internal override string GetLoginString()
            => "login";

        public override bool CanLoginWithApiKey()
            => true;

        public override bool CanLoginWithPasswordHash()
            => false;

        protected internal override JToken ParseFirstPostSearchResult(object json)
        {
            var posts = ((JObject)json)["posts"];
            var elem = posts == null ? ((JObject)json)["post"] : ((JArray)posts).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return elem;
        }

        protected internal override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            List<string> tags = elem["tags"]["general"].ToObject<string[]>().ToList();
            tags.AddRange(elem["tags"]["species"].ToObject<string[]>().ToList());
            tags.AddRange(elem["tags"]["character"].ToObject<string[]>().ToList());
            tags.AddRange(elem["tags"]["copyright"].ToObject<string[]>().ToList());
            tags.AddRange(elem["tags"]["artist"].ToObject<string[]>().ToList());
            tags.AddRange(elem["tags"]["meta"].ToObject<string[]>().ToList()); // TODO: Check others tags
            string url = elem["file"]["url"].Value<string>();
            string previewUrl = elem["preview"]["url"].Value<string>();
            return new Search.Post.SearchResult(
                    url == null ? null : new Uri(url),
                    previewUrl == null ? null : new Uri(previewUrl),
                    GetRating(elem["rating"].Value<string>()[0]),
                    tags.ToArray(),
                    elem["id"].Value<int>(),
                    elem["file"]["size"].Value<int>(),
                    elem["file"]["height"].Value<int>(),
                    elem["file"]["width"].Value<int>(),
                    elem["preview"]["height"].Value<int>(),
                    elem["preview"]["width"].Value<int>(),
                    elem["created_at"].Value<DateTime>(),
                    elem["sources"].Count() > 0 ? elem["sources"][0].Value<string>() : null,
                    elem["score"]["total"].Value<int>(),
                    elem["file"]["md5"].Value<string>()
                );
        }

        protected internal override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var arr = (JArray)((JObject)json)["posts"];
            if (arr == null)
            {
                var token = ((JToken)json)["post"];
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

        // GetCommentSearchResult not available

        // GetWikiSearchResult not available

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

        // GetRelatedSearchResult not available // TODO: Available with credentials?
    }
}
