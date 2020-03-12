namespace BooruSharp.Booru
{
    public class Realbooru : Template.Gelbooru02
    {
        public Realbooru(BooruAuth auth = null) : base("realbooru.com", auth) // 200000
        { }

        public override bool IsSafe()
            => false;
    }
}
