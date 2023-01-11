using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Converts a letter to its matching <see cref="Search.Post.Rating"/>.
        /// </summary>
        private protected static Rating GetRating(char c)
        {
            return char.ToLower(c) switch
            {
                'g' => Rating.General,
                's' => Rating.Safe,
                'q' => Rating.Questionable,
                'e' => Rating.Explicit,
                _ => throw new ArgumentException($"Invalid rating '{c}'.", nameof(c)),
            };
        }

        /// <inheritdoc/>
        public async Task<PostSearchResult> GetRandomPostAsync(params string[] tagsArg)
        {
            string[] tags = tagsArg != null
                ? tagsArg.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToArray()
                : Array.Empty<string>();

            if (!tags.Any() && !CanSearchWithNoTag)
            {
                throw new FeatureUnavailable("This booru doesn't support search with no tag");
            }
            if (MaxNumberOfTags != -1 && tags.Length > MaxNumberOfTags)
            {
                throw new TooManyTags(MaxNumberOfTags);
            }

            var url = await CreateRandomPostUriAsync(tags);
            return await GetPostSearchResultAsync(url);
        }
        /*
        private const int _limitedTagsSearchCount = 2;
        private const int _increasedPostLimitCount = 20001;

        private string GetLimit(int quantity)
            => (_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails ? "per_page=" : "limit=") + quantity;

        /// <summary>
        /// Searches for a post using its MD5 hash.
        /// </summary>
        /// <param name="md5">The MD5 hash of the post to search.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Post.SearchResult> GetPostByMd5Async(string md5)
        {
            if (!HasPostByMd5API)
                throw new Search.FeatureUnavailable();

            if (md5 == null)
                throw new ArgumentNullException(nameof(md5));

            return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, GetLimit(1), "md5=" + md5));
        }

        /// <summary>
        /// Searches for a post using its ID.
        /// </summary>
        /// <param name="id">The ID of the post to search.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Post.SearchResult> GetPostByIdAsync(int id)
        {
            if (!HasPostByIdAPI)
                throw new Search.FeatureUnavailable();

            if (_format == UrlFormat.Danbooru) return await GetSearchResultFromUrlAsync(BaseUrl + "posts/" + id + ".json");
            if (_format == UrlFormat.Philomena) return await GetSearchResultFromUrlAsync($"{BaseUrl}api/v1/json/images/{id}");
            if (_format == UrlFormat.BooruOnRails) return await GetSearchResultFromUrlAsync($"{BaseUrl}api/v3/posts/{id}");
            if (_format == UrlFormat.PostIndexJson) return await GetSearchResultFromUrlAsync(_imageUrl + "?tags=id:" + id);
            return await GetSearchResultFromUrlAsync(CreateUrl(_imageUrl, GetLimit(1), "id=" + id));
        }

        /// <summary>
        /// Gets the total number of available posts. If <paramref name="tagsArg"/> array is specified
        /// and isn't empty, the total number of posts containing these tags will be returned.
        /// </summary>
        /// <param name="tagsArg">The optional array of tags.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="Search.TooManyTags"/>
        public virtual async Task<int> GetPostCountAsync(params string[] tagsArg)
        {
            if (!HasPostCountAPI)
                throw new Search.FeatureUnavailable();

            string[] tags = tagsArg != null
                ? tagsArg.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToArray()
                : Array.Empty<string>();

            if (NoMoreThanTwoTags && tags.Length > _limitedTagsSearchCount)
                throw new Search.TooManyTags();

            if (_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails)
            {
                var url = CreateUrl(_imageUrl, GetLimit(1), TagsToString(tags));
                var json = await GetJsonAsync(url);
                var token = (JToken)JsonConvert.DeserializeObject(json);
                return token["total"].Value<int>();
            }
            else
            {
                var url = CreateUrl(_imageUrlXml, GetLimit(1), TagsToString(tags));
                XmlDocument xml = await GetXmlAsync(url);
                return int.Parse(xml.ChildNodes.Item(1).Attributes[0].InnerXml);
            }
        }

        /// <summary>
        /// Searches for multiple random posts. If <paramref name="tagsArg"/> array is
        /// specified and isn't empty, random posts containing those tags will be returned.
        /// </summary>
        /// <param name="limit">The number of posts to get.</param>
        /// <param name="tagsArg">The optional array of tags that must be contained in the posts.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        /// <exception cref="Search.TooManyTags"/>
        public virtual async Task<Search.Post.SearchResult[]> GetRandomPostsAsync(int limit, params string[] tagsArg)
        {
            if (!HasMultipleRandomAPI)
                throw new Search.FeatureUnavailable();

            string[] tags = tagsArg != null
                ? tagsArg.Where(tag => !string.IsNullOrWhiteSpace(tag)).ToArray()
                : Array.Empty<string>();

            if (NoMoreThanTwoTags && tags.Length > _limitedTagsSearchCount)
                throw new Search.TooManyTags();

            string tagString = TagsToString(tags);

            if (_format == UrlFormat.IndexPhp)
                return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, GetLimit(limit), tagString) + "+sort:random");
            if (_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails)
                return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, GetLimit(limit), tagString, "sf=random"));
            else if (NoMoreThanTwoTags)
                // +order:random count as a tag so we use random=true instead to save one
                return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, GetLimit(limit), tagString, "random=true"));
            else
                return await GetSearchResultsFromUrlAsync(CreateUrl(_imageUrl, GetLimit(limit), tagString) + "+order:random");
        }

        /// <summary>
        /// Gets the latest posts on the website. If <paramref name="tagsArg"/> array is
        /// specified and isn't empty, latest posts containing those tags will be returned.
        /// </summary>
        /// <param name="tagsArg">The optional array of tags that must be contained in the posts.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Post.SearchResult[]> GetLastPostsAsync(params string[] tagsArg)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(_imageUrl, TagsToString(tagsArg)))));
        }



        /// <summary>
        /// Gets the latest posts on the website. If <paramref name="tagsArg"/> array is
        /// specified and isn't empty, latest posts containing those tags will be returned.
        /// </summary>
        /// <param name="limit">The number of posts to get.</param>
        /// <param name="tagsArg">The optional array of tags that must be contained in the posts.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Post.SearchResult[]> GetLastPostsAsync(int limit, params string[] tagsArg)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(CreateUrl(_imageUrl, "limit=" + limit, TagsToString(tagsArg)))));
        }

        private async Task<Search.Post.SearchResult> GetSearchResultFromUrlAsync(string url)
        {
            return GetPostSearchResult(ParseFirstPostSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url))));
        }

        private Task<Search.Post.SearchResult> GetSearchResultFromUrlAsync(Uri url)
        {
            return GetSearchResultFromUrlAsync(url.AbsoluteUri);
        }

        private async Task<Search.Post.SearchResult[]> GetSearchResultsFromUrlAsync(string url)
        {
            return GetPostsSearchResult(JsonConvert.DeserializeObject(await GetJsonAsync(url)));
        }

        private Task<Search.Post.SearchResult[]> GetSearchResultsFromUrlAsync(Uri url)
        {
            return GetSearchResultsFromUrlAsync(url.AbsoluteUri);
        }
        */
    }
}
