using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public interface IBooru
    {
        /// <summary>
        /// Gets whether this booru is considered safe (that is, all posts on
        /// this booru have rating of <see cref="Search.Post.Rating.Safe"/>).
        /// </summary>
        public abstract bool IsSafe { get; }

        /// <summary>
        /// Gets whether it is possible to search for related tags on this booru.
        /// </summary>
        public bool HasRelatedAPI { get; }

        /// <summary>
        /// Gets whether it is possible to search for wiki entries on this booru.
        /// </summary>
        public bool HasWikiAPI { get; }

        /// <summary>
        /// Gets whether it is possible to search for comments on this booru.
        /// </summary>
        public bool HasCommentAPI { get; }

        /// <summary>
        /// Gets whether it is possible to search for tags by their IDs on this booru.
        /// </summary>
        public bool HasTagByIdAPI { get; }

        /// <summary>
        /// Gets whether it is possible to search for the last comments on this booru.
        /// </summary>
        // As a failsafe also check for the availability of comment API.
        public bool HasSearchLastComment { get; }

        /// <summary>
        /// Gets whether it is possible to search for posts by their MD5 on this booru.
        /// </summary>
        public bool HasPostByMd5API { get; }

        /// <summary>
        /// Gets whether it is possible to search for posts by their ID on this booru.
        /// </summary>
        public bool HasPostByIdAPI { get; }

        /// <summary>
        /// Gets whether it is possible to get the total number of posts on this booru.
        /// </summary>
        public bool HasPostCountAPI { get; }

        /// <summary>
        /// Gets whether it is possible to get multiple random images on this booru.
        /// </summary>
        public bool HasMultipleRandomAPI { get; }

        /// <summary>
        /// Gets whether this booru supports adding or removing favorite posts.
        /// </summary>
        public bool HasFavoriteAPI { get; }

        /// <summary>
        /// Gets whether this booru can't call post functions without search arguments.
        /// </summary>
        public bool NoEmptyPostSearch { get; }

        /// <summary>
        /// Gets a value indicating whether searching by more than two tags at once is not allowed.
        /// </summary>
        public bool NoMoreThanTwoTags { get; }

        /// <summary>
        /// Checks for the booru availability.
        /// Throws <see cref="HttpRequestException"/> if service isn't available.
        /// </summary>
        public Task CheckAvailabilityAsync();
    }
}
