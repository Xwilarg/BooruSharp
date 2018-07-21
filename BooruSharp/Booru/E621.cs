namespace BooruSharp.Booru
{
    public class E621 : Booru
    {
        public E621() : base("e621.net", UrlFormat.postIndexXml, 750, BooruOptions.wikiSearchUseTitle, BooruOptions.noTagById)
        { }
    }
}
