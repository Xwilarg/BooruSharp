namespace BooruSharp.Booru
{
    public class Furrybooru : Booru
    {
        public Furrybooru(BooruAuth auth = null) : base("furry.booru.org", auth, UrlFormat.indexPhp, null, BooruOptions.useHttp, BooruOptions.noRelated, BooruOptions.noWiki)
        { }

        public override bool IsSafe()
            => false;
    }
}
