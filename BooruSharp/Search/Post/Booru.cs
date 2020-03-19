using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class Booru
    {
        public async Task<Search.Post.SearchResult> GetImageByMd5Async(string md5)
        {
            if (!HavePostByMd5API())
                throw new Search.FeatureUnavailable();
            return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", "md5=" + md5));
        }

        public async Task<Search.Post.SearchResult> GetRandomImageAsync(params string[] tagsArg)
        {
            tagsArg = tagsArg.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (format == UrlFormat.indexPhp)
            {
                if (tagsArg.Length == 0)
                    return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", "id=" + await GetRandomIdAsync(TagsToString(tagsArg)))); // We need to request /index.php?page=post&s=random and get the id given by the redirect
                else
                {
                    // The previous option doesn't work if there are tags so we contact the XML endpoint to get post count
                    string url = CreateUrl(imageUrlXml, "limit=1", TagsToString(tagsArg));
                    XmlDocument xml = await GetXmlAsync(url);
                    int max = int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);
                    if (maxLimit)
                        max = 20001;
                    return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg), "pid=" + random.Next(0, max)));
                }
            }
            else
                return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg) + "+order:random"));
        }

        private async Task<Search.Post.SearchResult> GetSearchResultFromUrlAsync(string url)
        {
            return GetPostSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url)));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Search.Post.Rating GetRating(char c)
        {
            switch (c)
            {
                case 's': return Search.Post.Rating.Safe;
                case 'q': return Search.Post.Rating.Questionable;
                case 'e': return Search.Post.Rating.Explicit;
                default: throw new ArgumentException("Invalid rating " + c);
            }
        }
    }
}
