using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    public abstract class Danbooru : Booru
    {
        public Danbooru(string url, BooruAuth auth = null) : base(url, auth, UrlFormat.danbooru, BooruOptions.wikiSearchUseTitle)
        { }

        public override Search.Post.SearchResult GetPostSearchResult(object json)
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

        // GetCommentSearchResult not available
    }
}
