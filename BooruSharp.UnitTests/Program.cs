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
        public static async Task CheckCount(Booru.Booru booru, string s1 = "wet", string s2 = "swimsuit")
        {
            int nbGeneral = await booru.GetNbImage();
            int nbMed = await booru.GetNbImage(s1);
            int nbMin = await booru.GetNbImage(s1, s2);
            Assert.NotEqual(0, nbMin);
            Assert.InRange(nbMed, nbMin, nbGeneral);
        }
        private static async Task<string> CheckUrl(Uri url)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Add("User-Agent", "BooruSharp");
                    await hc.SendAsync(new HttpRequestMessage(new HttpMethod("HEAD"), url));
                }
                return (null);
            }
            catch (WebException ex)
            { return (ex.Message + " for " + url); }
        }

        public static async Task CheckResult(Search.Post.SearchResult result, string inputTag)
        {
            string resFile = await CheckUrl(result.fileUrl);
            string resPreview = await CheckUrl(result.previewUrl);
            Assert.True(resFile == null, resFile);
            Assert.True(resPreview == null, resPreview);
            Assert.InRange(result.rating, Search.Post.Rating.Safe, Search.Post.Rating.Explicit);
            Assert.Contains(inputTag, result.tags);
            Assert.NotEqual(0, result.id);
            if (result.size.HasValue)
                Assert.NotEqual(0, result.size.Value);
            Assert.NotEqual(0, result.height);
            Assert.NotEqual(0, result.width);
            Assert.NotEqual(0, result.previewHeight);
            Assert.NotEqual(0, result.previewWidth);
        }

        public static async Task CheckGetByOffset(Booru.Booru booru, string s1 = "school_swimsuit")
        {
            Search.Post.SearchResult result = await booru.GetImage(2, s1);
            await CheckResult(result, s1);
        }

        public static async Task CheckGetRandom(Booru.Booru booru, string s1 = "school_swimsuit")
        {
            Search.Post.SearchResult result = await booru.GetRandomImage(s1);
            await CheckResult(result, s1);
        }

        public static async Task CheckTag(Booru.Booru booru, string s1 = "pantyhose")
        {
            Search.Tag.SearchResult result = await booru.GetTag(s1);
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
            Assert.Equal(757, result.id);
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
            Assert.Equal(123, result.id);
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
            General.CheckComment(await new E621().GetComment(59432));
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
            General.CheckComment(await new Rule34().GetComment(2840746));
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
            Assert.Equal(158, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task LolibooruCheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Lolibooru().GetRelated("sky"); });
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
            Assert.Equal(167, result.id);
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
            Assert.Equal(123, result.id);
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
            General.CheckComment(await new E926().GetComment(541858));
        }
    }

    public class UnitXbooru
    {
        [Fact]
        public async Task XbooruCount()
        {
            await General.CheckCount(new Xbooru());
        }

        [Fact]
        public async Task XbooruGetByOffset()
        {
            await General.CheckGetByOffset(new Xbooru());
        }

        [Fact]
        public async Task XbooruGetRandom()
        {
            await General.CheckGetRandom(new Xbooru());
        }

        [Fact]
        public async Task XbooruCheckTag()
        {
            await General.CheckTag(new Xbooru());
        }

        [Fact]
        public async Task XbooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", (await new Xbooru().GetTag(151883)).name);
        }

        [Fact]
        public async Task XbooruCheckWiki()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Xbooru().GetWiki("futanari"); });
        }

        [Fact]
        public async Task XbooruCheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Xbooru().GetRelated("sky"); });
        }

        [Fact]
        public async Task XbooruCheckComment()
        {
            General.CheckComment(await new Xbooru().GetComment(740157));
        }
    }

    public class UnitFurrybooru
    {
        [Fact]
        public async Task FurrybooruCount()
        {
            await General.CheckCount(new Furrybooru());
        }

        [Fact]
        public async Task FurrybooruGetByOffset()
        {
            await General.CheckGetByOffset(new Furrybooru());
        }

        [Fact]
        public async Task FurrybooruGetRandom()
        {
            await General.CheckGetRandom(new Furrybooru());
        }

        [Fact]
        public async Task FurrybooruCheckTag()
        {
            await General.CheckTag(new Furrybooru());
        }

        [Fact]
        public async Task FurrybooruTagId()
        {
            Assert.Equal("kantai_collection", (await new Furrybooru().GetTag(151628)).name);
        }

        [Fact]
        public async Task FurrybooruCheckWiki()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Furrybooru().GetWiki("futanari"); });
        }

        [Fact]
        public async Task FurrybooruCheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Furrybooru().GetRelated("sky"); });
        }

        [Fact]
        public async Task FurrybooruCheckComment()
        {
            General.CheckComment(await new Furrybooru().GetComment(1282210));
        }
    }

    public class UnitRealbooru
    {
        [Fact]
        public async Task RealbooruCount()
        {
            await General.CheckCount(new Realbooru());
        }

        [Fact]
        public async Task RealbooruGetByOffset()
        {
            await General.CheckGetByOffset(new Realbooru());
        }

        [Fact]
        public async Task RealbooruGetRandom()
        {
            await General.CheckGetRandom(new Realbooru());
        }

        [Fact]
        public async Task RealbooruCheckTag()
        {
            await General.CheckTag(new Realbooru());
        }

        [Fact]
        public async Task RealbooruTagId()
        {
            Assert.Equal("kantai_collection", (await new Realbooru().GetTag(688290)).name);
        }

        [Fact]
        public async Task RealbooruCheckWiki()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Realbooru().GetWiki("futanari"); });
        }

        [Fact]
        public async Task RealbooruCheckRelated()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Realbooru().GetRelated("sky"); });
        }

        [Fact]
        public async Task RealbooruCheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Realbooru().GetComment(646911); });
        }
    }

    public class UnitSakugabooru
    {
        [Fact]
        public async Task SakugabooruCount()
        {
            await General.CheckCount(new Sakugabooru(), "kantai_collection", "animated");
        }

        [Fact]
        public async Task SakugabooruGetByOffset()
        {
            await General.CheckGetByOffset(new Sakugabooru(), "kantai_collection");
        }

        [Fact]
        public async Task SakugabooruGetRandom()
        {
            await General.CheckGetRandom(new Sakugabooru(), "kantai_collection");
        }

        [Fact]
        public async Task SakugabooruCheckTag()
        {
            await General.CheckTag(new Sakugabooru(), "kantai_collection");
        }

        [Fact]
        public async Task SakugabooruTagId()
        {
            Assert.Equal("kantai_collection", (await new Sakugabooru().GetTag(7148)).name);
        }

        [Fact]
        public async Task SakugabooruCheckWiki()
        {
            Search.Wiki.SearchResult result = await new Sakugabooru().GetWiki("animated");
            Assert.Equal(13, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public async Task SakugabooruCheckRelated()
        {
            Search.Related.SearchResult[] result = await new Sakugabooru().GetRelated("kantai_collection");
            General.CheckRelated(result);
        }

        [Fact]
        public async Task SakugabooruCheckComment()
        {
            await Assert.ThrowsAsync<Search.FeatureUnavailable>(async delegate () { await new Sakugabooru().GetComment(38886); });
        }
    }

    public class Other
    {
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
