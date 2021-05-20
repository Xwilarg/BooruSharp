using BooruSharp.Booru;
using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BooruSharp.Others
{
    /// <summary>
    /// Furaffinity.
    /// <para>https://www.furaffinity.net/</para>
    /// </summary>
    public class Furaffinity : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Furaffinity"/> class.
        /// </summary>
        public Furaffinity() : base("www.furaffinity.net", UrlFormat.None, BooruOptions.NoPostByID | BooruOptions.NoComment | BooruOptions.NoLastComments
                  | BooruOptions.NoMultipleRandom | BooruOptions.NoPostByMD5 | BooruOptions.NoRelated | BooruOptions.NoTagByID
                  | BooruOptions.NoWiki | BooruOptions.NoEmptyPostSearch | BooruOptions.NoPostCount | BooruOptions.NoFavorite)
        { }

        /// <inheritdoc/>
        public override bool IsSafe
            => false;

        /// <summary>
        /// Login
        /// </summary>
        public void LoginAsync(string cookieA, string cookieB)
        {
            _cookieA = cookieA;
            _cookieB = cookieB;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.furaffinity.net/browse/");
            if (_cookieA != null)
            {
                request.Content = new FormUrlEncodedContent(
                    new Dictionary<string, string>
                    {
                    { "Cookie", "a=" + _cookieA + ";b=" + _cookieB }
                    });
            }
            var req = await HttpClient.SendAsync(request);
            string html = await req.Content.ReadAsStringAsync();
            List<SearchResult> res = new List<SearchResult>();

            var matches = Regex.Matches(html,
                "<figure id=\"sid-[0-9]+\" class=\"([^\"]+)\"><b><u><a href=\"\\/view\\/([0-9]+)\\/\"><img alt=\"\" src=\"([^\"]+)  data-width=\"([0-9]+)\" data-height=\"([0-9]+)\"");
            foreach (var match in matches.Cast<Match>())
            {
                var id = match.Groups[2].Value;
                res.Add(
                    new SearchResult(
                        null,
                        new Uri("https:" + match.Groups[3].Value),
                        new Uri("https://www.furaffinity.net/view/" + id + "/"),
                        match.Groups[1].Value.Contains("r-adult") ? Rating.Explicit : Rating.Safe,
                        null,
                        int.Parse(id),
                        null,
                        -1,
                        -1,
                        int.Parse(match.Groups[5].Value),
                        int.Parse(match.Groups[4].Value),
                        null,
                        null,
                        null,
                        null
                    )
                );
            }
            return res.ToArray();
        }

        private string _cookieA, _cookieB;
    }
}
