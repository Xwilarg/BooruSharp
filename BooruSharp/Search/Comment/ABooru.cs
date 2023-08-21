using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooruSharp.Booru
{
    public abstract partial class ABooru
    {
        /*
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

            if (_format == UrlFormat.Philomena || _format == UrlFormat.BooruOnRails)
            {
                Uri url;
                if (_format == UrlFormat.Philomena)
                {
                    url = CreateUrl(_commentUrl, SearchArg("q") + ":" + postId);
                    return ((JArray)JsonConvert.DeserializeObject<JToken>(await GetJsonAsync(url))["comments"]).Select(GetCommentSearchResult).ToArray();
                }
                else
                {
                    url = new Uri($"{BaseUrl}api/v3/posts/{postId}/comments");
                    // Booru on rails doesn't return post id in JSON response
                    return ((JArray)JsonConvert.DeserializeObject<JToken>(await GetJsonAsync(url))["comments"]).Select(GetCommentSearchResult).Select(x =>
                    {
                        return new Search.Comment.SearchResult(x.CommentID, postId, x.AuthorID, x.Creation, x.AuthorName, x.Body);
                    }).ToArray();
                }
            }
            else
            {
                var url = CreateUrl(_commentUrl, SearchArg("post_id") + postId);

                var results = new List<Search.Comment.SearchResult>();

                if (CommentsUseXml)
                {
                    var xml = await GetXmlAsync(url);

                    foreach (var node in xml.LastChild)
                    {
                        var result = GetCommentSearchResult(node);

                        if (result.PostID == postId)
                            results.Add(result);
                    }
                }
                else
                {
                    var jsonArray = JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));

                    foreach (var json in jsonArray)
                    {
                        var result = GetCommentSearchResult(json);

                        if (result.PostID == postId)
                            results.Add(result);
                    }
                }

                return results.ToArray();
            }
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

            if (CommentsUseXml)
            {
                var xml = await GetXmlAsync(url);
                var results = new List<Search.Comment.SearchResult>(xml.LastChild.ChildNodes.Count);

                foreach (var node in xml.LastChild)
                    results.Add(GetCommentSearchResult(node));

                return results.ToArray();
            }
            else
            {
                var jsonArray = JsonConvert.DeserializeObject<JArray>(await GetJsonAsync(url));
                return jsonArray.Select(GetCommentSearchResult).ToArray();
            }
        }
        */
    }
}
