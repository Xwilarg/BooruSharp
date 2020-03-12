using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        private int ClampMinZero(int nb)
            => nb < 0 ? 0 : nb;

        public int? GetLimit()
        {
            return maxLimit;
        }

        public async Task<Search.Post.SearchResult> GetImageAsync(int id)
        {
            return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", "id=" + id));
        }

        public async Task<Search.Post.SearchResult> GetRandomImageAsync(params string[] tagsArg)
        {
            if (format == UrlFormat.indexPhp)
                return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", "id=" + await GetRandomIdAsync(TagsToString(tagsArg))));
            else
                return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg) + "+order:random"));
        }

        private async Task<Search.Post.SearchResult> GetSearchResultFromUrlAsync(string url)
        {
            return GetPostSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url)));
        }

        private Uri CreateUri(string value, string directory)
        {
            if (value.StartsWith("//"))
                return new Uri("http" + (useHttp ? "" : "s") + ":" + value.Replace(" ", "%20"));
            if (!value.StartsWith("http"))
                return new Uri("http" + (useHttp ? "" : "s") + "://img." + baseUrlRaw + "//images/" + directory + "/" + value); // Xbooru
            return new Uri(value.Replace(" ", "%20"));
        }

        protected Search.Post.Rating GetRating(char c)
        {
            switch (c)
            {
                case 's': return (Search.Post.Rating.Safe);
                case 'q': return (Search.Post.Rating.Questionable);
                case 'e': return (Search.Post.Rating.Explicit);
                default: throw new ArgumentException("Invalid rating " + c);
            }
        }
    }
}
