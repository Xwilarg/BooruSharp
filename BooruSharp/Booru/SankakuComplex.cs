namespace BooruSharp.Booru
{
    public class SankakuComplex : Booru
    {
        public SankakuComplex(BooruAuth auth = null) : base("capi-v2.sankakucomplex.com", auth, UrlFormat.sankaku, null)
        { }

        public override bool IsSafe()
            => false;
    }
}
