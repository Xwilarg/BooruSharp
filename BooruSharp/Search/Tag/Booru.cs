using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Tag.SearchResult> GetTag(string name)
        {
            return (await SearchTag(name, null));
        }

        public async Task<Search.Tag.SearchResult> GetTag(int id)
        {
            if (!searchTagById)
                throw new Search.FeatureUnavailable();
            return (await SearchTag(null, id));
        }

        private async Task<Search.Tag.SearchResult> SearchTag(string name, int? id)
        {
            XmlDocument xml = await GetXml(CreateUrl(tagUrl, ((name == null) ? ("id=" + id) : ("name=" + name))));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "name", "type", "count");
                if ((name == null && id.ToString() == args[0]) || (name != null && name == args[1]))
                    return (new Search.Tag.SearchResult(
                        Convert.ToInt32(args[0]),
                        args[1],
                        (Search.Tag.TagType)Convert.ToInt32(args[2]),
                        Convert.ToInt32(args[3])));
            }
            throw new Search.InvalidTags();
        }
    }
}
