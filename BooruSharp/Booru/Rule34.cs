namespace BooruSharp.Booru
{
    public sealed class Rule34 : Template.Gelbooru02
    {
        public Rule34() : base("rule34.xxx", BooruOptions.noComment)
        { }

        public override bool IsSafe()
            => false;
    }
}
