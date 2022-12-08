﻿namespace BooruSharp.Booru
{
    /// <summary>
    /// Lolibooru.
    /// <para>https://lolibooru.moe/</para>
    /// </summary>
    public sealed class Lolibooru : Template.MyImouto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lolibooru"/> class.
        /// </summary>
        public Lolibooru()
            : base("lolibooru.moe")//TODO:, BooruOptions.NoTagByID) // Tag search now returns 500 error
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /*

        private protected override Search.Tag.SearchResult GetTagSearchResult(object json)
        {
            var elem = (JObject)json;
            return new Search.Tag.SearchResult(
                elem["id"].Value<int>(),
                elem["name"].Value<string>(),
                (Search.Tag.TagType)elem["tag_type"].Value<int>(),
                elem["post_count"].Value<int>()
                );
        }*/
    }
}
