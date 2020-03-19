namespace BooruSharp.Booru
{
    public sealed class Furrybooru : Template.Gelbooru02
    {
        public Furrybooru(BooruAuth auth = null) : base("furry.booru.org", auth, BooruOptions.useHttp)
        { }

        public override bool IsSafe()
            => false;
    }
}
