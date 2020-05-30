using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Search for a post using their MD5
        /// </summary>
        /// <param name="md5">The MD5 of the post to search</param>
        public async Task<Search.Post.SearchResult> GetImageByMd5Async(string md5)
        {
            if (!HavePostByMd5API())
                throw new Search.FeatureUnavailable();
            return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", "md5=" + md5));
        }

        /// <summary>
        /// Search for a random post
        /// </summary>
        /// <param name="tagsArg">Tags that must be contained in the post (optional)</param>
        public async Task<Search.Post.SearchResult> GetRandomImageAsync(params string[] tagsArg)
        {
            tagsArg = tagsArg.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (tagsArg.Length > 2 && noMoreThan2Tags)
                throw new Search.TooManyTags();
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
                    if (max == 0)
                        throw new Search.InvalidTags();
                    if (maxLimit && max > 20001)
                        max = 20001;
                    return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg), "pid=" + random.Next(0, max)));
                }
            }
            else if (noMoreThan2Tags)
                return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg), "random=true")); // +order:random count as a tag so we use random=true instead to save one
            else
                return await GetSearchResultFromUrlAsync(CreateUrl(imageUrl, "limit=1", TagsToString(tagsArg) + "+order:random"));
        }

        /// <summary>
        /// Get the latest posts of the website
        /// </summary>
        /// <param name="tagsArg">Tags that must be contained in the post (optional)</param>
        public async Task<Search.Post.SearchResult[]> GetLastImagesAsync(params string[] tagsArg)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(imageUrl, TagsToString(tagsArg)))));
        }

        /// <summary>
        /// Get all the posts on a page
        /// </summary>
        /// <param name="page">The page to retrieve posts from</param>
        public async Task<Search.Post.SearchResult[]> GetImagesFromPageAsync(uint page, params string[] tagsArg)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(imageUrl, "page=" + page, TagsToString(tagsArg)))));
        }

        private async Task<Search.Post.SearchResult> GetSearchResultFromUrlAsync(string url)
        {
            return GetPostSearchResult(ParseFirstPostSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url))));
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
