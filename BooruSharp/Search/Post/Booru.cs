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
            var results = JsonConvert.DeserializeObject<Search.Post.SearchResultJson[]>(await GetJsonAsync(url)); // TODO: xbooru manage samples differently
            if (results.Length == 0)
                throw new Search.InvalidTags();
            var result = results[0];
            return new Search.Post.SearchResult(
                CreateUri(result.fileUrl, result.directory),
                result.previewUrl != null ? CreateUri(result.previewUrl, result.directory) : null,
                GetRating(result.rating[0]),
                result.tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32(result.id),
                result.fileSize ?? Convert.ToInt32(result.fileSize),
                Convert.ToInt32(result.height),
                Convert.ToInt32(result.width),
                Convert.ToInt32(result.previewHeight),
                Convert.ToInt32(result.previewWidth),
                result.createdAt != null ? ParseDateTime(result.createdAt) : (DateTime?)null,
                result.source,
                result.score == "" ? 0 : Convert.ToInt32(result.score));
        }

        private Uri CreateUri(string value, string directory)
        {
            if (value.StartsWith("//"))
                return new Uri("http" + (useHttp ? "" : "s") + ":" + value.Replace(" ", "%20"));
            if (!value.StartsWith("http"))
                return new Uri("http" + (useHttp ? "" : "s") + "://img." + baseUrlRaw + "//images/" + directory + "/" + value); // Xbooru
            return new Uri(value.Replace(" ", "%20"));
        }

        private Search.Post.Rating GetRating(char c)
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
