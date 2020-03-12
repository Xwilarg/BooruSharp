using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    public abstract class Moebooru : Booru
    {
        public Moebooru(string url, BooruAuth auth = null) : base(url, auth, UrlFormat.postIndexJson)
        { }

        public override Search.Post.SearchResult GetPostSearchResult(object json)
        {
            var elem = ((JArray)json).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return new Search.Post.SearchResult(
                    new Uri(elem["file_url"].Value<string>()),
                    new Uri(elem["preview_url"].Value<string>()),
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
                    elem["score"].Value<int>()
                );
        }
    }
}
