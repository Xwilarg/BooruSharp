namespace BooruSharp.Booru
{
    /// <summary>
    /// Safebooru.
    /// <para>https://safebooru.org/</para>
    /// </summary>
    public sealed class Safebooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Safebooru"/> class.
        /// </summary>
        public Safebooru()
            : base("safebooru.org")//TODO:, BooruOptions.NoComment)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => true;
    }
}
