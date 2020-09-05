namespace BooruSharp.Booru
{
    /// <summary>
    /// Yande.re.
    /// <para>https://yande.re/</para>
    /// </summary>
    public sealed class Yandere : Template.Moebooru
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Yandere"/> class.
        /// </summary>
        public Yandere() : base("yande.re", BooruOptions.noLastComments)
        { }

        /// <inheritdoc/>
        public override bool IsSafe() => false;
    }
}
