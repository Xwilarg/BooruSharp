using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /// <summary>
        /// Get the comments posted on a post.
        /// </summary>
        /// <param name="postId">The ID of the post to get information about.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Comment.SearchResult[]> GetCommentsAsync(int postId)
        {
            if (!HasCommentAPI)
                throw new Search.FeatureUnavailable();

            var url = CreateUrl(_commentUrl, SearchArg("post_id") + postId);
            var results = await GetCommentSearchResultsAsync(url);

            return results.Where(result => result.PostID == postId).ToArray();
        }

        /// <summary>
        /// Get the last comments posted on the website.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="Search.FeatureUnavailable"/>
        /// <exception cref="System.Net.Http.HttpRequestException"/>
        public virtual async Task<Search.Comment.SearchResult[]> GetLastCommentsAsync()
        {
            if (!HasSearchLastComment)
                throw new Search.FeatureUnavailable();

            var url = CreateUrl(_commentUrl);
            var results = await GetCommentSearchResultsAsync(url);

            return results.ToArray();
        }

        private async Task<IEnumerable<Search.Comment.SearchResult>> GetCommentSearchResultsAsync(Uri url)
        {
            if (CommentsUseXml)
            {
                var xml = await GetXmlAsync(url);
                return xml.LastChild.OfType<XmlNode>().Select(GetCommentSearchResult);
            }
            else
            {
                var jsonArray = await GetJsonAsync<JArray>(url);
                return jsonArray.Select(GetCommentSearchResult);
            }
        }
    }
}
