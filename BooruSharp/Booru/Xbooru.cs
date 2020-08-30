namespace BooruSharp.Booru
{
    public sealed class Xbooru : Template.Gelbooru02
    {
        public Xbooru() : base("xbooru.com")
        { }

        public override bool IsSafe() => false;
    }
}
