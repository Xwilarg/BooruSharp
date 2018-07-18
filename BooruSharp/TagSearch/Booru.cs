using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public TagSearch.SearchResult GetTag(string name)
        {
            XmlDocument xml = GetXml(CreateTagUrl("name=" + name));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "name", "type", "count");
                if (args[1] == name)
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
