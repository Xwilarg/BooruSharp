using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public TagSearch.SearchResult GetTag(string name)
        {
            XmlDocument xml = GetXml(CreateTagUrl("limit=1", "name=" + name));
            string[] args = GetStringFromXml(xml, "id", "name", "type");
            return (new TagSearch.SearchResult(
                Convert.ToUInt32(args[0]),
                args[1],
                (TagSearch.TagType)Convert.ToInt32(args[2])));
        }
    }
}
