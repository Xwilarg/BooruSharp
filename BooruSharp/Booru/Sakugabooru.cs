namespace BooruSharp.Booru
{
    public sealed class Sakugabooru : Template.Moebooru
    {
        public Sakugabooru() : base("sakugabooru.com", BooruOptions.noLastComments)
        { }

        public override bool IsSafe()
            => false;
    }
}
