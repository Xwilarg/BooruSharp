using BooruSharp.Booru;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    public static class General
    {
        private const int _randomPostIterationCount = 5;
        private const int _randomPostCount = 5;
        private const Search.Tag.TagType _unknownTagType = (Search.Tag.TagType)2;

        private static async Task<string> CheckUrlAsync(Uri url)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Add("User-Agent", "BooruSharp");
                    await hc.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                }
                return null;
            }
            catch (WebException ex)
            {
                return ex.Message + " for " + url;
            }
        }

        public static async Task CheckResultAsync(Search.Post.SearchResult result, string inputTag)
        {
            if (!(result.FileUrl is null))
            {
                string resFile = await CheckUrlAsync(result.FileUrl);
                string resPreview = await CheckUrlAsync(result.PreviewUrl);
                string resPost = await CheckUrlAsync(result.PostUrl);
                Assert.True(resPost is null, resPost);
                Assert.True(resFile is null, resFile);
                Assert.True(resPreview is null, resPreview);
                Assert.NotEqual(0, result.Height);
                Assert.NotEqual(0, result.Width);
                if (!(result.PreviewHeight is null))
                {
                    Assert.NotEqual(0, result.PreviewHeight);
                    Assert.NotEqual(0, result.PreviewWidth);
                }
            }
            Assert.InRange(result.Rating, Search.Post.Rating.Safe, Search.Post.Rating.Explicit);
            Assert.Contains(result.Tags, t => t.Contains(inputTag));
            Assert.NotEqual(0, result.ID);
            if (result.Size.HasValue)
                Assert.NotEqual(0, result.Size.Value);
        }

        public static async Task CheckGetRandomAsync(ABooru booru, string s1)
        {
            Search.Post.SearchResult result = await booru.GetRandomPostAsync(s1);
            Search.Post.SearchResult result2;
            int i = 0;
            do
            {
                result2 = await booru.GetRandomPostAsync(s1);
                i++;
            } while (result.ID == result2.ID && i < _randomPostIterationCount);
            Assert.NotEqual(result.ID, result2.ID);
            await CheckResultAsync(result, s1);
        }

        public static async Task CheckGetRandomsAsync(ABooru booru, string s1)
        {
            Search.Post.SearchResult[] result = await booru.GetRandomPostsAsync(_randomPostCount, s1);
            Assert.NotEmpty(result);
            Search.Post.SearchResult[] result2;
            int i = 0;
            do
            {
                result2 = await booru.GetRandomPostsAsync(_randomPostCount, s1);
                Assert.NotEmpty(result2);
                i++;
            } while (result[0].ID == result2[0].ID && i < _randomPostIterationCount);
            Assert.NotEqual(result[0].ID, result2[0].ID);
            await CheckResultAsync(result[0], s1);
        }

        public static async Task CheckTagAsync(ABooru booru, string s1 = "pantyhose")
        {
            Search.Tag.SearchResult result = await booru.GetTagAsync(s1);
            Assert.Equal(s1, result.Name);
            Assert.InRange(result.Type, Search.Tag.TagType.Trivia, Search.Tag.TagType.Metadata);
            Assert.NotEqual(_unknownTagType, result.Type);
            Assert.NotEqual(0, result.Count);
        }

        public static void CheckWiki(Search.Wiki.SearchResult result)
        {
            Assert.InRange(result.LastUpdate, result.Creation, DateTime.Now);
        }

        public static void CheckRelated(Search.Related.SearchResult[] result)
        {
            Assert.NotEmpty(result);
        }

        public static void CheckComment(Search.Comment.SearchResult[] result)
        {
            foreach (Search.Comment.SearchResult res in result)
            {
                Assert.NotEqual(0, res.AuthorID);
                Assert.NotEqual(0, res.CommentID);
                Assert.NotEqual(0, res.PostID);
                Assert.NotEmpty(res.Body);
            }
            Assert.NotEmpty(result);
        }

        public static bool CompareArray(Search.Post.SearchResult[] res1, Search.Post.SearchResult[] res2)
        {
            if (res1.Length != res2.Length)
                return false;
            for (int i = 0; i < res1.Length; i++)
                if (res1[i].ID != res2[i].ID)
                    return false;
            return true;
        }

        public static async Task<Search.Post.SearchResult> GetRandomPostAsync(ABooru booru)
        {
            if (booru.NoEmptyPostSearch)
                return await booru.GetRandomPostAsync("スク水"); // Pixiv doesn't handle random search with no tag
            return await booru.GetRandomPostAsync();
        }

        public static void Authorize(ABooru booru)
        {
            string booruName = booru.GetType().Name.ToUpperInvariant();
            string userID = Environment.GetEnvironmentVariable(booruName + "_USER_ID");
            string passwordHash = Environment.GetEnvironmentVariable(booruName + "_PASSWORD_HASH");

            Skip.If(
                userID is null || passwordHash is null,
                $"{booruName}_* environment variables aren't set.");

            booru.Auth = new BooruAuth(userID, passwordHash);
        }
    }
}
