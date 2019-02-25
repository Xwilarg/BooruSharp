namespace BooruSharp.Booru
{
    public class DanbooruDonmai : Booru
    {
        public DanbooruDonmai(BooruAuth auth = null) : base("danbooru.donmai.us", auth, UrlFormat.danbooru, 1000, BooruOptions.wikiSearchUseTitle, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
