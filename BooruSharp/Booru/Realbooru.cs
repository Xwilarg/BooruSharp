namespace BooruSharp.Booru
{
    public sealed class Realbooru : Template.Gelbooru02
    {
        public Realbooru() : base("realbooru.com")
        { }

        public override bool IsSafe()
            => false;
    }
}
