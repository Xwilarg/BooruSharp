using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public Search.Tag.SearchResult GetTag(string name)
        {
            return (SearchTag(name, null));
        }

        public Search.Tag.SearchResult GetTag(int id)
        {
            if (!searchTagById)
                throw new Search.FeatureUnavailable();
            return (SearchTag(null, id));
        }

        private Search.Tag.SearchResult SearchTag(string name, int? id)
        {
            XmlDocument xml = GetXml(CreateUrl(tagUrl, ((name == null) ? ("id=" + id) : ("name=" + name))));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "name", "type", "count");
                if ((name == null && id.ToString() == args[0]) || (name != null && name == args[1]))
                    return (new Search.Tag.SearchResult(
                        Convert.ToUInt32(args[0]),
                        args[1],
                        (Search.Tag.TagType)Convert.ToInt32(args[2]),
                        Convert.ToUInt32(args[3])));
            }
            throw new Search.InvalidTags();
        }
    }
}
