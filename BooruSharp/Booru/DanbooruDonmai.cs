namespace BooruSharp.Booru
{
    public sealed class DanbooruDonmai : Template.Danbooru
    {
        public DanbooruDonmai() : base("danbooru.donmai.us", BooruOptions.noMoreThan2Tags)
        { }

        public override bool IsSafe()
            => false;
    }
}
