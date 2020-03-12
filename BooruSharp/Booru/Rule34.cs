namespace BooruSharp.Booru
{
    public sealed class Rule34 : Template.Gelbooru02
    {
        public Rule34(BooruAuth auth = null) : base("rule34.xxx", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
