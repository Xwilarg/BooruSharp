namespace BooruSharp.Booru
{
    public sealed class Gelbooru : Template.Gelbooru
    {
        public Gelbooru(BooruAuth auth = null) : base("gelbooru.com", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
