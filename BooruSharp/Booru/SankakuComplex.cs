namespace BooruSharp.Booru
{
    /// <summary>
    /// Sankaku Complex.
    /// <para>https://beta.sankakucomplex.com/</para>
    /// </summary>
    public class SankakuComplex : Template.Sankaku
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SankakuComplex"/> class.
        /// </summary>
        public SankakuComplex() : base("capi-v2.sankakucomplex.com")
        { }

        /// <inheritdoc/>
        public override bool IsSafe => false;
    }
}
