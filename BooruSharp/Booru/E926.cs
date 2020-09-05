namespace BooruSharp.Booru
{
    /// <summary>
    /// E926.
    /// <para>https://e926.net/</para>
    /// </summary>
    public sealed class E926 : Template.E621
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="E926"/> class.
        /// </summary>
        public E926() : base("e926.net")
        { }

        /// <inheritdoc/>
        /// <remarks>
        /// <see cref="E926"/> can provide images with an explicit rating,
        /// but the URL of the file will be <see langword="null"/>.
        /// </remarks>
        public override bool IsSafe() => true;
    }
}
