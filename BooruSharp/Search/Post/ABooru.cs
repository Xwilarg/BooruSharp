using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Search for a post using its MD5
        /// </summary>
        /// <param name="md5">The MD5 of the post to search</param>
        public virtual async Task<Search.Post.SearchResult> GetPostByMd5Async(string md5)
        {
            if (!HasPostByMd5API())
                throw new Search.FeatureUnavailable();
            if (md5 == null)
                throw new ArgumentNullException("Argument can't be null");
            return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", "md5=" + md5));
        }

        /// <summary>
        /// Search for a post using its ID
        /// </summary>
        /// <param name="id">The ID fo the post to search</param>
        /// <returns></returns>
        public virtual async Task<Search.Post.SearchResult> GetPostByIdAsync(int id)
        {
            if (!HasPostByIdAPI())
                throw new Search.FeatureUnavailable();
            if (_format == UrlFormat.danbooru)
                return await GetSearchResultFromUrlAsync(_baseUrl + "/posts/" + id + ".json");
            return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", "id=" + id));
        }

        /// <summary>
        /// Get the number of posts available
        /// </summary>
        /// <param name="tagsArg">Tags for which you want the number of posts about (optional)</param>
        public virtual async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (!HasPostCountAPI())
                throw new Search.FeatureUnavailable();
            if (tagsArg == null) tagsArg = new string[0];
            else tagsArg = tagsArg.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (tagsArg.Length > 2 && NoMoreThanTwoTags())
                throw new Search.TooManyTags();
            XmlDocument xml = await GetXmlAsync(CreateUrl(_imageUrlXml, "limit=1", TagsToString(tagsArg)));
            return int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);
        }

        /// <summary>
        /// Search for a random post
        /// </summary>
        /// <param name="tagsArg">Tags that must be contained in the post (optional)</param>
        public virtual async Task<Search.Post.SearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            if (tagsArg == null) tagsArg = new string[0];
            else tagsArg = tagsArg.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (tagsArg.Length > 2 && NoMoreThanTwoTags())
                throw new Search.TooManyTags();
            if (_format == UrlFormat.indexPhp)
            {
                if (this is Template.Gelbooru)
                    return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", TagsToString(tagsArg)) + "+sort:random");
                if (tagsArg.Length == 0)
                    return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", "id=" + await GetRandomIdAsync(TagsToString(tagsArg)))); // We need to request /index.php?page=post&s=random and get the id given by the redirect
                // The previous option doesn't work if there are tags so we contact the XML endpoint to get post count
                string url = CreateUrl(_imageUrlXml, "limit=1", TagsToString(tagsArg));
                XmlDocument xml = await GetXmlAsync(url);
                int max = int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);
                if (max == 0)
                    throw new Search.InvalidTags();
                if (SearchIncreasedPostLimit() && max > 20001)
                    max = 20001;
                return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", TagsToString(tagsArg), "pid=" + _random.Next(0, max)));
            }
            if (NoMoreThanTwoTags())
                return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", TagsToString(tagsArg), "random=true")); // +order:random count as a tag so we use random=true instead to save one
            return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, "limit=1", TagsToString(tagsArg)) + "+order:random");
        }

        /// <summary>
        /// Search for random posts
        /// </summary>
        /// <param name="limit">Number of posts you want to get</param>
        /// <param name="tagsArg">Tags that must be contained in the post (optional)</param>
        public virtual async Task<Search.Post.SearchResult[]> GetRandomPostsAsync(int limit, params string[] tagsArg)
        {
            if (!HasMultipleRandomAPI())
                throw new Search.FeatureUnavailable();
            if (tagsArg == null) tagsArg = new string[0];
            else tagsArg = tagsArg.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (tagsArg.Length > 2 && NoMoreThanTwoTags())
                throw new Search.TooManyTags();
            if (_format == UrlFormat.indexPhp)
                return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, "limit=" + limit, TagsToString(tagsArg)) + "+sort:random");
            if (NoMoreThanTwoTags())
                return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, "limit=" + limit, TagsToString(tagsArg), "random=true")); // +order:random count as a tag so we use random=true instead to save one
            return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, "limit=" + limit, TagsToString(tagsArg)) + "+order:random");
        }

        /// <summary>
        /// Get the latest posts of the website
        /// </summary>
        /// <param name="tagsArg">Tags that must be contained in the post (optional)</param>
        public virtual async Task<Search.Post.SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(_imageUrl, TagsToString(tagsArg)))));
        }

        private async Task<Search.Post.SearchResult> GetSearchResultFromUrlAsync(string url)
        {
            return GetPostSearchResult(ParseFirstPostSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url))));
        }

        private async Task<Search.Post.SearchResult[]> GetSearchResultsFromUrlAsync(string url)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url)));
        }

        /// <summary>
        /// Convert letter to its maching Search.Post.Rating
        /// </summary>
        protected Search.Post.Rating GetRating(char c)
        {
            switch (c)
            {
                case 's': case 'S': return Search.Post.Rating.Safe;
                case 'q': case 'Q': return Search.Post.Rating.Questionable;
                case 'e': case 'E': return Search.Post.Rating.Explicit;
                default: throw new ArgumentException("Invalid rating " + c);
            }
        }
    }
}
