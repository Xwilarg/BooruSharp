namespace BooruSharp.Booru
{
    public sealed class Yandere : Template.Moebooru
    {
        public Yandere(BooruAuth auth = null) : base("yande.re", auth, BooruOptions.noLastComments)
        { }

        public override bool IsSafe()
            => false;
    }
}
