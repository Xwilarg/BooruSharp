using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<int> GetNbImage(params string[] tags)
        {
            string url;
            if (format == UrlFormat.danbooru)
                url = baseUrl + "/counts/posts.xml?" + TagsToString(tags);
            else
                url = CreateUrl(imageUrl, "limit=1", TagsToString(tags));
            XmlDocument xml = await GetXml(url);
            return (GetNbImageInternal(xml));
        }

        private int GetNbImageInternal(XmlDocument xml)
        {
            if (format == UrlFormat.danbooru)
                return ClampMinZero(Convert.ToInt32(xml.ChildNodes.Item(1).FirstChild.InnerXml));
            else
                return ClampMinZero(Convert.ToInt32(xml.ChildNodes.Item(1).Attributes[0].InnerXml));
        }

        private int ClampMinZero(int nb)
            => nb < 0 ? 0 : nb;

        public int? GetLimit()
        {
            return maxLimit;
        }

        public async Task<Search.Post.SearchResult> GetImageAsync(int offset, params string[] tagsArg)
        {
            var results = JsonConvert.DeserializeObject<Search.Post.SearchResultJson[]>(await GetJsonAsync(CreateUrl(imageUrl, "limit=1", GetPage() + offset, TagsToString(tagsArg))));
            if (results.Length == 0)
                throw new Search.InvalidTags();
            var result = results[0];
            return new Search.Post.SearchResult(
                new Uri((result.fileUrl.StartsWith("//") ? "http" + (useHttp ? "" : "s") + ":" : "") + result.fileUrl.Replace(" ", "%20")),
                new Uri((result.previewUrl.StartsWith("//") ? "http" + (useHttp ? "" : "s") + ":" : "") + result.previewUrl.Replace(" ", "%20")),
                GetRating(result.rating[0]),
                result.tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                Convert.ToInt32(result.id),
                result.fileSize ?? Convert.ToInt32(result.fileSize),
                Convert.ToInt32(result.height),
                Convert.ToInt32(result.width),
                Convert.ToInt32(result.previewHeight),
                Convert.ToInt32(result.previewWidth),
                ParseDateTime(result.createdAt),
                result.source,
                result.score == "" ? 0 : Convert.ToInt32(result.score));
        }

        public async Task<Search.Post.SearchResult> GetRandomImageAsync(params string[] tags)
        {
            int nbMax = await GetNbImage(tags);
            if (nbMax == 0)
                throw new Search.InvalidTags();
            if (GetLimit() != null && GetLimit() < nbMax)
                nbMax = GetLimit().Value;
            int firstPage = GetFirstPage();
            int randomNb = random.Next(firstPage, nbMax + firstPage);
            return (await GetImage(randomNb, tags));
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
