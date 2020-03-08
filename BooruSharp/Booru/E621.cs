namespace BooruSharp.Booru
{
    public class E621 : Booru
    {
        public E621(BooruAuth auth = null) : base("e621.net", auth, UrlFormat.postIndexJson, 750, BooruOptions.wikiSearchUseTitle, BooruOptions.noTagById)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
