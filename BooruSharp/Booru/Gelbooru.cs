namespace BooruSharp.Booru
{
    public class Gelbooru : Booru
    {
        public Gelbooru(BooruAuth auth = null) : base("gelbooru.com", auth, UrlFormat.indexPhp, 20000, BooruOptions.noWiki, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
