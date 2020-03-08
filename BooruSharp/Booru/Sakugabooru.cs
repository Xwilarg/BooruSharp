namespace BooruSharp.Booru
{
    public class Sakugabooru : Booru
    {
        public Sakugabooru(BooruAuth auth = null) : base("sakugabooru.com", auth, UrlFormat.postIndexJson, 750, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
