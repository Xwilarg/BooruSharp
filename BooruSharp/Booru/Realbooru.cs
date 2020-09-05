namespace BooruSharp.Booru
{
    /// <summary>
    /// Realbooru.
    /// <para>http://realbooru.com/</para>
    /// </summary>
    public sealed class Realbooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Realbooru"/> class.
        /// </summary>
        public Realbooru() : base("realbooru.com")
        { }

        /// <inheritdoc/>
        public override bool IsSafe() => false;
    }
}
