using BooruSharp.Search.Post;
using System.Threading.Tasks;
using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Realbooru.
    /// <para>http://realbooru.com/</para>
    /// </summary>
    public sealed class Realbooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Realbooru"/> class.
        /// </summary>
        public Realbooru()
            : base("realbooru.com")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var parsingData = (await GetDataAsync<SearchResult[]>(uri))[0];

            return new PostSearchResult(
                fileUrl: new($"{FileBaseUrl}images/{parsingData.Directory}/{parsingData.Image}"),
                previewUrl: new($"{PreviewBaseUrl}thumbnails/{parsingData.Directory}/thumbnails_{parsingData.Image}"),
                postUrl: new($"{PostBaseUrl}index.php?page=post&s=view&id={parsingData.Id}"),
                sampleUri: parsingData.Sample == 1 ? new($"{SampleBaseUrl}samples/{parsingData.Directory}/sample_{parsingData.Image}.jpg") : null,
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
                hash: null
            );
        }

        public class SearchResult
        {
            public string Directory { init; get; }
            public string Image { init; get; }
            public int Id { init; get; }
            public int Sample { init; get; }
            public string Rating { init; get; }
            public string Tags { init; get; }
            public int Height { init; get; }
            public int Width { init; get; }
            public int? Score { init; get; }
        }
    }
}
