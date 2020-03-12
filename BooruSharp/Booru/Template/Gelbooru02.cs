using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Gelbooru 0.2
    /// </summary>
    public abstract class Gelbooru02 : Booru
    {
        public Gelbooru02(string url, BooruAuth auth = null) : base(url, auth, UrlFormat.indexPhp, null, BooruOptions.useHttp, BooruOptions.noRelated, BooruOptions.noWiki)
        {
            this.url = url;
        }

        public override Search.Post.SearchResult GetPostSearchResult(object json)
        {
            var elem = ((JArray)json).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return new Search.Post.SearchResult(
                    new Uri("http" + (useHttp ? "" : "s") + "://" + url + "//images/" + elem["directory"].Value<string>() + "/" + elem["image"].Value<string>()),
                    new Uri("http" + (useHttp ? "" : "s") + "://" + url + "//thumbnails/" + elem["directory"].Value<string>() + "/thumbnails_" + elem["image"].Value<string>()),
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

        private string url;
    }
}
