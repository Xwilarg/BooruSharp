using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Gelbooru 0.2
    /// </summary>
    public abstract class Gelbooru02 : ABooru
    {
        [Obsolete(_deprecationMessage)]
        public Gelbooru02(string url, params BooruOptions[] options) : this(url, MergeOptions(options))
        { }

        public Gelbooru02(string url, BooruOptions options = BooruOptions.none) : base(url, UrlFormat.indexPhp, options |
             BooruOptions.noRelated | BooruOptions.noWiki | BooruOptions.noPostByMd5 | BooruOptions.commentApiXml | BooruOptions.tagApiXml | BooruOptions.noMultipleRandom)
        {
            this.url = url;
        }

        protected internal override JToken ParseFirstPostSearchResult(object json)
        {
            var elem = ((JArray)json).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            return elem;
        }

        protected internal override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            return new Search.Post.SearchResult(
                    new Uri("http" + (_useHttp ? "" : "s") + "://" + url + "//images/" + elem["directory"].Value<string>() + "/" + elem["image"].Value<string>()),
                    new Uri("http" + (_useHttp ? "" : "s") + "://" + url + "//thumbnails/" + elem["directory"].Value<string>() + "/thumbnails_" + elem["image"].Value<string>()),
                    new Uri(_baseUrl + "/index.php?page=post&s=view&id=" + elem["id"].Value<int>()),
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
                    elem["score"].Value<int?>(),
                    null
                );
        }

        protected internal override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            var arr = (JArray)json;
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
            var elem = (XmlNode)json;
            XmlNode creatorId = elem.Attributes.GetNamedItem("creator_id");
            return new Search.Comment.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                int.Parse(elem.Attributes.GetNamedItem("post_id").Value),
                creatorId.InnerText == "" ? (int?)null : int.Parse(creatorId.Value),
                DateTime.ParseExact(elem.Attributes.GetNamedItem("created_at").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                elem.Attributes.GetNamedItem("creator").Value,
                elem.Attributes.GetNamedItem("body").Value
                );
        }

        // GetWikiSearchResult not available

        protected internal override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (XmlNode)json;
            return new Search.Tag.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                elem.Attributes.GetNamedItem("name").Value,
                (Search.Tag.TagType)int.Parse(elem.Attributes.GetNamedItem("type").Value),
                int.Parse(elem.Attributes.GetNamedItem("count").Value)
                );
        }

        // GetRelatedSearchResult not available

        private string url;
    }
}
