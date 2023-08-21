namespace BooruSharp.Booru
{
    /// <summary>
    /// Sakugabooru.
    /// <para>https://www.sakugabooru.com/</para>
    /// </summary>
    public sealed class Sakugabooru : Template.Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sakugabooru"/> class.
        /// </summary>
        public Sakugabooru()
            : base("sakugabooru.com")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
        /// <inheritdoc/>
        public override bool HasPostByIdAPI => false;
    }
}
