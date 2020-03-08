namespace BooruSharp.Booru
{
    public class Xbooru : Booru
    {
        public Xbooru(BooruAuth auth = null) : base("xbooru.com", auth, UrlFormat.indexPhp, null, BooruOptions.noWiki, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
            => false;
    }
}
