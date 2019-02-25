namespace BooruSharp.Booru
{
    public class Yandere : Booru
    {
        public Yandere(BooruAuth auth = null) : base("yande.re", auth, UrlFormat.postIndexXml, null, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
