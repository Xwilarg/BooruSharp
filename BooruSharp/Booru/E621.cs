namespace BooruSharp.Booru
{
    public sealed class E621 : Template.E621
    {
        public E621() : base("e621.net")
        { }

        public override bool IsSafe() => false;
    }
}
