namespace BooruSharp.Booru
{
    public sealed class Konachan : Template.Moebooru
    {
        public Konachan(BooruAuth auth = null) : base("konachan.com", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
