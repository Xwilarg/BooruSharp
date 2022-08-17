using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Ponybooru.
    /// <para>https://ponybooru.org/</para>
    /// </summary>
    public class Ponybooru : Philomena
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Ponybooru"/> class.
        /// </summary>
        public Ponybooru() : base("ponybooru.org")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        protected override int FilterID => 2;
    }
}
