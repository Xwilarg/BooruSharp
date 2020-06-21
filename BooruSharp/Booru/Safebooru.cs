namespace BooruSharp.Booru
{
    public sealed class Safebooru : Template.Gelbooru02
    {
        public Safebooru() : base("safebooru.org", BooruOptions.noComment)
        { }

        public override bool IsSafe()
            => true;
    }
}
