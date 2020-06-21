namespace BooruSharp.Booru
{
    public sealed class Furrybooru : Template.Gelbooru02
    {
        public Furrybooru() : base("furry.booru.org", BooruOptions.useHttp)
        { }

        public override bool IsSafe()
            => false;
    }
}
