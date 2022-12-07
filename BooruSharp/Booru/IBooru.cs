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
        /// Checks for the booru availability.
        /// Throws <see cref="HttpRequestException"/> if service isn't available.
        /// </summary>
        public Task CheckAvailabilityAsync();
    }
}
