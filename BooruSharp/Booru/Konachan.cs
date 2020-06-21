namespace BooruSharp.Booru
{
    public sealed class Konachan : Template.Moebooru
    {
        public Konachan() : base("konachan.com")
        { }

        public override bool IsSafe()
            => false;
    }
}
