using System;
using System.Net;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public int GetNbImage(params string[] tags)
        {
            XmlDocument xml = new XmlDocument();
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent: BooruSharp");
                xml.LoadXml(wc.DownloadString(CreateUrl("limit=1", TagsToString(tags))));
            }
            return (Convert.ToInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        public int? GetLimit()
        {
            return (maxLimit);
        }

        public ImageSearch.SearchResult GetImage(int offset, params string[] tagsArg)
        {
            XmlDocument xml = new XmlDocument();
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent: BooruSharp");
                xml.LoadXml(wc.DownloadString(CreateUrl("limit=1", TagsToString(tagsArg), ((needInterrogation) ? ("page=") : ("pid=")) + offset)));
            }
            string baseUrl = null, previewUrl = null, tags = null;
            ImageSearch.Rating rating = (ImageSearch.Rating)(-1);
            if (xml.ChildNodes.Item(1).FirstChild.Attributes.Count > 0)
            {
                baseUrl = xml.ChildNodes.Item(1).FirstChild.Attributes.GetNamedItem("file_url").InnerXml;
                previewUrl = xml.ChildNodes.Item(1).FirstChild.Attributes.GetNamedItem("preview_url").InnerXml;
                rating = GetRating(xml.ChildNodes.Item(1).FirstChild.Attributes.GetNamedItem("rating").InnerXml[0]);
                tags = xml.ChildNodes.Item(1).FirstChild.Attributes.GetNamedItem("tags").InnerXml;
            }
            else
            {
                foreach (XmlNode node in xml.ChildNodes.Item(1).FirstChild.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "file_url":
                            baseUrl = node.InnerXml;
                            break;

                        case "preview_url":
                            previewUrl = node.InnerXml;
                            break;

                        case "rating":
                            rating = GetRating(node.InnerXml[0]);
                            break;

                        case "tags":
                            tags = node.InnerXml;
                            break;
                    }
                }
            }
            return (new ImageSearch.SearchResult(((baseUrl.StartsWith("//")) ? ("https:") : ("")) + baseUrl,
                                            ((previewUrl.StartsWith("//")) ? ("https:") : ("")) + previewUrl,
                                            rating,
                                            tags.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)));
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
