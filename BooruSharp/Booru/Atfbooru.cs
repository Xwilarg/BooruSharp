namespace BooruSharp.Booru
{
    public class Atfbooru : Booru
    {
        public Atfbooru() : base("atfbooru.ninja", UrlFormat.danbooru, 1000, BooruOptions.wikiSearchUseTitle, BooruOptions.noRelated, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
