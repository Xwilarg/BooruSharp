namespace BooruSharp.Booru
{
    public class DanbooruDonmai : Booru
    {
        public DanbooruDonmai() : base("danbooru.donmai.us", UrlFormat.danbooru, 1000, BooruOptions.wikiSearchUseTitle, BooruOptions.noRelated, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
