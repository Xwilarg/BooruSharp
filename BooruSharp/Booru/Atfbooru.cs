namespace BooruSharp.Booru
{
    public class Atfbooru : Booru
    {
        public Atfbooru(BooruAuth auth = null) : base("atfbooru.ninja", auth, UrlFormat.danbooru, 1000, BooruOptions.wikiSearchUseTitle, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
