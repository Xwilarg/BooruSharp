namespace BooruSharp.Booru
{
    public class SankakuComplex : Template.Sankaku
    {
        public SankakuComplex() : base("capi-v2.sankakucomplex.com")
        { }

        public override bool IsSafe() => false;
    }
}
