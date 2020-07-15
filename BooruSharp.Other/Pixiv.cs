using BooruSharp.Booru;

namespace BooruSharp.Other
{
    public class Pixiv : ABooru
    {
        public Pixiv() : base("app-api.pixiv.net", (UrlFormat)(-1),
            new[] { BooruOptions.noComment, BooruOptions.noFavorite, BooruOptions.noLastComments, BooruOptions.noMultipleRandom, BooruOptions.noPostById,
                BooruOptions.noPostByMd5, BooruOptions.noPostCount, BooruOptions.noRelated, BooruOptions.noTagById, BooruOptions.noWiki })
        { }

        public override bool IsSafe()
            => false;
    }
}
