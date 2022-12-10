using BooruSharp.Search.Post;
using BooruSharp.Search.Tag;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on Sankaku. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class Sankaku : ABooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sankaku"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        protected Sankaku(string domain)
            : base(domain)
        { }

        protected override Uri CreateQueryString(string query, string squery = "index")
        {
            if (query == "wiki")
            {
                return new($"{BaseUrl}{query}");
            }
            return new($"{BaseUrl}{query}s");
        }

        protected override Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            return Task.FromResult(CreateUrl(_imageUrl, "limit=1", "tags=" + string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant() + "+order:random"));
        }

        /// <inheritdoc/>
        protected override void PreRequest(HttpRequestMessage message) // TODO: Doesn't work rn
        {
            if (Auth != null)
            {
                message.Headers.Add("Cookie", "user_id=" + Auth.UserId + ";pass_hash=" + Auth.PasswordHash);
            }
        }

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var parsingData = (await GetDataAsync<SearchResult[]>(uri))[0];

            return new PostSearchResult(
                fileUrl: parsingData.FileUrl != null ? new(parsingData.FileUrl) : null,
                previewUrl: parsingData.PreviewUrl != null ? new(parsingData.PreviewUrl) : null,
                postUrl: new UriBuilder(BaseUrl)
                {
                    Host = BaseUrl.Host.Replace("capi-v2", "beta"),
                    Path = $"/post/show/{parsingData.Id}",
                }.Uri,
                sampleUri: parsingData.SampleUrl != null && parsingData.SampleUrl.Contains("/preview/") ? new Uri(parsingData.SampleUrl) : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.Tags.Select(x => x.NameEn),
                detailedTags: parsingData.Tags.Select(x => new TagSearchResult(x.Id, x.NameEn, GetTagType(x.Type), x.PostCount)),
                id: parsingData.Id,
                size: parsingData.FileSize,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: parsingData.PreviewHeight,
                previewWidth: parsingData.PreviewWidth,
                creation: _unixTime.AddSeconds(parsingData.CreatedAt.S),
                sources: string.IsNullOrEmpty(parsingData.Source) ? Array.Empty<string>() : new[] { parsingData.Source },
                score: parsingData.TotalScore,
                hash: parsingData.Md5
            );
        }

        public class SearchResult
        {
            public string FileUrl { init; get; }
            public string PreviewUrl { init; get; }
            public string SampleUrl { init; get; }
            public int Id { init; get; }
            public string Rating { init; get; }
            public Tag[] Tags { init; get; }
            public int FileSize { init; get; }
            public int Height { init; get; }
            public int Width { init; get; }
            public int? PreviewHeight { init; get; }
            public int? PreviewWidth { init; get; }
            public CreationInfo CreatedAt { init; get; }
            public string Source { init; get; }
            public int TotalScore { init; get; }
            public string Md5 { init; get; }
        }

        public class Tag
        {
            public int Id { init; get; }
            public string NameEn { init; get; }
            public int Type { init; get; }
            public int PostCount { init; get; }
        }

        public class CreationInfo
        {
            public int S { init; get; }
        }

        private static TagType GetTagType(int type)
        {
            return type switch
            {
                0 => TagType.Trivia,
                1 => TagType.Artist,
                3 => TagType.Copyright,
                4 => TagType.Character,
                8 => TagType.Metadata,
                _ => (TagType)6,
            };
        }

        /*
        private protected override JToken ParseFirstPostSearchResult(object json)
        {
            JArray array = json as JArray;
            return array?.FirstOrDefault() ?? throw new Search.InvalidTags();
        }

        private protected override Search.Post.SearchResult GetPostSearchResult(JToken elem)
        {
            int id = elem["id"].Value<int>();

            var postUriBuilder = new UriBuilder(BaseUrl)
            {
                Host = BaseUrl.Host.Replace("capi-v2", "beta"),
                Path = $"/post/show/{id}",
            };

            var detailedTags = new List<Search.Tag.SearchResult>();
            var tags = new List<string>();
            foreach(var tag in (JArray)elem["tags"])
            {
                var name = tag["name"].Value<string>();
                tags.Add(name);
                
                detailedTags.Add(new Search.Tag.SearchResult(
                    tag["id"].Value<int>(),
                    name,
                    GetTagType(tag["type"].Value<int>()),
                    tag["post_count"].Value<int>()
                    ));
            }

            var url = elem["file_url"].Value<string>();
            var previewUrl = elem["preview_url"].Value<string>();
            var sampleUrl = elem["sample_url"].Value<string>();
            
            return new Search.Post.SearchResult(
                url == null ? null : new Uri(url),
                previewUrl == null ? null : new Uri(previewUrl),
                postUriBuilder.Uri,
                sampleUrl != null && sampleUrl.Contains("/preview/") ? new Uri(sampleUrl) : null,
                GetRating(elem["rating"].Value<string>()[0]),
                tags,
                detailedTags,
                id,
                elem["file_size"].Value<int>(),
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                elem["preview_height"].Value<int?>(),
                elem["preview_width"].Value<int?>(),
                _unixTime.AddSeconds(elem["created_at"]["s"].Value<int>()),
                elem["source"].Value<string>(),
                elem["total_score"].Value<int>(),
                elem["md5"].Value<string>()
                );
        }

        private protected override Search.Post.SearchResult[] GetPostsSearchResult(object json)
        {
            return json is JArray array
                ? array.Select(GetPostSearchResult).ToArray()
                : Array.Empty<Search.Post.SearchResult>();
        }

        private protected override Search.Comment.SearchResult GetCommentSearchResult(object json)
        {
            var elem = (JObject)json;
            var authorToken = elem["author"];

            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                authorToken["id"].Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                authorToken["name"].Value<string>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                _unixTime.AddSeconds(elem["created_at"]["s"].Value<int>()),
                _unixTime.AddSeconds(elem["updated_at"]["s"].Value<int>()),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json) // TODO: Fix TagType values
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        // GetRelatedSearchResult not available
        */
    }
}
