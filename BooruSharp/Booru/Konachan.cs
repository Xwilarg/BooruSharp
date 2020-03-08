namespace BooruSharp.Booru
{
    public class Konachan : Booru
    {
        public Konachan(BooruAuth auth = null) : base("konachan.com", auth, UrlFormat.postIndexJson, null)
        { }

        public override bool IsSafe()
            => false;
    }
}
