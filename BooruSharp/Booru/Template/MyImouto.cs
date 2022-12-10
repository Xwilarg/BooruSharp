using BooruSharp.Search.Post;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BooruSharp.Booru.Template
{
    /// <summary>
    /// Template booru based on MyImouto. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class MyImouto : Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyImouto"/> template class.
        /// </summary>
        /// <param name="domain">
        /// The fully qualified domain name. Example domain
        /// name should look like <c>www.google.com</c>.
        /// </param>
        protected MyImouto(string domain)
            : base(domain)
        { }

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var parsingData = (await GetDataAsync<SearchResult[]>(uri))[0];

            return new PostSearchResult(
                fileUrl: new(parsingData.FileUrl),
                previewUrl: new(parsingData.PreviewUrl),
                postUrl: new Uri($"{BaseUrl}post/show/{parsingData.Id}"),
                sampleUri: parsingData.SampleUrl != null ? new Uri(parsingData.SampleUrl) : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.Tags.Split().Select(HttpUtility.HtmlDecode),
                detailedTags: null,
                id: parsingData.Id,
                size: parsingData.FileSize,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: parsingData.PreviewHeight,
                previewWidth: parsingData.PreviewWidth,
                creation: _unixTime.AddSeconds(parsingData.CreatedAt),
                sources: string.IsNullOrEmpty(parsingData.Source) ? Array.Empty<string>() : new[] { parsingData.Source },
                score: int.Parse(parsingData.Score),
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
            public string Tags { init; get; }
            public int FileSize { init; get; }
            public int Height { init; get; }
            public int Width { init; get; }
            public int PreviewHeight { init; get; }
            public int PreviewWidth { init; get; }
            public int CreatedAt { init; get; }
            public string Source { init; get; }
            public string Score { init; get; }
            public string Md5 { init; get; }
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
            var sampleUrl = elem["sample_url"].Value<string>();

            return new Search.Post.SearchResult(
                new Uri(elem["file_url"].Value<string>()),
                new Uri(elem["preview_url"].Value<string>()),
                new Uri(BaseUrl + "post/show/" + id),
                string.IsNullOrWhiteSpace(sampleUrl) ? null : new Uri(sampleUrl),
                GetRating(elem["rating"].Value<string>()[0]),
                elem["tags"].Value<string>().Split(' '),
                null,
                id,
                elem["file_size"].Value<int>(),
                elem["height"].Value<int>(),
                elem["width"].Value<int>(),
                elem["preview_height"].Value<int>(),
                elem["preview_width"].Value<int>(),
                _unixTime.AddSeconds(elem["created_at"].Value<int>()),
                elem["source"].Value<string>(),
                elem["score"].Value<int>(),
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
            return new Search.Comment.SearchResult(
                elem["id"].Value<int>(),
                elem["post_id"].Value<int>(),
                elem["creator_id"].Value<int?>(),
                elem["created_at"].Value<DateTime>(),
                elem["creator"].Value<string>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Wiki.SearchResult GetWikiSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Wiki.SearchResult(
                elem["id"].Value<int>(),
                elem["title"].Value<string>(),
                elem["created_at"].Value<DateTime>(),
                elem["updated_at"].Value<DateTime>(),
                elem["body"].Value<string>()
                );
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["type"].Value<int>(),
                elem["count"].Value<int>()
                );
        }

        private protected override Search.Related.SearchResult GetRelatedSearchResult(object json)
        {
            var elem = (JArray)json;
            return new Search.Related.SearchResult(
                elem[0].Value<string>(),
                elem[1].Value<int>()
                );
        }
        */
    }
}
