namespace BooruSharp.Booru
{
    public sealed class Atfbooru : Template.Danbooru
    {
        public Atfbooru(BooruAuth auth = null) : base("booru.allthefallen.moe", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
