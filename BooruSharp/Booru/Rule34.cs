namespace BooruSharp.Booru
{
    public class Rule34 : Booru
    {
        public Rule34(BooruAuth auth = null) : base("rule34.xxx", auth, UrlFormat.indexPhp, 20000, BooruOptions.noWiki, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
