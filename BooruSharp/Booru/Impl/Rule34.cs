using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System;
using BooruSharp.Search;
using BooruSharp.Search.Post;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Rule 34.
    /// <para>https://rule34.xxx/</para>
    /// </summary>
    public sealed class Rule34 : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule34"/> class.
        /// </summary>
        public Rule34()
            : base("api.rule34.xxx") //TODO:, BooruOptions.NoComment
        { }

        public override bool CanSearchWithNoTag => false;

        public override Uri PostBaseUrl => new("https://rule34.xxx");

        protected override async Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            // We don't have to handle what happen when there is no tag because it'll throw before

            var url = new Uri($"{_imageUrl}&limit=1&id={string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant()}&json=0");
            XmlDocument xml = await GetXmlAsync(url.AbsoluteUri);
            int max = int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);

            if (max == 0)
                throw new InvalidTags();

            // The limit is in fact 200000 but search with tags make it incredibly hard to know what is really your pid
            if (max > 20001)
                max = 20001;

            return new Uri($"{_imageUrl}&limit=1&tags={string.Join("+", tags.Select(Uri.EscapeDataString)).ToLowerInvariant()}&pid={Random.Next(0, max)}&json=1");
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var data = await GetDataAsync<SearchResult[]>(uri);
            if (!data.Any())
            {
                throw new InvalidTags();
            }
            var parsingData = data[0];


            return new PostSearchResult(
                fileUrl: new($"{parsingData.FileUrl}"),
                previewUrl: new($"{parsingData.PreviewUrl}"),
                postUrl: new($"{PostBaseUrl}index.php?page=post&s=view&id={parsingData.Id}"),
                sampleUri: parsingData.Sample ? new($"{parsingData.SampleUrl}") : null,
                rating: GetRating(parsingData.Rating[0]),
                tags: parsingData.Tags.Split(),
                detailedTags: null,
                id: parsingData.Id,
                size: null,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: null,
                previewWidth: null,
                creation: null,
                sources: null,
                score: parsingData.Score,
                hash: parsingData.Hash
            );
        }

        public class SearchResult
        {
            public string PreviewUrl { init; get; }
            public string SampleUrl { init; get; }
            public string FileUrl { init; get; }
            public string Image { init; get; }
            public int Id { init; get; }
            public bool Sample { init; get; }
            public string Rating { init; get; }
            public string Tags { init; get; }
            public int Height { init; get; }
            public int Width { init; get; }
            public int? Score { init; get; }
            public string Hash { init; get; }
        }
    }
}
