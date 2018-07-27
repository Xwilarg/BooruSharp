namespace BooruSharp.Booru
{
    public class Yandere : Booru
    {
        public Yandere() : base("yande.re", UrlFormat.postIndexXml, null, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
