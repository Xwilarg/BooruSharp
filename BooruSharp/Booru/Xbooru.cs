namespace BooruSharp.Booru
{
    public sealed class Xbooru : Template.Gelbooru02
    {
        public Xbooru(BooruAuth auth = null) : base("xbooru.com", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
