using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Gelbooru 0.2. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Gelbooru02 : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru02"/> template class.
        /// </summary>
        /// <param name="url">The base URL to use. This should be a host name.</param>
        /// <param name="options">The collection of option values.</param>
        [Obsolete(_deprecationMessage)]
        public Gelbooru02(string url, params BooruOptions[] options) : this(url, MergeOptions(options))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru02"/> template class.
        /// </summary>
        /// <param name="url">The base URL to use. This should be a host name.</param>
        /// <param name="options">The options to use. Use | (bitwise OR) operator to combine multiple options.</param>
        public Gelbooru02(string url, BooruOptions options = BooruOptions.None) : base(url, UrlFormat.IndexPhp, options |
             BooruOptions.NoRelated | BooruOptions.NoWiki | BooruOptions.NoPostByMD5 | BooruOptions.CommentApiXml | BooruOptions.TagApiXml | BooruOptions.NoMultipleRandom)
        {
            _url = url;
        }

        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            return new Search.Post.SearchResult(
                new Uri("http" + (UsesHttp ? "" : "s") + "://" + _url + "//images/" + elem["directory"].Value<string>() + "/" + elem["image"].Value<string>()),
                new Uri("http" + (UsesHttp ? "" : "s") + "://" + _url + "//thumbnails/" + elem["directory"].Value<string>() + "/thumbnails_" + elem["image"].Value<string>()),
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

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array 
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (XmlNode)json;
            XmlNode creatorId = elem.Attributes.GetNamedItem("creator_id");
            return new Search.Comment.SearchResult(
                int.Parse(elem.Attributes.GetNamedItem("id").Value),
                int.Parse(elem.Attributes.GetNamedItem("post_id").Value),
                creatorId.InnerText.Length > 0 ? int.Parse(creatorId.Value) : (int?)null,
                DateTime.ParseExact(elem.Attributes.GetNamedItem("created_at").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                elem.Attributes.GetNamedItem("creator").Value,
                elem.Attributes.GetNamedItem("body").Value
                );
        }

        // GetWikiSearchResult not available

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
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

        private readonly string _url;
    }
}
