using BooruSharp.Booru.Template;
using BooruSharp.Search.Post;
using BooruSharp.Search;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Twibooru.
    /// <para>https://twibooru.org/</para>
    /// </summary>
    public class Twibooru : BooruOnRails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Twibooru"/> class.
        /// </summary>
        public Twibooru() : base("twibooru.org")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        protected override int FilterID => 2;

        private protected override async Task<PostSearchResult> GetPostSearchResultAsync(Uri uri)
        {
            var posts = await GetDataAsync<PostContainer>(uri);
            if (posts.Posts != null && !posts.Posts.Any())
            {
                throw new InvalidTags();
            }
            var parsingData = posts.Posts == null ? posts.Post : posts.Posts[0];

            Rating rating;
            if (parsingData.Tags.Contains("explicit")) rating = Rating.Explicit;
            else if (parsingData.Tags.Contains("questionable")) rating = Rating.Questionable;
            else if (parsingData.Tags.Contains("suggestive")) rating = Rating.Safe;
            else if (parsingData.Tags.Contains("safe")) rating = Rating.General;
            else rating = (Rating)(-1); // Some images doesn't have a rating
            return new PostSearchResult(
                fileUrl: new(parsingData.Representations.Full),
                previewUrl: new(parsingData.Representations.Thumb),
                postUrl: new($"{PostBaseUrl}{parsingData.Id}"),
                sampleUri: new(parsingData.Representations.Large),
                rating: rating,
                tags: parsingData.Tags,
                detailedTags: null,
                id: parsingData.Id,
                size: parsingData.Size,
                height: parsingData.Height,
                width: parsingData.Width,
                previewHeight: null,
                previewWidth: null,
                creation: parsingData.CreatedAt,
                sources: string.IsNullOrEmpty(parsingData.SourceUrl) ? Array.Empty<string>() : new[] { parsingData.SourceUrl },
                score: parsingData.Score,
                hash: parsingData.Sha512Hash
            );
        }
    }
}
