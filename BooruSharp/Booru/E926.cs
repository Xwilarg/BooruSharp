namespace BooruSharp.Booru
{
    public class E926 : Booru
    {
        public E926(BooruAuth auth = null) : base("beta.e926.net", auth, UrlFormat.danbooru, 750, BooruOptions.wikiSearchUseTitle, BooruOptions.noTagById)
        { }

        public override bool IsSafe()
            => true;
    }
}
