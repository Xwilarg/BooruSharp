using System;
using System.Linq;
using System.Net;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        protected Booru(string baseUrl, UrlFormat format, int? maxLimit, bool useHttp = false)
        {
            imageUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetImageUrl(format);
            tagUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetTagUrl(format);
            needInterrogation = (imageUrl.EndsWith(".xml"));
            random = new Random();
            this.maxLimit = maxLimit;
        }

        private string GetImageUrl(UrlFormat format)
        {
            switch (format)
            {
                case UrlFormat.postIndexXml:
                    return ("post/index.xml");

                case UrlFormat.indexPhp:
                    return ("index.php?page=dapi&s=post&q=index");

                case UrlFormat.postXml:
                    return ("post.xml");

                default:
                    throw new ArgumentException("Invalid URL format " + format);
            }
        }

        private string GetTagUrl(UrlFormat format)
        {
            switch (format)
            {
                case UrlFormat.postIndexXml:
                    return ("tag/index.xml");

                case UrlFormat.indexPhp:
                    return ("index.php?page=dapi&s=tag&q=index");

                case UrlFormat.postXml:
                    return ("tag.xml");

                default:
                    throw new ArgumentException("Invalid URL format " + format);
            }
        }

        private XmlDocument GetXml(string url)
        {
            XmlDocument xml = new XmlDocument();
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("User-Agent: BooruSharp");
                xml.LoadXml(wc.DownloadString(url));
            }
            return (xml);
        }

        private string CreateImageUrl(params string[] args)
        {
            return (imageUrl + ((needInterrogation) ? ("?") : ("&")) + String.Join("&", args));
        }
        private string CreateTagUrl(params string[] args)
        {
            return (tagUrl + ((needInterrogation) ? ("?") : ("&")) + String.Join("&", args));
        }

        private string TagsToString(string[] tags)
        {
            return ("tags=" + String.Join("+", tags));
        }

        private string[] GetStringFromXml(XmlDocument xml, params string[] tags)
        {
            string[] vars = new string[tags.Length];
            if (xml.ChildNodes.Item(1).FirstChild.Attributes.Count > 0)
            {
                int i = 0;
                foreach (string s in tags)
                {
                    vars[i] = xml.ChildNodes.Item(1).FirstChild.Attributes.GetNamedItem(s).InnerXml;
                    i++;
                }
            }
            else
            {
                foreach (XmlNode node in xml.ChildNodes.Item(1).FirstChild.ChildNodes)
                {
                    if (tags.Contains(node.Name))
                        vars[Array.IndexOf(tags, node.Name)] = node.InnerXml;
                }
            }
            return (vars);
        }

        protected readonly string imageUrl, tagUrl;
        private readonly bool needInterrogation;
        private readonly int? maxLimit;
        private Random random;
    }
}
