using BooruSharp.Booru;
using BooruSharp.Search.Post;
using System.Threading.Tasks;

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

        public override Task<SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            return base.GetRandomPostAsync(tagsArg);
        }

        // https://github.com/tobiichiamane/pixivcs/blob/master/PixivBaseAPI.cs#L61-L63
        private string _clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private string _clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private string _hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";
    }
}
