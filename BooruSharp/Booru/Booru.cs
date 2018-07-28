using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public abstract bool IsSafe();

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

        public bool HaveTagByIdAPI()
        {
            return (searchTagById);
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
            searchTagById = !options.Contains(BooruOptions.noTagById);
            needInterrogation = (imageUrl.EndsWith(".xml"));
            random = new Random();
            this.maxLimit = maxLimit;
            wikiSearchUseTitle = options.Contains(BooruOptions.wikiSearchUseTitle);
        }

        protected static string GetUrl(UrlFormat format, string query, string squery = "index")
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

        private async Task<XmlDocument> GetXml(string url)
        {
            XmlDocument xml = new XmlDocument();
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "BooruSharp");
                xml.LoadXml(await (await hc.GetAsync(url)).Content.ReadAsStringAsync());
            }
            return (xml);
        }

        private string CreateUrl(string url, params string[] args)
        {
            return (url + ((needInterrogation) ? ("?") : ("&")) + String.Join("&", args));
        }

        private string TagsToString(string[] tags)
        {
            return (("tags=" + String.Join("+", tags)).ToLower());
        }

        private string[] GetStringFromXml(XmlNode xml, params string[] tags)
        {
            string[] vars = new string[tags.Length];
            if (xml.Attributes.Count > 0)
            {
                int i = 0;
                foreach (string s in tags)
                {
                    XmlNode node = xml.Attributes.GetNamedItem(s);
                    if (node != null)
                        vars[i] = node.InnerXml;
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
            dt = Regex.Replace(dt, "[+][0-9]{2}:[0-9]{2}", "");
            dt = dt.Replace(" UTC", "");
            dt = Regex.Replace(dt, " [-+][0-9]{4}", "");
            if (dt.Length > 10 && dt[10] == 'T') dt = dt.Substring(0, 10) + " " + dt.Substring(11, dt.Length - 11);
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return (res);
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return (res);
            if (DateTime.TryParseExact(dt, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return (res);
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt64(dt)));
        }

        protected readonly string imageUrl, tagUrl, wikiUrl, relatedUrl, commentUrl;
        private bool searchTagById;
        private readonly bool needInterrogation;
        private readonly int? maxLimit;
        private Random random;
        private bool wikiSearchUseTitle;
    }
}
