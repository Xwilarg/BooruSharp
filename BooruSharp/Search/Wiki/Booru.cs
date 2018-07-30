using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Wiki.SearchResult> GetWiki(string query)
        {
            if (wikiUrl == null)
                throw new Search.FeatureUnavailable();
            XmlDocument xml = await GetXml(CreateUrl(wikiUrl, SearchArg((wikiSearchUseTitle) ? ("title") : ("query")) + query));
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "title", "created_at", "created-at", "updated_at", "updated-at", "body");
                if (args[1] == query)
                    return (new Search.Wiki.SearchResult(
                        Convert.ToInt32(args[0]),
                        args[1],
                        ParseDateTime(args[2] ?? args[3]),
                        ParseDateTime(args[4] ?? args[5]),
                        args[6]));
            }
            throw new Search.InvalidTags();
        }
    }
}
