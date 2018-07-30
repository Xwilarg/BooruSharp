using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Comment.SearchResult[]> GetComment(int postId)
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            return (await GetCommentInternal(CreateUrl(commentUrl, SearchArg("post_id") + postId)));
        }

        public async Task<Search.Comment.SearchResult[]> GetLastComment()
        {
            if (commentUrl == null)
                throw new Search.FeatureUnavailable();
            return (await GetCommentInternal(CreateUrl(commentUrl)));
        }

        private async Task<Search.Comment.SearchResult[]> GetCommentInternal(string url)
        {
            XmlDocument xml = await GetXml(url);
            int i = 0;
            Search.Comment.SearchResult[] results = new Search.Comment.SearchResult[xml.ChildNodes.Item(1).ChildNodes.Count];
            foreach (XmlNode node in xml.ChildNodes.Item(1).ChildNodes)
            {
                string[] args = GetStringFromXml(node, "id", "post_id", "creator_id", "created_at", "creator", "body");
                results[i] = new Search.Comment.SearchResult(
                    Convert.ToInt32(args[0]),
                    Convert.ToInt32(args[1]),
                    Convert.ToInt32(args[2]),
                    ParseDateTime(args[3]),
                    args[4],
                    args[5]);
                i++;
            }
            return (results);
        }
    }
}
