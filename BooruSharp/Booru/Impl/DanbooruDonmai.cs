using System.Linq;
using System.Threading.Tasks;
using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Danbooru.
    /// <para>https://danbooru.donmai.us/</para>
    /// </summary>
    public sealed class DanbooruDonmai : Template.Danbooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DanbooruDonmai"/> class.
        /// </summary>
        public DanbooruDonmai()
            : base("danbooru.donmai.us")
        { }

        protected override Task<Uri> CreateRandomPostUriAsync(string[] tags)
        {
            if (tags.Length > 2)
            {
                throw new Search.TooManyTags();
            }
            return base.CreateRandomPostUriAsync(tags);
        }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
