namespace BooruSharp.Booru
{
    public class SankakuComplex : Template.Sankaku
    {
        public SankakuComplex(BooruAuth auth = null) : base("capi-v2.sankakucomplex.com", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
