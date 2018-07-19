using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public Search.Comment.SearchResult[] GetComment(int postId)
        {
            if (wikiUrl == null)
                throw new Search.FeatureUnavailable();
            XmlDocument xml = GetXml(CreateUrl(commentUrl, "post_id=" + postId));
            int i = 0;
            Search.Comment.SearchResult[] results = new Search.Comment.SearchResult[xml.ChildNodes.Item(1).ChildNodes.Count];
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "post_id", "creator_id", "created_at", "creator", "body");
                results[i] = new Search.Comment.SearchResult(
                    Convert.ToUInt32(args[0]),
                    Convert.ToUInt32(args[1]),
                    Convert.ToUInt32(args[2]),
                    ParseDateTime(args[3]),
                    args[4],
                    args[5]);
                i++;
            }
            return (results);
        }
    }
}
