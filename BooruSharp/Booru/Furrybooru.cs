using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// FurryBooru.
    /// <para>https://furry.booru.org/</para>
    /// </summary>
    [Obsolete("Furrybooru does no longer works, please consider using E621/E926 instead", error: true)] // TODO: Looks like it's fixed
    public sealed class Furrybooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Furrybooru"/> class.
        /// </summary>
        public Furrybooru()
            : base("furry.booru.org", BooruOptions.UseHttp)
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
