namespace BooruSharp.Booru
{
    public sealed class Sakugabooru : Template.Moebooru
    {
        public Sakugabooru(BooruAuth auth = null) : base("sakugabooru.com", auth) // 750
        { }

        public override bool IsSafe()
            => false;
    }
}
