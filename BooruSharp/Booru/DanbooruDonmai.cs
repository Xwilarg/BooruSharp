namespace BooruSharp.Booru
{
    public sealed class DanbooruDonmai : Template.Danbooru
    {
        public DanbooruDonmai(BooruAuth auth = null) : base("danbooru.donmai.us", auth, BooruOptions.noMoreThan2Tags)
        { }

        public override bool IsSafe()
            => false;
    }
}
