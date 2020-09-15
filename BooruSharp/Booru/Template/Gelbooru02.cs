using BooruSharp.Utils;
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
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        /// <param name="options">
        /// The options to use. Use <c>|</c> (bitwise OR) operator to combine multiple options.
        /// </param>
        protected Gelbooru02(string domain, BooruOptions options = BooruOptions.None) 
            : base(domain, UrlFormat.IndexPhp, options | BooruOptions.NoRelated | BooruOptions.NoWiki | BooruOptions.NoPostByMD5
                  | BooruOptions.CommentApiXml | BooruOptions.TagApiXml | BooruOptions.NoMultipleRandom)
        {
            _url = domain;
        }

        private protected override JToken ParseFirstPostSearchResult(JToken token)
        {
            JArray array = token as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken token)
        {
            string baseUrl = BaseUrl.Scheme + "://" + _url;
            string directory = token["directory"].Value<string>();
            string image = token["image"].Value<string>();
            int id = token["id"].Value<int>();

            return new Search.Post.SearchResult(
                new Uri(baseUrl + "//images/" + directory + "/" + image),
                new Uri(baseUrl + "//thumbnails/" + directory + "/thumbnails_" + image),
                new Uri(BaseUrl + "index.php?page=post&s=view&id=" + id),
                RatingUtils.Parse(token["rating"].Value<string>()),
                token["tags"].Value<string>().Split(' '),
                id,
                null,
                token["height"].Value<int>(),
                token["width"].Value<int>(),
                null,
                null,
                null,
                null,
                token["score"].Value<int?>(),
                null
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(JToken token)
        {
            return token is JArray array 
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(XmlNode node)
        {
            XmlNode creatorId = node.Attributes.GetNamedItem("creator_id");
            return new Search.Comment.SearchResult(
                int.Parse(node.Attributes.GetNamedItem("id").Value),
                int.Parse(node.Attributes.GetNamedItem("post_id").Value),
                creatorId.InnerText.Length > 0 ? int.Parse(creatorId.Value) : (int?)null,
                DateTime.ParseExact(node.Attributes.GetNamedItem("created_at").Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                node.Attributes.GetNamedItem("creator").Value,
                node.Attributes.GetNamedItem("body").Value
                );
        }

        // GetWikiSearchResult not available

        private protected override Search.Tag.SearchResult GetTagSearchResult(XmlNode node)
        {
            return new Search.Tag.SearchResult(
                int.Parse(node.Attributes.GetNamedItem("id").Value),
                node.Attributes.GetNamedItem("name").Value,
                (Search.Tag.TagType)int.Parse(node.Attributes.GetNamedItem("type").Value),
                int.Parse(node.Attributes.GetNamedItem("count").Value)
                );
        }

        // GetRelatedSearchResult not available

        private readonly string _url;
    }
}
