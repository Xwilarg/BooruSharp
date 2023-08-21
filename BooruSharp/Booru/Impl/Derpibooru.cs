using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Derpibooru.
    /// <para>https://derpibooru.org/</para>
    /// </summary>
    public class Derpibooru : Philomena
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Derpibooru"/> class.
        /// </summary>
        public Derpibooru() : base("derpibooru.org")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        protected override int FilterID => 56027;
    }
}
