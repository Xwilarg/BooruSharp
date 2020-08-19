namespace BooruSharp.Booru
{
    public sealed class Rule34 : Template.Gelbooru02
    {
        public Rule34() : base("rule34.xxx", BooruOptions.noComment | BooruOptions.limitOf20000) // The limit is in fact 200000 but search with tags make it incredibly hard to know what is really your pid
        { }

        public override bool IsSafe() => false;
    }
}
