using BooruSharp.Search;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
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

        /// <exception cref="HttpRequestException">Service not available</exception>
        public async Task CheckAvailability()
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                await hc.SendAsync(new HttpRequestMessage(HttpMethod.Head, imageUrl));
            }
        }

        protected Booru(string baseUrl, BooruAuth auth, UrlFormat format, int? maxLimit, params BooruOptions[] options)
        {
            this.auth = auth;
            useHttp = options.Contains(BooruOptions.useHttp);
            this.baseUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl;
            this.format = format;
            imageUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "post");
            tagUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "tag");
            if (options.Contains(BooruOptions.noWiki))
                wikiUrl = null;
            else if (format == UrlFormat.danbooru)
                wikiUrl = "http" + ((useHttp) ? ("") : ("s")) + "://" + baseUrl + "/" + GetUrl(format, "wiki_page");
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
            this.maxLimit = maxLimit;
            wikiSearchUseTitle = options.Contains(BooruOptions.wikiSearchUseTitle);
        }

        protected static string GetUrl(UrlFormat format, string query, string squery = "index")
        {
            switch (format)
            {
                case UrlFormat.postIndexJson:
                    return query + "/" + squery + ".json";

                case UrlFormat.indexPhp:
                    return "index.php?page=dapi&s=" + query + "&q=index&json=1";

                case UrlFormat.danbooru:
                    return query + "s.json";

                case UrlFormat.sankaku:
                    return query + "s";

                default:
                    throw new ArgumentException("Invalid URL format " + format);
            }
        }

        private async Task<XmlDocument> GetXml(string url)
        {
            XmlDocument xml = new XmlDocument();
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                HttpResponseMessage msg = await hc.GetAsync(url);
                if (msg.StatusCode == HttpStatusCode.Forbidden)
                    throw new AuthentificationRequired();
                xml.LoadXml(await msg.Content.ReadAsStringAsync());
            }
            return (xml);
        }

        private string CreateUrl(string url, params string[] args)
        {
            string authArgs = "";
            if (auth != null)
                authArgs = "&login=" + auth.Login + "&password_hash=" + auth.PasswordHash;
            if (format == UrlFormat.indexPhp)
                return (url + "&" + string.Join("&", args) + authArgs);
            else
                return (url + "?" + string.Join("&", args) + authArgs);
        }

        private string TagsToString(string[] tags)
        {
            return ((tags == null) ? ("") : ("tags=" + string.Join("+", tags.Select(x => Uri.EscapeDataString(x))).ToLower()));
        }

        private int GetFirstPage()
        {
            if (format == UrlFormat.indexPhp)
                return (0);
            else
                return (1);
        }

        private string SearchArg(string value)
        {
            if (format == UrlFormat.danbooru)
                return ("search[" + value + "]=");
            else
                return (value + "=");
        }

        private string GetPage()
        {
            if (format == UrlFormat.indexPhp)
                return ("pid=");
            else
                return ("page=");
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
            dt = Regex.Replace(dt, "[-+][0-9]{2}:[0-9]{2}", "");
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

        private readonly BooruAuth auth;
        private readonly string baseUrl;
        private readonly string imageUrl, tagUrl, wikiUrl, relatedUrl, commentUrl;
        private readonly bool searchTagById;
        private readonly int? maxLimit;
        private readonly bool wikiSearchUseTitle;
        private readonly UrlFormat format;
        private readonly bool useHttp;
        private static readonly Random random = new Random();
    }
}
