namespace BooruSharp.Booru
{
    public class Lolibooru : Booru
    {
        public Lolibooru(BooruAuth auth = null) : base("lolibooru.moe", auth, UrlFormat.postIndexXml, null, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
