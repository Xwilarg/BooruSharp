using BooruSharp.Booru;
using System;
using System.Net;
using Xunit;

namespace BooruSharp.UnitTests
{
    public static class General
    {
        public static void CheckCount(Booru.Booru booru)
        {
            int nbGeneral = booru.GetNbImage();
            int nbMed = booru.GetNbImage("wet");
            int nbMin = booru.GetNbImage("wet", "swimsuit");
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

        public static void CheckGetByOffset(Booru.Booru booru)
        {
            Search.Post.SearchResult result = booru.GetImage(2, "school_swimsuit");
            CheckResult(result, "school_swimsuit");
        }

        public static void CheckGetRandom(Booru.Booru booru)
        {
            Search.Post.SearchResult result = booru.GetRandomImage("school_swimsuit");
            CheckResult(result, "school_swimsuit");
        }

        public static void CheckTag(Booru.Booru booru)
        {
            Search.Tag.SearchResult result = booru.GetTag("pantyhose");
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
        public void GelbooruCount()
        {
            General.CheckCount(new Gelbooru());
        }

        [Fact]
        public void GelbooruGetByOffset()
        {
            General.CheckGetByOffset(new Gelbooru());
        }

        [Fact]
        public void GelbooruGetRandom()
        {
            General.CheckGetRandom(new Gelbooru());
        }

        [Fact]
        public void GelbooruCheckTag()
        {
            General.CheckTag(new Gelbooru());
        }

        [Fact]
        public void GelbooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", new Gelbooru().GetTag(463392).name);
        }

        [Fact]
        public void GelbooruCheckWiki()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Gelbooru().GetWiki("futanari"); });
        }

        [Fact]
        public void GelbooruCheckRelated()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Gelbooru().GetRelated("sky"); });
        }

        [Fact]
        public void GelbooruCheckComment()
        {
            General.CheckComment(new Gelbooru().GetComment(3988284));
        }
    }

    public class UnitSafebooru
    {
        [Fact]
        public void SafebooruCount()
        {
            General.CheckCount(new Safebooru());
        }

        [Fact]
        public void SafebooruGetByOffset()
        {
            General.CheckGetByOffset(new Safebooru());
        }

        [Fact]
        public void SafebooruGetRandom()
        {
            General.CheckGetRandom(new Safebooru());
        }

        [Fact]
        public void SafebooruCheckTag()
        {
            General.CheckTag(new Safebooru());
        }

        [Fact]
        public void SafebooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", new Safebooru().GetTag(316679).name);
        }

        [Fact]
        public void SafebooruCheckWiki()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Safebooru().GetWiki("futanari"); });
        }

        [Fact]
        public void SafebooruCheckRelated()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Safebooru().GetRelated("sky"); });
        }

        [Fact]
        public void SafebooruCheckComment()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Safebooru().GetComment(132); });
        }
    }

    public class UnitKonachan
    {
        [Fact]
        public void KonachanCount()
        {
            General.CheckCount(new Konachan());
        }

        [Fact]
        public void KonachanGetByOffset()
        {
            General.CheckGetByOffset(new Konachan());
        }

        [Fact]
        public void KonachanGetRandom()
        {
            General.CheckGetRandom(new Konachan());
        }

        [Fact]
        public void KonachanCheckTag()
        {
            General.CheckTag(new Konachan());
        }

        [Fact]
        public void KonachanTagId()
        {
            Assert.Equal("hibiki_(kancolle)", new Konachan().GetTag(75885).name);
        }

        [Fact]
        public void KonachanCheckWiki()
        {
            Search.Wiki.SearchResult result = new Konachan().GetWiki("futanari");
            Assert.Equal<uint>(757, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public void KonachanCheckRelated()
        {
            Search.Related.SearchResult[] result = new Konachan().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public void KonachanCheckComment()
        {
            General.CheckComment(new Konachan().GetComment(142938));
        }
    }

    public class UnitE621
    {
        [Fact]
        public void E621Count()
        {
            General.CheckCount(new E621());
        }

        [Fact]
        public void E621GetByOffset()
        {
            General.CheckGetByOffset(new E621());
        }

        [Fact]
        public void E621GetRandom()
        {
            General.CheckGetRandom(new E621());
        }

        [Fact]
        public void E621CheckTag()
        {
            General.CheckTag(new E621());
        }

        [Fact]
        public void E621TagId()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new E621().GetTag(267881); });
        }

        [Fact]
        public void E621CheckWiki()
        {
            Search.Wiki.SearchResult result = new E621().GetWiki("futanari");
            Assert.Equal<uint>(123, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public void E621CheckRelated()
        {
            Search.Related.SearchResult[] result = new E621().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public void E621CheckComment()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new E621().GetComment(59432); });
        }
    }

    public class UnitRule34
    {
        [Fact]
        public void Rule34Count()
        {
            General.CheckCount(new Rule34());
        }

        [Fact]
        public void Rule34GetByOffset()
        {
            General.CheckGetByOffset(new Rule34());
        }

        [Fact]
        public void Rule34GetRandom()
        {
            General.CheckGetRandom(new Rule34());
        }

        [Fact]
        public void Rule34CheckTag()
        {
            General.CheckTag(new Rule34());
        }

        [Fact]
        public void Rule34TagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", new Rule34().GetTag(321239).name);
        }

        [Fact]
        public void Rule34CheckWiki()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Rule34().GetWiki("futanari"); });
        }

        [Fact]
        public void Rule34CheckRelated()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Rule34().GetRelated("sky"); });
        }

        [Fact]
        public void Rule34CheckComment()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Rule34().GetComment(1556058); });
        }
    }

    public class UnitLolibooru
    {
        [Fact]
        public void LolibooruCount()
        {
            General.CheckCount(new Lolibooru());
        }

        [Fact]
        public void LolibooruGetByOffset()
        {
            General.CheckGetByOffset(new Lolibooru());
        }

        [Fact]
        public void LolibooruGetRandom()
        {
            General.CheckGetRandom(new Lolibooru());
        }

        [Fact]
        public void LolibooruCheckTag()
        {
            General.CheckTag(new Lolibooru());
        }

        [Fact]
        public void LolibooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", new Lolibooru().GetTag(2939).name);
        }

        [Fact]
        public void LolibooruCheckWiki()
        {
            Search.Wiki.SearchResult result = new Lolibooru().GetWiki("futanari");
            Assert.Equal<uint>(158, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public void LolibooruCheckRelated()
        {
            Search.Related.SearchResult[] result = new Lolibooru().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public void LolibooruCheckComment()
        {
            General.CheckComment(new Lolibooru().GetComment(134097));
        }
    }

    public class UnitYandere
    {
        [Fact]
        public void YandereCount()
        {
            General.CheckCount(new Yandere());
        }

        [Fact]
        public void YandereGetByOffset()
        {
            General.CheckGetByOffset(new Yandere());
        }

        [Fact]
        public void YandereGetRandom()
        {
            General.CheckGetRandom(new Yandere());
        }

        [Fact]
        public void YandereCheckTag()
        {
            General.CheckTag(new Yandere());
        }

        [Fact]
        public void YandereTagId()
        {
            Assert.Equal("hibiki_(kancolle)", new Yandere().GetTag(98153).name);
        }

        [Fact]
        public void YandereCheckWiki()
        {
            Search.Wiki.SearchResult result = new Yandere().GetWiki("futanari");
            Assert.Equal<uint>(167, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public void YandereCheckRelated()
        {
            Search.Related.SearchResult[] result = new Yandere().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public void YandereCheckComment()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new Yandere().GetComment(405923); });
        }
    }

    public class UnitE926
    {
        [Fact]
        public void E926Count()
        {
            General.CheckCount(new E926());
        }

        [Fact]
        public void E926GetByOffset()
        {
            General.CheckGetByOffset(new E926());
        }

        [Fact]
        public void E926GetRandom()
        {
            General.CheckGetRandom(new E926());
        }

        [Fact]
        public void E926CheckTag()
        {
            General.CheckTag(new E926());
        }

        [Fact]
        public void E926TagId()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new E926().GetTag(1329650); });
        }

        [Fact]
        public void E926CheckWiki()
        {
            Search.Wiki.SearchResult result = new E926().GetWiki("futanari");
            Assert.Equal<uint>(123, result.id);
            General.CheckWiki(result);
        }

        [Fact]
        public void E926CheckRelated()
        {
            Search.Related.SearchResult[] result = new E926().GetRelated("sky");
            General.CheckRelated(result);
        }

        [Fact]
        public void E926CheckComment()
        {
            Assert.Throws<Search.FeatureUnavailable>(delegate () { new E926().GetComment(541858); });
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
        public void GelbooruTagCharacter()
        {
            Assert.Equal(Search.Tag.TagType.Character, new Gelbooru().GetTag("cirno").type);
        }

        [Fact]
        public void GelbooruTagCopyright()
        {
            Assert.Equal(Search.Tag.TagType.Copyright, new Gelbooru().GetTag("kantai_collection").type);
        }

        [Fact]
        public void GelbooruTagArtist()
        {
            Assert.Equal(Search.Tag.TagType.Artist, new Gelbooru().GetTag("mtu_(orewamuzituda)").type);
        }

        [Fact]
        public void GelbooruTagMetadata()
        {
            Assert.Equal(Search.Tag.TagType.Metadata, new Gelbooru().GetTag("uncensored").type);
        }

        [Fact]
        public void GelbooruTagTrivia()
        {
            Assert.Equal(Search.Tag.TagType.Trivia, new Gelbooru().GetTag("futanari").type);
        }
    }
}
