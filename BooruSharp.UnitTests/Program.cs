using BooruSharp.Booru;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    public static class General
    {
        public static async Task CheckCount(Booru.Booru booru)
        {
            int nbGeneral = await booru.GetNbImage();
            int nbMed = await booru.GetNbImage("wet");
            int nbMin = await booru.GetNbImage("wet", "swimsuit");
            Assert.NotEqual(0, nbMin);
            Assert.InRange(nbMed, nbMin, nbGeneral);
        }
        private static bool CheckUrl(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                try
                {
                    WebRequest request = WebRequest.Create(url);
                    request.Method = "HEAD";
                    request.GetResponse();
                    return (true);
                }
                catch (WebException)
                { }
            }
            return (false);
        }

        public static void CheckResult(Search.Post.SearchResult result, string inputTag)
        {
            Assert.True(CheckUrl(result.fileUrl));
            Assert.True(CheckUrl(result.previewUrl));
            Assert.InRange(result.rating, Search.Post.Rating.Safe, Search.Post.Rating.Explicit);
            Assert.Contains(inputTag, result.tags);
            Assert.NotEqual<uint>(0, result.id);
        }

        public static async Task CheckGetByOffset(Booru.Booru booru)
        {
            Search.Post.SearchResult result = await booru.GetImage(2, "school_swimsuit");
            CheckResult(result, "school_swimsuit");
        }

        public static async Task CheckGetRandom(Booru.Booru booru)
        {
            Search.Post.SearchResult result = await booru.GetRandomImage("school_swimsuit");
            CheckResult(result, "school_swimsuit");
        }

        public static async Task CheckTag(Booru.Booru booru)
        {
            Search.Tag.SearchResult result = await booru.GetTag("pantyhose");
            Assert.Equal("pantyhose", result.name);
            Assert.InRange(result.type, Search.Tag.TagType.Trivia, Search.Tag.TagType.Metadata);
            Assert.NotEqual((Search.Tag.TagType)2, result.type);
            Assert.NotEqual<uint>(0, result.count);
        }

        public static void CheckWiki(Search.Wiki.SearchResult result)
        {
            Assert.InRange(result.lastUpdate, result.creation, DateTime.Now);
        }

        public static void CheckRelated(Search.Related.SearchResult[] result)
        {
            foreach (Search.Related.SearchResult res in result)
                Assert.NotEqual<uint>(0, res.count);
            Assert.NotEmpty(result);
        }
        public static void CheckComment(Search.Comment.SearchResult[] result)
        {
            foreach (Search.Comment.SearchResult res in result)
            {
                Assert.NotEqual<uint>(0, res.authorId);
                Assert.NotEqual<uint>(0, res.commentId);
                Assert.NotEqual<uint>(0, res.postId);
                Assert.NotEmpty(res.body);
                Assert.NotEmpty(res.authorName);
            }
            Assert.NotEmpty(result);
        }
    }

    public class UnitGelbooru
    {
        [Fact]
        public async Task GelbooruCount()
        {
            await General.CheckCount(new Gelbooru());
        }

        [Fact]
        public async Task GelbooruGetByOffset()
        {
            await General.CheckGetByOffset(new Gelbooru());
        }

        [Fact]
        public async Task GelbooruGetRandom()
        {
            await General.CheckGetRandom(new Gelbooru());
        }

        [Fact]
        public async Task GelbooruCheckTag()
        {
            await General.CheckTag(new Gelbooru());
        }

        [Fact]
        public async Task GelbooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", (await new Gelbooru().GetTag(463392)).name);
        }

        [Fact]
        public async Task GelbooruCheckWiki()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Gelbooru().GetWiki("futanari"); });
        }

        [Fact]
        public async Task GelbooruCheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Gelbooru().GetRelated("sky"); });
        }

        [Fact]
        public async Task GelbooruCheckComment()
        {
            General.CheckComment(await new Gelbooru().GetComment(3988284));
        }
    }

    public class UnitSafebooru
    {
        [Fact]
        public async Task SafebooruCount()
        {
            await General.CheckCount(new Safebooru());
        }

        [Fact]
        public async Task SafebooruGetByOffset()
        {
            await General.CheckGetByOffset(new Safebooru());
        }

        [Fact]
        public async Task SafebooruGetRandom()
        {
            await General.CheckGetRandom(new Safebooru());
        }

        [Fact]
        public async Task SafebooruCheckTag()
        {
            await General.CheckTag(new Safebooru());
        }

        [Fact]
        public async Task SafebooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", (await new Safebooru().GetTag(316679)).name);
        }

        [Fact]
        public async Task SafebooruCheckWiki()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Safebooru().GetWiki("futanari"); });
        }

        [Fact]
        public async Task SafebooruCheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Safebooru().GetRelated("sky"); });
        }

        [Fact]
        public async Task SafebooruCheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Safebooru().GetComment(132); });
        }
    }

    public class UnitKonachan
    {
        [Fact]
        public async Task KonachanCount()
        {
            await General.CheckCount(new Konachan());
        }

        [Fact]
        public async Task KonachanGetByOffset()
        {
            await General.CheckGetByOffset(new Konachan());
        }

        [Fact]
        public async Task KonachanGetRandom()
        {
            await General.CheckGetRandom(new Konachan());
        }

        [Fact]
        public async Task KonachanCheckTag()
        {
            await General.CheckTag(new Konachan());
        }

        [Fact]
        public async Task KonachanTagId()
        {
            Assert.Equal("hibiki_(kancolle)", (await new Konachan().GetTag(75885)).name);
        }

        [Fact]
        public async Task KonachanCheckWiki()
        {
            Search.Wiki.SearchResult result = await new Konachan().GetWiki("futanari");
            Assert.Equal<uint>(757, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task KonachanCheckRelated()
        {
            Search.Related.SearchResult[] result = await new Konachan().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task KonachanCheckComment()
        {
            General.CheckComment(await new Konachan().GetComment(142938));
        }
    }

    public class UnitE621
    {
        [Fact]
        public async Task E621Count()
        {
            await General.CheckCount(new E621());
        }

        [Fact]
        public async Task E621GetByOffset()
        {
            await General.CheckGetByOffset(new E621());
        }

        [Fact]
        public async Task E621GetRandom()
        {
            await General.CheckGetRandom(new E621());
        }

        [Fact]
        public async Task E621CheckTag()
        {
            await General.CheckTag(new E621());
        }

        [Fact]
        public async Task E621TagId()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new E621().GetTag(267881); });
        }

        [Fact]
        public async Task E621CheckWiki()
        {
            Search.Wiki.SearchResult result = await new E621().GetWiki("futanari");
            Assert.Equal<uint>(123, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task E621CheckRelated()
        {
            Search.Related.SearchResult[] result = await new E621().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task E621CheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new E621().GetComment(59432); });
        }
    }

    public class UnitRule34
    {
        [Fact]
        public async Task Rule34Count()
        {
            await General.CheckCount(new Rule34());
        }

        [Fact]
        public async Task Rule34GetByOffset()
        {
            await General.CheckGetByOffset(new Rule34());
        }

        [Fact]
        public async Task Rule34GetRandom()
        {
            await General.CheckGetRandom(new Rule34());
        }

        [Fact]
        public async Task Rule34CheckTag()
        {
            await General.CheckTag(new Rule34());
        }

        [Fact]
        public async Task Rule34TagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", (await new Rule34().GetTag(321239)).name);
        }

        [Fact]
        public async Task Rule34CheckWiki()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Rule34().GetWiki("futanari"); });
        }

        [Fact]
        public async Task Rule34CheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Rule34().GetRelated("sky"); });
        }

        [Fact]
        public async Task Rule34CheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Rule34().GetComment(1556058); });
        }
    }

    public class UnitLolibooru
    {
        [Fact]
        public async Task LolibooruCount()
        {
            await General.CheckCount(new Lolibooru());
        }

        [Fact]
        public async Task LolibooruGetByOffset()
        {
            await General.CheckGetByOffset(new Lolibooru());
        }

        [Fact]
        public async Task LolibooruGetRandom()
        {
            await General.CheckGetRandom(new Lolibooru());
        }

        [Fact]
        public async Task LolibooruCheckTag()
        {
            await General.CheckTag(new Lolibooru());
        }

        [Fact]
        public async Task LolibooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", (await new Lolibooru().GetTag(2939)).name);
        }

        [Fact]
        public async Task LolibooruCheckWiki()
        {
            Search.Wiki.SearchResult result = await new Lolibooru().GetWiki("futanari");
            Assert.Equal<uint>(158, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task LolibooruCheckRelated()
        {
            Search.Related.SearchResult[] result = await new Lolibooru().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task LolibooruCheckComment()
        {
            General.CheckComment(await new Lolibooru().GetComment(134097));
        }
    }

    public class UnitYandere
    {
        [Fact]
        public async Task YandereCount()
        {
            await General.CheckCount(new Yandere());
        }

        [Fact]
        public async Task YandereGetByOffset()
        {
            await General.CheckGetByOffset(new Yandere());
        }

        [Fact]
        public async Task YandereGetRandom()
        {
            await General.CheckGetRandom(new Yandere());
        }

        [Fact]
        public async Task YandereCheckTag()
        {
            await General.CheckTag(new Yandere());
        }

        [Fact]
        public async Task YandereTagId()
        {
            Assert.Equal("hibiki_(kancolle)", (await new Yandere().GetTag(98153)).name);
        }

        [Fact]
        public async Task YandereCheckWiki()
        {
            Search.Wiki.SearchResult result = await new Yandere().GetWiki("futanari");
            Assert.Equal<uint>(167, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task YandereCheckRelated()
        {
            Search.Related.SearchResult[] result = await new Yandere().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task YandereCheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Yandere().GetComment(405923); });
        }
    }

    public class UnitE926
    {
        [Fact]
        public async Task E926Count()
        {
            await General.CheckCount(new E926());
        }

        [Fact]
        public async Task E926GetByOffset()
        {
            await General.CheckGetByOffset(new E926());
        }

        [Fact]
        public async Task E926GetRandom()
        {
            await General.CheckGetRandom(new E926());
        }

        [Fact]
        public async Task E926CheckTag()
        {
            await General.CheckTag(new E926());
        }

        [Fact]
        public async Task E926TagId()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new E926().GetTag(1329650); });
        }

        [Fact]
        public async Task E926CheckWiki()
        {
            Search.Wiki.SearchResult result = await new E926().GetWiki("futanari");
            Assert.Equal<uint>(123, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task E926CheckRelated()
        {
            Search.Related.SearchResult[] result = await new E926().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task E926CheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new E926().GetComment(541858); });
        }
    }

    public class Other
    {
        [Fact]
        public void CustomBooru()
        {
            new Booru.Custom.CustomBooru("furry.booru.org", UrlFormat.indexPhp, null, Booru.BooruOptions.useHttp);
        }

        [Fact]
        public void CustomBooruNotUrl()
        {
            Assert.Throws<Booru.Custom.InvalidBooru>(delegate() { new Booru.Custom.CustomBooru("AWAWAWAWAWA", UrlFormat.indexPhp); });
        }

        [Fact]
        public void CustomBooruBadUrl()
        {
            Assert.Throws<Booru.Custom.InvalidBooru>(delegate () { new Booru.Custom.CustomBooru("https://zirk.eu", UrlFormat.indexPhp); });
        }

        [Fact]
        public async Task GelbooruTagCharacter()
        {
            Assert.Equal(Search.Tag.TagType.Character, (await new Gelbooru().GetTag("cirno")).type);
        }

        [Fact]
        public async Task GelbooruTagCopyright()
        {
            Assert.Equal(Search.Tag.TagType.Copyright, (await new Gelbooru().GetTag("kantai_collection")).type);
        }

        [Fact]
        public async Task GelbooruTagArtist()
        {
            Assert.Equal(Search.Tag.TagType.Artist, (await new Gelbooru().GetTag("mtu_(orewamuzituda)")).type);
        }

        [Fact]
        public async Task GelbooruTagMetadata()
        {
            Assert.Equal(Search.Tag.TagType.Metadata, (await new Gelbooru().GetTag("uncensored")).type);
        }

        [Fact]
        public async Task GelbooruTagTrivia()
        {
            Assert.Equal(Search.Tag.TagType.Trivia, (await new Gelbooru().GetTag("futanari")).type);
        }
    }
}
