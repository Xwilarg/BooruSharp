using Newtonsoft.Json.Linq;

namespace BooruSharp.Booru
{
    public sealed class Lolibooru : Template.Moebooru
    {
        public Lolibooru(BooruAuth auth = null) : base("lolibooru.moe", auth)
        { }

        public override bool IsSafe()
            => false;

        protected internal override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["type"].Value<int>(),
                elem["post_count"].Value<int>()
                );
        }
    }
}
