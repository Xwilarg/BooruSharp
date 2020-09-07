namespace BooruSharp.Booru
{
    /// <summary>
    /// Xbooru.
    /// <para>https://xbooru.com/</para>
    /// </summary>
    public sealed class Xbooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Xbooru"/> class.
        /// </summary>
        public Xbooru()
            : base("xbooru.com")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
