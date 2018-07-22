namespace BooruSharp.Booru
{
    public class Realbooru : Booru
    {
        public Realbooru() : base("realbooru.com", UrlFormat.indexPhp, 200000, BooruOptions.noRelated, BooruOptions.noWiki, BooruOptions.noComment)
        { }
    }
}
