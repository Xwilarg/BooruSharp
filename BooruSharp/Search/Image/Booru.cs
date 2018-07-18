using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public int GetNbImage(params string[] tags)
        {
            XmlDocument xml = GetXml(CreateUrl(imageUrl, "limit=1", TagsToString(tags)));
            return (Convert.ToInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        public int? GetLimit()
        {
            return (maxLimit);
        }

        public Search.Image.SearchResult GetImage(int offset, params string[] tagsArg)
        {
            XmlDocument xml = GetXml(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg), ((needInterrogation) ? ("page=") : ("pid=")) + offset));
            string[] args = GetStringFromXml(xml.ChildNodes.Item(1).FirstChild, "file_url", "preview_url", "rating", "tags");
            return (new Search.Image.SearchResult(((
                args[0].StartsWith("//")) ? ("https:") : ("")) + args[0],
                ((args[1].StartsWith("//")) ? ("https:") : ("")) + args[1],
                GetRating(args[2][0]),
                args[3].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)));
        }

        public Search.Image.SearchResult GetRandomImage(params string[] tags)
        {
            int nbMax = GetNbImage(tags);
            if (nbMax == 0)
                throw new Search.InvalidTags();
            if (GetLimit() != null && GetLimit() < nbMax)
                nbMax = GetLimit().Value;
            int randomNb = random.Next(((needInterrogation) ? (1) : (0)), nbMax + 1);
            return (GetImage(randomNb, tags));
        }

        private Search.Image.Rating GetRating(char c)
        {
            switch (c)
            {
                case 's': return (Search.Image.Rating.Safe);
                case 'q': return (Search.Image.Rating.Questionable);
                case 'e': return (Search.Image.Rating.Explicit);
                default: throw new ArgumentException("Invalid rating " + c);
            }
        }
    }
}
