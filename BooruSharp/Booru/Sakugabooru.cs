namespace BooruSharp.Booru
{
    public sealed class Sakugabooru : Template.Moebooru
    {
        public Sakugabooru(BooruAuth auth = null) : base("sakugabooru.com", auth, BooruOptions.noLastComments)
        { }

        public override bool IsSafe()
            => false;
    }
}
