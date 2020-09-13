using Newtonsoft.Json.Linq;

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
