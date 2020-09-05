namespace BooruSharp.Booru
{
    /// <summary>
    /// FurryBooru.
    /// <para>https://furry.booru.org/</para>
    /// </summary>
    public sealed class Furrybooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Furrybooru"/> class.
        /// </summary>
        public Furrybooru() : base("furry.booru.org", BooruOptions.useHttp)
        { }

        /// <inheritdoc/>
        public override bool IsSafe() => false;
    }
}
