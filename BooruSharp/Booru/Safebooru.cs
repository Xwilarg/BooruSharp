namespace BooruSharp.Booru
{
    public class Safebooru : Booru
    {
        public Safebooru() : base("safebooru.org", UrlFormat.indexPhp, null, BooruOptions.noWiki, BooruOptions.noRelated, BooruOptions.noComment)
        { }
    }
}
