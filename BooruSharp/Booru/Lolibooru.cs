using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Lolibooru.
    /// <para>https://lolibooru.moe/</para>
    /// </summary>
    public sealed class Lolibooru : Template.Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lolibooru"/> class.
        /// </summary>
        public Lolibooru()
            : base("lolibooru.moe")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        public async override Task CheckAvailabilityAsync()
        {
            // It seems this booru doesn't comprehend HEAD requests to its API.
            var request = new HttpRequestMessage(HttpMethod.Get, "https://lolibooru.moe/post/index.xml");

            using (var response = await GetResponseAsync(request))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        private protected override Search.Tag.SearchResult GetTagSearchResult(JToken token)
        {
            return new Search.Tag.SearchResult(
                token["id"].Value<int>(),
                token["name"].Value<string>(),
                (Search.Tag.TagType)token["tag_type"].Value<int>(),
                token["post_count"].Value<int>()
                );
        }
    }
}
