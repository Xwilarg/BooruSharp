using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<int> GetNbImage(params string[] tags)
        {
            string url;
            if (format == UrlFormat.danbooru)
                url = baseUrl + "/counts/posts.xml";
            else
                url = CreateUrl(imageUrl, "limit=1", TagsToString(tags));
            XmlDocument xml = await GetXml(url);
            return (GetNbImageInternal(xml));
        }

        private int GetNbImageInternal(XmlDocument xml)
        {
            if (format == UrlFormat.danbooru)
                return (Convert.ToInt32(xml.ChildNodes.Item(1).FirstChild.InnerXml));
            else
                return (Convert.ToInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        public int? GetLimit()
        {
            return (maxLimit);
        }

        public async Task<Search.Post.SearchResult> GetImage(int offset, params string[] tagsArg)
        {
            XmlDocument xml = await GetXml(CreateUrl(imageUrl, "limit=1", GetPage() + offset, TagsToString(tagsArg)));
            if (xml.ChildNodes.Item(1).ChildNodes.Count == 0)
                throw new Search.InvalidTags();
            string[] args = GetStringFromXml(xml.ChildNodes.Item(1).FirstChild, "file_url", "file-url", "preview_url", "preview-file-url",
                "rating", "tags", "tag-string", "id", "file_size", "file-size",
                "height", "image-height", "width", "image-width", "preview_height", "preview_width", "created_at", "created-at", "source", "score");
            return (new Search.Post.SearchResult(
                (args[0] == null && args[1] == null) ? (null) : (new Uri((((args[0] ?? args[1]).StartsWith("//")) ? ("http" + ((useHttp) ? ("") : ("s")) + ":") : ("")) + (args[0] ?? args[1]).Replace(" ", "%20"))),
                (args[2] == null && args[3] == null) ? (null) : (new Uri((((args[2] ?? args[3]).StartsWith("//")) ? ("http" + ((useHttp) ? ("") : ("s")) + ":") : ("")) + (args[2] ?? args[3]))),
                GetRating(args[4][0]),
                (args[5] ?? args[6]).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32(args[7]),
                (args[8] == null && args[9] == null) ? ((int?)null) : (Convert.ToInt32(args[8] ?? args[9])),
                Convert.ToInt32(args[10] ?? args[11]),
                Convert.ToInt32(args[12] ?? args[13]),
                (args[14] == null) ? ((int?)null) : (Convert.ToInt32(args[14])),
                (args[15] == null) ? ((int?)null) : (Convert.ToInt32(args[15])),
                ParseDateTime(args[16] ?? args[17]),
                (args[18] == "") ? (null) : (args[18]),
                (args[19] == "") ? (0) : (Convert.ToInt32(args[19]))));
        }

        public async Task<Search.Post.SearchResult> GetRandomImage(params string[] tags)
        {
            int nbMax = await GetNbImage(tags);
            if (nbMax == 0)
                throw new Search.InvalidTags();
            if (GetLimit() != null && GetLimit() < nbMax)
                nbMax = GetLimit().Value;
            int firstPage = GetFirstPage();
            int randomNb = random.Next(firstPage, nbMax + firstPage);
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
