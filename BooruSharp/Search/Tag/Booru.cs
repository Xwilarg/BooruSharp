using System;
using System.Collections.Generic;
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

        public async Task<Search.Tag.SearchResult[]> GetTags(string name)
        {
            List<string> urlTags = new List<string>() { SearchArg("name") + name };
            if (format != UrlFormat.danbooru)
                urlTags.Add("limit=0");
            XmlDocument xml = await GetXml(CreateUrl(tagUrl, urlTags.ToArray()));
            int i = 0;
            Search.Tag.SearchResult[] results = new Search.Tag.SearchResult[xml.ChildNodes.Item(1).ChildNodes.Count];
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "name", "type", "count", "post-count");
                results[i] = new Search.Tag.SearchResult(
                        Convert.ToInt32(args[0]),
                        args[1],
                        (Search.Tag.TagType)Convert.ToInt32(args[2]),
                        Convert.ToInt32(args[3] ?? args[4]));
                i++;
            }
            return (results);
        }

        private async Task<Search.Tag.SearchResult> SearchTag(string name, int? id)
        {
            List<string> urlTags = new List<string>();
            if (name == null)
                urlTags.Add(SearchArg("id") + id);
            else
                urlTags.Add(SearchArg("name") + name);
            if (format != UrlFormat.danbooru)
                urlTags.Add("limit=0");
            XmlDocument xml = await GetXml(CreateUrl(tagUrl, urlTags.ToArray()));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "name", "type", "count", "post-count");
                if ((name == null && id.ToString() == args[0]) || (name != null && name == args[1]))
                    return (new Search.Tag.SearchResult(
                        Convert.ToInt32(args[0]),
                        args[1],
                        (Search.Tag.TagType)Convert.ToInt32(args[2]),
                        Convert.ToInt32(args[3] ?? args[4])));
            }
            throw new Search.InvalidTags();
        }
    }
}
