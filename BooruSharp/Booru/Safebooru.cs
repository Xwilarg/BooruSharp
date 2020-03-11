namespace BooruSharp.Booru
{
    public class Safebooru : Booru
    {
        public Safebooru(BooruAuth auth = null) : base("safebooru.org", auth, UrlFormat.indexPhp, null, BooruOptions.noComment, BooruOptions.noWiki, BooruOptions.noRelated, BooruOptions.noComment)
        { }

        public override bool IsSafe()
            => true;
    }
}
