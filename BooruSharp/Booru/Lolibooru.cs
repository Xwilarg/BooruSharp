namespace BooruSharp.Booru
{
    public class Lolibooru : Booru
    {
        public Lolibooru() : base("lolibooru.moe", UrlFormat.postIndexXml, null, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
