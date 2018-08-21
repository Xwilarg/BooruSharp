namespace BooruSharp.Booru
{
    public class Sakugabooru : Booru
    {
        public Sakugabooru() : base("sakugabooru.com", UrlFormat.postIndexXml, 750, BooruOptions.noComment)
        { }

        public override bool IsSafe()
        {
            return (false);
        }
    }
}
