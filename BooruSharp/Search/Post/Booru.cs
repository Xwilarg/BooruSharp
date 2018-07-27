using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<int> GetNbImage(params string[] tags)
        {
            XmlDocument xml = await GetXml(CreateUrl(imageUrl, "limit=1", TagsToString(tags)));
            return (GetNbImageInternal(xml));
        }

        private int GetNbImageInternal(XmlDocument xml)
        {
            return (Convert.ToInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        public int? GetLimit()
        {
            return (maxLimit);
        }

        public async Task<Search.Post.SearchResult> GetImage(int offset, params string[] tagsArg)
        {
            XmlDocument xml = await GetXml(CreateUrl(imageUrl, "limit=1", ((needInterrogation) ? ("page=") : ("pid=")) + offset, TagsToString(tagsArg)));
            if (xml.ChildNodes.Item(1).ChildNodes.Count == 0)
                throw new Search.InvalidTags();
            string[] args = GetStringFromXml(xml.ChildNodes.Item(1).FirstChild, "file_url", "preview_url", "rating", "tags", "id",
                                            "file_size", "height", "width", "preview_height", "preview_width", "created_at", "source", "score");
            return (new Search.Post.SearchResult(
                new Uri(((args[0].StartsWith("//")) ? ("https:") : ("")) + args[0].Replace(" ", "%20")),
                new Uri(((args[1].StartsWith("//")) ? ("https:") : ("")) + args[1]),
                GetRating(args[2][0]),
                args[3].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32(args[4]),
                (args[5] == null) ? ((int?)null) : (Convert.ToInt32(args[5])),
                Convert.ToInt32(args[6]),
                Convert.ToInt32(args[7]),
                Convert.ToInt32(args[8]),
                Convert.ToInt32(args[9]),
                ParseDateTime(args[10]),
                (args[11] == "") ? (null) : (args[11]),
                Convert.ToInt32(args[12])));
        }

        public async Task<Search.Post.SearchResult> GetRandomImage(params string[] tags)
        {
            int nbMax = await GetNbImage(tags);
            if (nbMax == 0)
                throw new Search.InvalidTags();
            if (GetLimit() != null && GetLimit() < nbMax)
                nbMax = GetLimit().Value;
            int randomNb = random.Next(((needInterrogation) ? (1) : (0)), nbMax + 1);
            return (await GetImage(randomNb, tags));
        }

        private Search.Post.Rating GetRating(char c)
        {
            switch (c)
            {
                case 's': return (Search.Post.Rating.Safe);
                case 'q': return (Search.Post.Rating.Questionable);
                case 'e': return (Search.Post.Rating.Explicit);
                default: throw new ArgumentException("Invalid rating " + c);
            }
        }
    }
}
