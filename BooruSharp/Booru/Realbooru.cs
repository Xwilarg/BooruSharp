namespace BooruSharp.Booru
{
    public sealed class Realbooru : Template.Gelbooru02
    {
        public Realbooru(BooruAuth auth = null) : base("realbooru.com", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
