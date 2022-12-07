using BooruSharp.Search.Post;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public interface IBooru
    {
        public bool CanSearchWithNoTag { get; }

        /// <summary>
        /// Gets whether this booru is considered safe (that is, all posts on
        /// this booru have rating of <see cref="Search.Post.Rating.Safe"/>).
        /// </summary>
        public abstract bool IsSafe { get; }

        /// <summary>
        /// Checks for the booru availability.
        /// Throws <see cref="HttpRequestException"/> if service isn't available.
        /// </summary>
        public Task CheckAvailabilityAsync();

        /// <summary>
        /// Searches for a random post. If <paramref name="tagsArg"/> array is specified
        /// and isn't empty, random post containing those tags will be returned.
        /// </summary>
        /// <param name="tagsArg">The optional array of tags that must be contained in the post.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="Search.TooManyTags"/>
        public Task<PostSearchResult> GetRandomPostAsync(params string[] tagsArg);
    }
}
