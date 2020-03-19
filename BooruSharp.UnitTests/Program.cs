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
        private static async Task<string> CheckUrl(Uri url)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Add("User-Agent", "BooruSharp");
                    await hc.SendAsync(new HttpRequestMessage(new HttpMethod("HEAD"), url));
                }
                return null;
            }
            catch (WebException ex)
            { return ex.Message + " for " + url; }
        }

        public static async Task CheckResult(Search.Post.SearchResult result, string inputTag)
        {
            if (result.fileUrl != null)
            {
                string resFile = await CheckUrl(result.fileUrl);
                string resPreview = await CheckUrl(result.previewUrl);
                Assert.True(resFile == null, resFile);
                Assert.True(resPreview == null, resPreview);
                Assert.NotEqual(0, result.height);
                Assert.NotEqual(0, result.width);
                if (result.previewHeight != null)
                {
                    Assert.NotEqual(0, result.previewHeight);
                    Assert.NotEqual(0, result.previewWidth);
                }
            }
            Assert.InRange(result.rating, Search.Post.Rating.Safe, Search.Post.Rating.Explicit);
            Assert.Contains(inputTag, result.tags);
            Assert.NotEqual(0, result.id);
            if (result.size.HasValue)
                Assert.NotEqual(0, result.size.Value);
        }

        public static async Task CheckGetRandom(Booru.Booru booru, string s1)
        {
            Search.Post.SearchResult result = await booru.GetRandomImageAsync(s1);
            Search.Post.SearchResult result2 = await booru.GetRandomImageAsync(s1);
            Assert.NotEqual(result.id, result2.id);
            await CheckResult(result, s1);
        }

        public static async Task CheckTag(Booru.Booru booru, string s1 = "pantyhose")
        {
            Search.Tag.SearchResult result = await booru.GetTagAsync(s1);
            Assert.Equal(s1, result.name);
            Assert.InRange(result.type, Search.Tag.TagType.Trivia, Search.Tag.TagType.Metadata);
            Assert.NotEqual((Search.Tag.TagType)2, result.type);
            Assert.NotEqual(0, result.count);
        }

        public static void CheckWiki(Search.Wiki.SearchResult result)
        {
            Assert.InRange(result.lastUpdate, result.creation, DateTime.Now);
        }

        public static void CheckRelated(Search.Related.SearchResult[] result)
        {
            foreach (Search.Related.SearchResult res in result)
                Assert.NotEqual(0, res.count);
            Assert.NotEmpty(result);
        }

        public static void CheckComment(Search.Comment.SearchResult[] result)
        {
            foreach (Search.Comment.SearchResult res in result)
            {
                Assert.NotEqual(0, res.authorId);
                Assert.NotEqual(0, res.commentId);
                Assert.NotEqual(0, res.postId);
                Assert.NotEmpty(res.body);
            }
            Assert.NotEmpty(result);
        }
    }

    public class BooruTests
    {
        [Theory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru))]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru))]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        public async Task GetByMd5(Type t)
        {
            var booru = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            if (!booru.HavePostByMd5API())
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetImageByMd5Async("0"); });
            else
            {
                Search.Post.SearchResult result1;
                do
                {
                    result1 = await booru.GetRandomImageAsync();
                } while (result1.md5 == null);
                var result2 = await booru.GetImageByMd5Async(result1.md5);
                Assert.Equal(result1.id, result2.id);
            }
        }

        [Theory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru))]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection")]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        public async Task GetRandom(Type t, string tag = "school_swimsuit")
        {
            await General.CheckGetRandom((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null), tag);
        }

        [Theory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru))]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection")]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        public async Task CheckTag(Type t, string tag = "pantyhose")
        {
            await General.CheckTag((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null), tag);
        }

        [Theory]
        [InlineData(typeof(Atfbooru), "hibiki_(kantai_collection)", 2033)]
        [InlineData(typeof(DanbooruDonmai), "hibiki_(kantai_collection)", 1240738)]
        [InlineData(typeof(E621), "kantai_collection", 267881)]
        [InlineData(typeof(E926), "kantai_collection", 1329650)]
        [InlineData(typeof(Furrybooru), "kantai_collection", 151628)]
        [InlineData(typeof(Gelbooru), "hibiki_(kantai_collection)", 463392)]
        [InlineData(typeof(Konachan), "hibiki_(kancolle)", 75885)]
        [InlineData(typeof(Lolibooru), "hibiki_(kantai_collection)", 2939)]
        [InlineData(typeof(Realbooru), "kantai_collection", 688290)]
        [InlineData(typeof(Rule34), "hibiki_(kantai_collection)", 321239)]
        [InlineData(typeof(Safebooru), "hibiki_(kantai_collection)", 316679)]
        [InlineData(typeof(Sakugabooru), "kantai_collection", 7148)]
        [InlineData(typeof(SankakuComplex), "kantai_collection", -1)]
        [InlineData(typeof(Xbooru), "hibiki_(kantai_collection)", 151883)]
        [InlineData(typeof(Yandere), "hibiki_(kancolle)", 98153)]
        public async Task TagId(Type t, string tag, int tagId)
        {
            var booru = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            if (!booru.HaveTagByIdAPI())
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetTagAsync(tagId); });
            else
                Assert.Equal(tag, (await booru.GetTagAsync(tagId)).name);
        }

        [Theory]
        [InlineData(typeof(Atfbooru), "highres", 82)]
        [InlineData(typeof(DanbooruDonmai), "futanari", 3589)]
        [InlineData(typeof(E621), "futanari", 123)]
        [InlineData(typeof(E926), "futanari", 123)]
        [InlineData(typeof(Furrybooru), "futanari", -1)]
        [InlineData(typeof(Gelbooru), "futanari", -1)]
        [InlineData(typeof(Konachan), "futanari", -1)]
        [InlineData(typeof(Lolibooru), "futanari", 158)]
        [InlineData(typeof(Realbooru), "futanari", -1)]
        [InlineData(typeof(Rule34), "futanari", -1)]
        [InlineData(typeof(Safebooru), "futanari", -1)]
        [InlineData(typeof(Sakugabooru), "animated", 13)]
        [InlineData(typeof(SankakuComplex), "futanari", -1)]
        [InlineData(typeof(Xbooru), "futanari", -1)]
        [InlineData(typeof(Yandere), "futanari", 167)]
        public async Task CheckWiki(Type t, string tag, int? id)
        {
            var booru = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            if (!booru.HaveWikiAPI())
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetWikiAsync(tag); });
            else
            {
                Search.Wiki.SearchResult result = await booru.GetWikiAsync(tag);
                Assert.Equal(id, result.id);
                General.CheckWiki(result);
            }
        }

        public enum AvailableStatus
        {
            AuthRequired,
            Ok
        }

        [Theory]
        [InlineData(typeof(Atfbooru), "kantai_collection", AvailableStatus.Ok)]
        [InlineData(typeof(DanbooruDonmai), "kantai_collection", AvailableStatus.Ok)]
        [InlineData(typeof(E621), "sky", AvailableStatus.AuthRequired)]
        [InlineData(typeof(E926), "sky", AvailableStatus.AuthRequired)]
        [InlineData(typeof(Furrybooru), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Gelbooru), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Konachan), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Lolibooru), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Realbooru), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Rule34), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Safebooru), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Sakugabooru), "kantai_collection", AvailableStatus.Ok)]
        [InlineData(typeof(SankakuComplex), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Xbooru), "sky", AvailableStatus.Ok)]
        [InlineData(typeof(Yandere), "sky", AvailableStatus.Ok)]
        public async Task CheckRelated(Type t, string tag, AvailableStatus isAvailable) // TODO: Check if suppose to be alone
        {
            var booru = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            if (!booru.HaveRelatedAPI())
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetRelatedAsync(tag); });
            else if (isAvailable == AvailableStatus.AuthRequired)
                await Assert.ThrowsAsync<Search.AuthentificationRequired>(async delegate () { await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetRelatedAsync(tag); });
            else
            {
                Search.Related.SearchResult[] result = await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetRelatedAsync(tag);
                General.CheckRelated(result);
            }
        }

        [Fact]
        public async Task CheckRelatedE621()
        {
            Search.Related.SearchResult[] result = await new E621(
                new BooruAuth(Environment.GetEnvironmentVariable("E621_API_LOGIN"), Environment.GetEnvironmentVariable("E621_API_KEY"))).GetRelatedAsync("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task CheckRelatedE926()
        {
            Search.Related.SearchResult[] result = await new E926(
                new BooruAuth(Environment.GetEnvironmentVariable("E621_API_LOGIN"), Environment.GetEnvironmentVariable("E621_API_KEY"))).GetRelatedAsync("sky");
            General.CheckRelated(result);
        }

        [Theory]
        [InlineData(typeof(Atfbooru), 257639)]
        [InlineData(typeof(DanbooruDonmai), 3193008)]
        [InlineData(typeof(E621), 59432)]
        [InlineData(typeof(E926), 541858)]
        [InlineData(typeof(Furrybooru), 1282210)]
        [InlineData(typeof(Gelbooru), 3988284)]
        [InlineData(typeof(Konachan), 142938)]
        [InlineData(typeof(Lolibooru), 134097)]
        [InlineData(typeof(Realbooru), 646911)]
        [InlineData(typeof(Rule34), 2840746)]
        [InlineData(typeof(Safebooru), 132)]
        [InlineData(typeof(Sakugabooru), 38886)]
        [InlineData(typeof(SankakuComplex), -1)]
        [InlineData(typeof(Xbooru), 740157)]
        [InlineData(typeof(Yandere), 619494)]
        public async Task CheckComment(Type t, int id)
        {
            var booru = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            if (!booru.HaveCommentAPI())
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await booru.GetCommentsAsync(id); });
            else
                General.CheckComment(await booru.GetCommentsAsync(id));
        }

        [Theory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru))]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru))]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        public async Task CheckLastComment(Type t)
        {
            var booru = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            if (!booru.HaveCommentAPI() || !booru.HaveSearchLastComment())
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await booru.GetLastCommentsAsync(); });
            else
                General.CheckComment(await booru.GetLastCommentsAsync());
        }

        [Theory]
        [InlineData(typeof(Atfbooru), "female", true)]
        [InlineData(typeof(DanbooruDonmai), "hibi", true)]
        [InlineData(typeof(E621), "hibiki", true)]
        [InlineData(typeof(E926), "hibiki", true)]
        [InlineData(typeof(Furrybooru), "hibiki", true)]
        [InlineData(typeof(Gelbooru), "hibiki", true)]
        [InlineData(typeof(Konachan), "hibiki", false)]
        [InlineData(typeof(Lolibooru), "hibiki", false)]
        [InlineData(typeof(Realbooru), "female", true)]
        [InlineData(typeof(Rule34), "hibiki", true)]
        [InlineData(typeof(Safebooru), "hibiki", true)]
        [InlineData(typeof(Sakugabooru), "kantai", false)]
        [InlineData(typeof(SankakuComplex), "hibiki", false)]
        [InlineData(typeof(Xbooru), "hibiki", true)]
        [InlineData(typeof(Yandere), "hibiki", false)]
        public async Task CheckTags(Type t, string tag, bool onlyOnce)
        {
            if (onlyOnce)
                Assert.NotEmpty(await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetTagsAsync(tag));
            else
                Assert.NotInRange((await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).GetTagsAsync(tag)).Length, 0, 1);
        }

        [Theory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru))]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru))]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        public async Task CheckAvailable(Type t)
        {
            await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).CheckAvailabilityAsync();
        }

        /*[Theory]
        public async Task CheckNotAvailable(Type t)
        {
            await Assert.ThrowsAsync<HttpRequestException>(async () => await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).CheckAvailability());
        }*/

        [Theory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926), "breast")]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru))]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru), "breast")]
        [InlineData(typeof(Sakugabooru), "another")]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        public async Task CheckIsSafe(Type t, string explicitTag="pussy")
        {
            Booru.Booru b = (Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null);
            bool isSafe = b.IsSafe();
            bool foundExplicit = false;
            for (int i = 0; i < 10; i++)
            {
                var rating = (await b.GetRandomImageAsync(explicitTag)).rating;
                if (isSafe)
                    Assert.NotEqual(Search.Post.Rating.Explicit, rating);
                if (rating == Search.Post.Rating.Explicit)
                    foundExplicit = true;
            }
            if (!isSafe)
                Assert.True(foundExplicit);

        }
    }

    // TODO: Find a way to check if commands are available on website or not

    public class Other
    {
        [Fact]
        public async Task GelbooruTagCharacter()
        {
            Assert.Equal(Search.Tag.TagType.Character, (await new Gelbooru().GetTagAsync("cirno")).type);
        }

        [Fact]
        public async Task GelbooruTagCopyright()
        {
            Assert.Equal(Search.Tag.TagType.Copyright, (await new Gelbooru().GetTagAsync("kantai_collection")).type);
        }

        [Fact]
        public async Task GelbooruTagArtist()
        {
            Assert.Equal(Search.Tag.TagType.Artist, (await new Gelbooru().GetTagAsync("mtu_(orewamuzituda)")).type);
        }

        [Fact]
        public async Task GelbooruTagMetadata()
        {
            Assert.Equal(Search.Tag.TagType.Metadata, (await new Gelbooru().GetTagAsync("uncensored")).type);
        }

        [Fact]
        public async Task GelbooruTagTrivia()
        {
            Assert.Equal(Search.Tag.TagType.Trivia, (await new Gelbooru().GetTagAsync("futanari")).type);
        }
    }
}
