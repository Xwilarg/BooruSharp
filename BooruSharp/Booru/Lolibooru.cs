using Newtonsoft.Json.Linq;

namespace BooruSharp.Booru
{
    public sealed class Lolibooru : Template.Moebooru
    {
        public Lolibooru() : base("lolibooru.moe")
        { }

        public override bool IsSafe() => false;

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["tag_type"].Value<int>(),
                elem["post_count"].Value<int>()
                );
        }
    }
}
