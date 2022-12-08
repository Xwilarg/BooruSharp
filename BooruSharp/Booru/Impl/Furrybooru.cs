using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System;

namespace BooruSharp.Booru
{
    /// <summary>
    /// FurryBooru.
    /// <para>https://furry.booru.org/</para>
    /// </summary>
    public sealed class Furrybooru : Template.Gelbooru02
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Furrybooru"/> class.
        /// </summary>
        public Furrybooru()
            : base("furry.booru.org")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;

        public override bool CanSearchWithNoTag => false;
    }
}
