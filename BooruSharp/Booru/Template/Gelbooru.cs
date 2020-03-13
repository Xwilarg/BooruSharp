using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Gelbooru 0.2
    /// </summary>
    public abstract class Gelbooru : Booru
    {
        public Gelbooru(string url, BooruAuth auth = null) : base(url, auth, UrlFormat.indexPhp, BooruOptions.noWiki, BooruOptions.noRelated, BooruOptions.limitOf20000)
        { }

        public override Search.Post.SearchResult GetPostSearchResult(object json)
        {
            var elem = ((JArray)json).FirstOrDefault();
            if (elem == null)
                throw new Search.InvalidTags();
            Match match = Regex.Match(elem["created_at"].Value<string>(), "[\\w]{3} ([\\w]{3}) ([0-9]{2}) ([0-9:]{8}) ([-+0-9]{5}) ([0-9]{4})");
            string timezone = match.Groups[4].Value;
            string dt = match.Groups[5] + "-" + GetMonth(match.Groups[1].Value) + "-" + match.Groups[2].Value + "T" + match.Groups[3].Value + ".0000000" + timezone.Substring(0, 3) + ":" + timezone.Substring(3, 2);
            return new Search.Post.SearchResult(
                    new Uri(elem["file_url"].Value<string>()),
                    new Uri("https://gelbooru.com/thumbnails/" + elem["directory"].Value<string>() + "/thumbnail_" + elem["image"].Value<string>()),
                    GetRating(elem["rating"].Value<string>()[0]),
                    elem["tags"].Value<string>().Split(' '),
                    elem["id"].Value<int>(),
                    null,
                    elem["height"].Value<int>(),
                    elem["width"].Value<int>(),
                    null,
                    null,
                    DateTime.Parse(dt),
                    elem["source"].Value<string>(),
                    elem["score"].Value<int>()
                );
        }

        private string GetMonth(string value)
        {
            switch (value)
            {
                case "Jan": return "01";
                case "Feb": return "02";
                case "Mar": return "03";
                case "Apr": return "04";
                case "May": return "05";
                case "Jun": return "06";
                case "Jul": return "07";
                case "Aug": return "08";
                case "Sep": return "09";
                case "Oct": return "10";
                case "Nov": return "11";
                case "Dec": return "12";
                default: throw new ArgumentException("Invalid month " + value);
            }
        }
    }
}
