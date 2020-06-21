namespace BooruSharp.Booru
{
    public sealed class Gelbooru : Template.Gelbooru
    {
        public Gelbooru() : base("gelbooru.com")
        { }

        public override bool IsSafe()
            => false;
    }
}
