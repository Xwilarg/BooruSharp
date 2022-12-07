using BooruSharp.Booru.Template;

namespace BooruSharp.Booru
{
    /// <summary>
    /// Twibooru.
    /// <para>https://twibooru.org/</para>
    /// </summary>
    public class Twibooru : BooruOnRails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Twibooru"/> class.
        /// </summary>
        public Twibooru() : base("twibooru.org")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        /// <inheritdoc/>
        protected override int FilterID => 2;
    }
}
