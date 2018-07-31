using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Related.SearchResult[]> GetRelated(string tag)
        {
            if (relatedUrl == null)
                throw new Search.FeatureUnavailable();
            if (format == UrlFormat.danbooru)
                return (await GetRelatedDanbooru(tag));
            else
                return (await GetRelatedOther(tag));
        }

        private async Task<Search.Related.SearchResult[]> GetRelatedDanbooru(string tag)
        {
            XmlDocument xml = await GetXml(CreateUrl(tagUrl, SearchArg("name") + tag));
            string arg = GetStringFromXml(xml.ChildNodes.Item(1).FirstChild, "related-tags")[0];
            string[] allTags = arg.Split(' ');
            Search.Related.SearchResult[] result = new Search.Related.SearchResult[allTags.Length / 2];
            for (int i = 0; i < allTags.Length; i += 2)
                result[i / 2] = new Search.Related.SearchResult(allTags[i], null);
            return (result);
        }

        private async Task<Search.Related.SearchResult[]> GetRelatedOther(string tag)
        {
            XmlDocument xml = await GetXml(CreateUrl(relatedUrl, SearchArg("tags") + tag));
            int i = 0;
            Search.Related.SearchResult[] results = new Search.Related.SearchResult[xml.ChildNodes.Item(1).FirstChild.ChildNodes.Count];
            foreach (XmlNode node in xml.ChildNodes.Item(1).FirstChild.ChildNodes)
            {
                string[] args = GetStringFromXml(node, "name", "count");
                results[i] = new Search.Related.SearchResult(args[0], Convert.ToInt32(args[1]));
                i++;
            }
            return (results);
        }
    }
}
