namespace BooruSharp.Booru
{
    public class Realbooru : Booru
    {
        public Realbooru(BooruAuth auth = null) : base("realbooru.com", auth, UrlFormat.indexPhp, 200000, BooruOptions.noRelated, BooruOptions.noWiki, BooruOptions.noComment)
        { }

        public override bool IsSafe()
            => false;
    }
}
