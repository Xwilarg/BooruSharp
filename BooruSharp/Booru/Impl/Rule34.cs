namespace BooruSharp.Booru
{
    /// <summary>
    /// Rule 34.
    /// <para>https://rule34.xxx/</para>
    /// </summary>
    public sealed class Rule34 : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule34"/> class.
        /// </summary>
        public Rule34()
            // The limit is in fact 200000 but search with tags make it incredibly hard to know what is really your pid
            : base("rule34.xxx") //TODO:, BooruOptions.NoComment | BooruOptions.LimitOf20000)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
