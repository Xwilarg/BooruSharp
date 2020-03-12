namespace BooruSharp.Booru
{
    public sealed class E621 : Template.E621
    {
        public E621(BooruAuth auth = null) : base("e621.net", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
