using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public bool HaveRelatedAPI()
        {
            return (relatedUrl != null);
        }
        public bool HaveWikiAPI()
        {
            return (wikiUrl != null);
        }
        public bool HaveCommentAPI()
        {
            return (commentUrl != null);
        }

        protected Booru(string baseUrl, UrlFormat format, int? maxLimit, params BooruOptions[] options)
        {
            bool useHttp = options.Contains(BooruOptions.useHttp);
            imageUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "post");
            tagUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "tag");
            if (options.Contains(BooruOptions.noWiki))
                wikiUrl = null;
            else
                wikiUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "wiki");
            if (options.Contains(BooruOptions.noRelated))
                relatedUrl = null;
            else
                relatedUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "tag", "related");
            if (options.Contains(BooruOptions.noComment))
                commentUrl = null;
            else
                commentUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "comment");
            needInterrogation = (imageUrl.EndsWith(".xml"));
            random = new Random();
            this.maxLimit = maxLimit;
            wikiSearchUseTitle = options.Contains(BooruOptions.wikiSearchUseTitle);
        }

        private string GetUrl(UrlFormat format, string query, string squery = "index")
        {
            switch (format)
            {
                case UrlFormat.postIndexXml:
                    return (query + "/" + squery +".xml");

                case UrlFormat.indexPhp:
                    return ("index.php?page=dapi&s=" + query + "&q=index");

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

        private string CreateUrl(string url, params string[] args)
        {
            return (url + ((needInterrogation) ? ("?") : ("&")) + String.Join("&", args));
        }

        private string TagsToString(string[] tags)
        {
            return ("tags=" + String.Join("+", tags));
        }

        private string[] GetStringFromXml(XmlNode xml, params string[] tags)
        {
            string[] vars = new string[tags.Length];
            if (xml.Attributes.Count > 0)
            {
                int i = 0;
                foreach (string s in tags)
                {
                    vars[i] = xml.Attributes.GetNamedItem(s).InnerXml;
                    i++;
                }
            }
            else
            {
                foreach (XmlNode node in xml.ChildNodes)
                {
                    if (tags.Contains(node.Name))
                        vars[Array.IndexOf(tags, node.Name)] = node.InnerXml;
                }
            }
            return (vars);
        }

        private DateTime ParseDateTime(string dt)
        {
            DateTime res;
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm:ss UTC", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return (res);
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return (res);
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return (res);
            return (DateTime.ParseExact(dt, "yyyy-MM-ddTHH:mm:ss+00:00", CultureInfo.InvariantCulture));
        }

        protected readonly string imageUrl, tagUrl, wikiUrl, relatedUrl, commentUrl;
        private readonly bool needInterrogation;
        private readonly int? maxLimit;
        private Random random;
        private bool wikiSearchUseTitle;
    }
}
