namespace BooruSharp.Booru
{
    public class Furrybooru : Booru
    {
        public Furrybooru() : base("furry.booru.org", UrlFormat.indexPhp, null, BooruOptions.useHttp, BooruOptions.noRelated, BooruOptions.noWiki)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
