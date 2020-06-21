namespace BooruSharp.Booru
{
    public sealed class Atfbooru : Template.Danbooru
    {
        public Atfbooru() : base("booru.allthefallen.moe")
        { }

        public override bool IsSafe()
            => false;
    }
}
