namespace BooruSharp.Booru
{
    /// <summary>
    /// Gelbooru.
    /// <para>https://gelbooru.com/</para>
    /// </summary>
    public sealed class Gelbooru : Template.Gelbooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Gelbooru"/> class.
        /// </summary>
        public Gelbooru() : base("gelbooru.com")
        { }

        /// <inheritdoc/>
        public override bool IsSafe() => false;
    }
}
