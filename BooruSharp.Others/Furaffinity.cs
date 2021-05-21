using BooruSharp.Booru;
using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public void Login(string cookieA, string cookieB)
        {
            _cookieA = cookieA;
            _cookieB = cookieB;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        public override async Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            if (_cookieA == null)
                throw new AuthentificationRequired();

            if (tagsArg.Length > 0)
                throw new ArgumentException("GetRandomPostAsync can't be used with tags", nameof(tagsArg));
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.furaffinity.net/browse");
            request.Headers.Add("Cookie", "a=" + _cookieA + ";b=" + _cookieB);
            var req = await HttpClient.SendAsync(request);
            string html = await req.Content.ReadAsStringAsync();

            var match = Regex.Match(html, _postRegex);
            var maxId = match.Groups[2].Value;

            var id = Random.Next(1, int.Parse(maxId) + 1);

            var postUrl = new Uri("https://www.furaffinity.net/view/" + id);
            request = new HttpRequestMessage(HttpMethod.Post, postUrl);
            request.Headers.Add("Cookie", "a=" + _cookieA + ";b=" + _cookieB);

            req = await HttpClient.SendAsync(request);
            html = await req.Content.ReadAsStringAsync();

            if (html.Contains("System Error"))
                throw new InvalidPostId("No post was found");

            var rating = Regex.Match(html, "rating-box inline ([^\"]+)").Groups[1].Value;
            var size = Regex.Match(html, "Size<\\/strong> <span>([0-9]+) x ([0-9]+)px");
            var file = Regex.Match(html, "<div class=\"download[^\"]*\"><a href=\"([^\"]+)\">");
            var result = new SearchResult(
                file.Success ? new Uri("https:" + file.Groups[1].Value) : null,
                null,
                postUrl,
                rating == "adult" ? Rating.Explicit : rating == "mature" ? Rating.Questionable : rating == "general" ? Rating.Safe : throw new NullReferenceException("Invalid rating " + rating),
                Regex.Matches(html, "<span class=\"tags\"><a[^>]+>([^<]+)").Cast<Match>().Select(x => x.Groups[1].Value).ToArray(),
                id,
                null,
                int.Parse(size.Groups[2].Value),
                int.Parse(size.Groups[1].Value),
                null,
                null,
                null,
                null,
                null,
                null
                );
            return result;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult[]> GetLastPostsAsync(int limit, params string[] tagsArg)
        {
            if (tagsArg.Length > 0)
                throw new ArgumentException("Can't use limit parameter with tags", nameof(tagsArg));
            if (limit != 24 && limit != 48 && limit != 72)
                throw new ArgumentException("limit parameter must be 24, 48 or 72", nameof(tagsArg));
            return await GetLastPostsInternal(limit, tagsArg);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidTags"/>
        public override async Task<SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            return await GetLastPostsInternal(48, tagsArg);
        }

        private async Task<SearchResult[]> GetLastPostsInternal(int limit, string[] tagsArg)
        {
            string endpoint = tagsArg.Length > 0 ? "search" : "browse";
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.furaffinity.net/" + endpoint);
            MultipartFormDataContent formData = new MultipartFormDataContent();
            if (tagsArg.Length > 0)
            {
                formData.Add(new StringContent("@keywords+" + string.Join("+", tagsArg)), "q");
            }
            else
            {
                formData.Add(new StringContent("perpage"), limit.ToString());
            }
            request.Content = formData;
            if (_cookieA != null)
            {
                request.Headers.Add("Cookie", "a=" + _cookieA + "; b=" + _cookieB);
            }
            var req = await HttpClient.SendAsync(request);
            string html = await req.Content.ReadAsStringAsync();
            List<SearchResult> res = new List<SearchResult>();

            var matches = Regex.Matches(html, _postRegex);
            foreach (var match in matches.Cast<Match>())
            {
                Console.WriteLine(match.Groups[1].Value);
                var id = match.Groups[2].Value;
                res.Add(
                    new SearchResult(
                        null,
                        new Uri("https:" + match.Groups[3].Value),
                        new Uri("https://www.furaffinity.net/view/" + id + "/"),
                        match.Groups[1].Value.Contains("r-adult") ? Rating.Explicit : Rating.Safe,
                        Array.Empty<string>(),
                        int.Parse(id),
                        null,
                        -1,
                        -1,
                        (int)Math.Ceiling(float.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture)),
                        (int)Math.Ceiling(float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture)),
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
        private readonly string _postRegex = "<figure id=\"sid-[0-9]+\" class=\"([^\"]+)\"><b><u><a href=\"\\/view\\/([0-9]+)\\/\"><img alt=\"\" src=\"([^\"]+)\"  data-width=\"([0-9.]+)\" data-height=\"([0-9.]+)\"";
    }
}
