namespace BooruSharp.Booru
{
    public class Lolibooru : Booru
    {
        public Lolibooru(BooruAuth auth = null) : base("lolibooru.moe", auth, UrlFormat.postIndexJson, null, BooruOptions.noRelated)
        { }

        public override bool IsSafe()
            => false;
    }
}
