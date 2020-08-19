namespace BooruSharp.Booru
{
    public sealed class Yandere : Template.Moebooru
    {
        public Yandere() : base("yande.re", BooruOptions.noLastComments)
        { }

        public override bool IsSafe() => false;
    }
}
