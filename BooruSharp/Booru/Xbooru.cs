namespace BooruSharp.Booru
{
    public class Xbooru : Booru
    {
        public Xbooru() : base("xbooru.com", UrlFormat.indexPhp, null, BooruOptions.noWiki, BooruOptions.noRelated)
        { }
    }
}
