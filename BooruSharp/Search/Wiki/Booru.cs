using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public Search.Wiki.SearchResult GetWiki(string query)
        {
            if (wikiUrl == null)
                throw new Search.FeatureUnavailable();
            XmlDocument xml = GetXml(CreateUrl(wikiUrl, ((wikiSearchUseTitle) ? ("title=") : ("query=")) + query));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "title", "created_at", "updated_at", "body");
                if (args[1] == query)
                    return (new Search.Wiki.SearchResult(
                        Convert.ToUInt32(args[0]),
                        args[1],
                        ParseDateTime(args[2]),
                        ParseDateTime(args[3]),
                        args[4]));
            }
            throw new Search.InvalidTags();
        }
    }
}
