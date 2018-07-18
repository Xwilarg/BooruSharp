namespace BooruSharp.Booru
{
    public class Gelbooru : Booru
    {
        public Gelbooru() : base("gelbooru.com", UrlFormat.indexPhp, 20000, BooruOptions.noWiki)
        { }
    }
}
