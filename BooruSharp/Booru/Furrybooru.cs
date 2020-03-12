using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru
{
    public sealed class Furrybooru : Booru
    {
        public Furrybooru(BooruAuth auth = null) : base("furry.booru.org", auth, UrlFormat.indexPhp, null, BooruOptions.useHttp, BooruOptions.noRelated, BooruOptions.noWiki)
        { }

        public override bool IsSafe()
            => false;

        public override Search.Post.SearchResult GetPostSearchResult(object json)
        {
            var elem = ((JArray)json).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return new Search.Post.SearchResult(
                    new Uri("https://furry.booru.org//images/" + elem["directory"].Value<string>() + "/" + elem["image"].Value<string>()),
                    new Uri("https://furry.booru.org//thumbnails/" + elem["directory"].Value<string>() + "/thumbnails_" + elem["image"].Value<string>()),
                    GetRating(elem["rating"].Value<string>()[0]),
                    elem["tags"].Value<string>().Split(' '),
                    elem["id"].Value<int>(),
                    null,
                    elem["height"].Value<int>(),
                    elem["width"].Value<int>(),
                    null,
                    null,
                    null,
                    null,
                    elem["score"].Value<int>()
                );
        }
    }
}
