using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public TagSearch.SearchResult GetTag(string name)
        {
            return (SearchTag(name, null));
        }

        public TagSearch.SearchResult GetTag(int id)
        {
            return (SearchTag(null, id));
        }

        public TagSearch.SearchResult SearchTag(string name, int? id)
        {
            XmlDocument xml = GetXml(CreateTagUrl(((name == null) ? ("id=" + id) : ("name=" + name))));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "name", "type", "count");
                if ((name == null && id.ToString() == args[0]) || (name != null && name == args[1]))
                    return (new TagSearch.SearchResult(
                        Convert.ToUInt32(args[0]),
                        args[1],
                        (TagSearch.TagType)Convert.ToInt32(args[2]),
                        Convert.ToUInt32(args[3])));
            }
            throw new ImageSearch.InvalidTags();
        }
    }
}
