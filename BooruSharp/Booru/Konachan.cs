namespace BooruSharp.Booru
{
    public class Konachan : Booru
    {
        public Konachan(BooruAuth auth = null) : base("konachan.com", auth, UrlFormat.postIndexXml, null)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
