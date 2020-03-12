namespace BooruSharp.Booru
{
    public sealed class E926 : Template.E621
    {
        public E926(BooruAuth auth = null) : base("beta.e926.net", auth)
        { }

        public override bool IsSafe()
            => true;
    }
}
