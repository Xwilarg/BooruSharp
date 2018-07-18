using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public Search.Related.SearchResult[] GetRelated(string tag)
        {
            if (wikiUrl == null)
                throw new Search.Wiki.NoWiki();
            XmlDocument xml = GetXml(CreateUrl(relatedUrl, "tags=" + tag));
            int i = 0;
            Search.Related.SearchResult[] results = new Search.Related.SearchResult[xml.ChildNodes.Item(1).FirstChild.ChildNodes.Count];
            foreach (XmlNode node in xml.ChildNodes.Item(1).FirstChild.ChildNodes)
            {
                string[] args = GetStringFromXml(node, "name", "count");
                results[i] = new Search.Related.SearchResult(args[0], Convert.ToUInt32(args[1]));
                i++;
            }
            return (results);
        }
    }
}
