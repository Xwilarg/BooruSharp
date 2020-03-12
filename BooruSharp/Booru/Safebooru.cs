namespace BooruSharp.Booru
{
    public sealed class Safebooru : Template.Gelbooru02
    {
        public Safebooru(BooruAuth auth = null) : base("safebooru.org", auth)
        { }

        public override bool IsSafe()
            => true;
    }
}
