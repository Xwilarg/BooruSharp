using System;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public int GetNbImage(params string[] tags)
        {
            XmlDocument xml = GetXml(CreateImageUrl("limit=1", TagsToString(tags)));
            return (Convert.ToInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        public int? GetLimit()
        {
            return (maxLimit);
        }

        public ImageSearch.SearchResult GetImage(int offset, params string[] tagsArg)
        {
            XmlDocument xml = GetXml(CreateImageUrl("limit=1", TagsToString(tagsArg), ((needInterrogation) ? ("page=") : ("pid=")) + offset));
            string[] args = GetStringFromXml(xml.ChildNodes.Item(1).FirstChild, "file_url", "preview_url", "rating", "tags");
            return (new ImageSearch.SearchResult(((
                args[0].StartsWith("//")) ? ("https:") : ("")) + args[0],
                ((args[1].StartsWith("//")) ? ("https:") : ("")) + args[1],
                GetRating(args[2][0]),
                args[3].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)));
        }

        public ImageSearch.SearchResult GetRandomImage(params string[] tags)
        {
            int nbMax = GetNbImage(tags);
            if (nbMax == 0)
                throw new ImageSearch.InvalidTags();
            if (GetLimit() != null && GetLimit() < nbMax)
                nbMax = GetLimit().Value;
            int randomNb = random.Next(((needInterrogation) ? (1) : (0)), nbMax + 1);
            return (GetImage(randomNb, tags));
        }

        private ImageSearch.Rating GetRating(char c)
        {
            switch (c)
            {
                case 's': return (ImageSearch.Rating.Safe);
                case 'q': return (ImageSearch.Rating.Questionable);
                case 'e': return (ImageSearch.Rating.Explicit);
                default: throw new ArgumentException("Invalid rating " + c);
            }
        }
    }
}
