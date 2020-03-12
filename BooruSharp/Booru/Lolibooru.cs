namespace BooruSharp.Booru
{
    public sealed class Lolibooru : Template.Moebooru
    {
        public Lolibooru(BooruAuth auth = null) : base("lolibooru.moe", auth)
        { }

        public override bool IsSafe()
            => false;
    }
}
