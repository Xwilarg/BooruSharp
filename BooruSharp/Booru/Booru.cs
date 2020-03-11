using BooruSharp.Search;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public abstract bool IsSafe();

        public bool HaveRelatedAPI()
            => relatedUrl != null;
        public bool HaveWikiAPI()
            => wikiUrl != null;
        public bool HaveCommentAPI()
            => commentUrl != null;

        public bool HaveTagByIdAPI()
            => searchTagById;

        /// <exception cref="HttpRequestException">Service not available</exception>
        public async Task CheckAvailabilityAsync()
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
            this.baseUrlRaw = baseUrl;
            this.baseUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl;
            this.format = format;
            imageUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "post");
            tagUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag");
            if (options.Contains(BooruOptions.noWiki))
                wikiUrl = null;
            else if (format == UrlFormat.danbooru)
                wikiUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki_page");
            else
                wikiUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "wiki");
            if (options.Contains(BooruOptions.noRelated))
                relatedUrl = null;
            else
                relatedUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "tag", "related");
            if (options.Contains(BooruOptions.noComment))
                commentUrl = null;
            else
                commentUrl = "http" + (useHttp ? "" : "s") + "://" + baseUrl + "/" + GetUrl(format, "comment");
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

        private async Task<string> GetJsonAsync(string url)
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                HttpResponseMessage msg = await hc.GetAsync(url);
                if (msg.StatusCode == HttpStatusCode.Forbidden)
                    throw new AuthentificationRequired();
                return await msg.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> GetRandomIdAsync(string tags)
        {
            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
                HttpResponseMessage msg = await hc.GetAsync(baseUrl + "/" + "index.php?page=post&s=random&tags=" + tags);
                return HttpUtility.ParseQueryString(msg.RequestMessage.RequestUri.Query).Get("id");
            }
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
            return tags == null ? "" : "tags=" + string.Join("+", tags.Select(x => Uri.EscapeDataString(x))).ToLower();
        }

        private string SearchArg(string value)
        {
            if (format == UrlFormat.danbooru)
                return "search[" + value + "]=";
            else
                return value + "=";
        }

        private DateTime ParseDateTime(string dt)
        {
            DateTime res;
            dt = Regex.Replace(dt, "[-+][0-9]{2}:[0-9]{2}", ""); // TODO: manage timezones
            dt = dt.Replace(" UTC", "");
            dt = Regex.Replace(dt, " [-+][0-9]{4}", "");
            dt = Regex.Replace(dt, "\\.[0-9]{3}", ""); // Ignore ms
            if (dt.Length > 10 && dt[10] == 'T') dt = dt.Substring(0, 10) + " " + dt.Substring(11, dt.Length - 11);
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return res;
            if (DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return res;
            if (DateTime.TryParseExact(dt, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
                return res;
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt64(dt));
        }

        private readonly BooruAuth auth;
        private readonly string baseUrlRaw;
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
