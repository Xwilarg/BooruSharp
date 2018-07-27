namespace BooruSharp.Booru
{
    public class Konachan : Booru
    {
        public Konachan() : base("konachan.com", UrlFormat.postIndexXml, null)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
