using BooruSharp.Booru;
using System;
using System.Net;
using Xunit;

namespace BooruSharp.UnitTests
{
    public class Program
    {
        private void CheckCount(Booru.Booru booru)
        {
            int nbGeneral = booru.GetNbImage();
            int nbMed = booru.GetNbImage("wet");
            int nbMin = booru.GetNbImage("wet", "swimsuit");
            Assert.NotEqual(0, nbMin);
            Assert.InRange(nbMed, nbMin, nbGeneral);
        }

        [Fact]
        public void GelbooruCount()
        {
            CheckCount(new Gelbooru());
        }

        [Fact]
        public void SafebooruCount()
        {
            CheckCount(new Safebooru());
        }

        [Fact]
        public void KonachanCount()
        {
            CheckCount(new Konachan());
        }

        [Fact]
        public void E621Count()
        {
            CheckCount(new E621());
        }

        [Fact]
        public void Rule34Count()
        {
            CheckCount(new Rule34());
        }

        [Fact]
        public void LolibooruCount()
        {
            CheckCount(new Lolibooru());
        }

        [Fact]
        public void YandereCount()
        {
            CheckCount(new Yandere());
        }

        private bool CheckUrl(string url)
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

        private void CheckResult(Search.Image.SearchResult result, string inputTag)
        {
            Assert.True(CheckUrl(result.fileUrl));
            Assert.True(CheckUrl(result.previewUrl));
            Assert.InRange(result.rating, Search.Image.Rating.Safe, Search.Image.Rating.Explicit);
            Assert.Contains(inputTag, result.tags);
        }

        private void CheckGetByOffset(Booru.Booru booru)
        {
            Search.Image.SearchResult result = booru.GetImage(2, "school_swimsuit");
            CheckResult(result, "school_swimsuit");
        }

        [Fact]
        public void GelbooruGetByOffset()
        {
            CheckGetByOffset(new Gelbooru());
        }

        [Fact]
        public void SafebooruGetByOffset()
        {
            CheckGetByOffset(new Safebooru());
        }

        [Fact]
        public void KonachanGetByOffset()
        {
            CheckGetByOffset(new Konachan());
        }

        [Fact]
        public void E621GetByOffset()
        {
            CheckGetByOffset(new E621());
        }

        [Fact]
        public void Rule34GetByOffset()
        {
            CheckGetByOffset(new Rule34());
        }

        [Fact]
        public void LolibooruGetByOffset()
        {
            CheckGetByOffset(new Lolibooru());
        }

        [Fact]
        public void YandereGetByOffset()
        {
            CheckGetByOffset(new Yandere());
        }

        private void CheckGetRandom(Booru.Booru booru)
        {
            Search.Image.SearchResult result = booru.GetRandomImage("school_swimsuit");
            CheckResult(result, "school_swimsuit");
        }

        [Fact]
        public void GelbooruGetRandom()
        {
            CheckGetRandom(new Gelbooru());
        }

        [Fact]
        public void SafebooruGetRandom()
        {
            CheckGetRandom(new Safebooru());
        }

        [Fact]
        public void KonachanGetRandom()
        {
            CheckGetRandom(new Konachan());
        }

        [Fact]
        public void E621GetRandom()
        {
            CheckGetRandom(new E621());
        }

        [Fact]
        public void Rule34GetRandom()
        {
            CheckGetRandom(new Rule34());
        }

        [Fact]
        public void LolibooruGetRandom()
        {
            CheckGetRandom(new Lolibooru());
        }

        [Fact]
        public void YandereGetRandom()
        {
            CheckGetRandom(new Yandere());
        }

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

        private void CheckTag(Booru.Booru booru)
        {
            Search.Tag.SearchResult result = booru.GetTag("pantyhose");
            Assert.Equal("pantyhose", result.name);
            Assert.InRange(result.type, Search.Tag.TagType.Trivia, Search.Tag.TagType.Metadata);
            Assert.NotEqual((Search.Tag.TagType)2, result.type);
            Assert.NotEqual<uint>(0, result.count);
        }

        [Fact]
        public void GelbooruCheckTag()
        {
            CheckTag(new Gelbooru());
        }

        [Fact]
        public void SafebooruCheckTag()
        {
            CheckTag(new Safebooru());
        }

        [Fact]
        public void KonachanCheckTag()
        {
            CheckTag(new Konachan());
        }

        [Fact]
        public void E621CheckTag()
        {
            CheckTag(new E621());
        }

        [Fact]
        public void Rule34CheckTag()
        {
            CheckTag(new Rule34());
        }

        [Fact]
        public void LolibooruCheckTag()
        {
            CheckTag(new Lolibooru());
        }

        [Fact]
        public void YandereCheckTag()
        {
            CheckTag(new Yandere());
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

        [Fact]
        public void SafebooruTagId()
        {
            Assert.Equal("hibiki_(kantai_collection)", new Safebooru().GetTag(316679).name);
        }

        private void CheckWiki(Search.Wiki.SearchResult result)
        {
            Assert.InRange(result.lastUpdate, result.creation, DateTime.Now);
        }

        [Fact]
        public void GelbooruCheckWiki()
        {
            Assert.Throws<Search.Wiki.NoWiki>(delegate() { new Gelbooru().GetWiki("futanari"); });
        }

        [Fact]
        public void SafebooruCheckWiki()
        {
            Assert.Throws<Search.Wiki.NoWiki>(delegate () { new Safebooru().GetWiki("futanari"); });
        }

        [Fact]
        public void KonachanCheckWiki()
        {
            Search.Wiki.SearchResult result = new Konachan().GetWiki("futanari");
            Assert.Equal<uint>(757, result.id);
            CheckWiki(result);
        }

        [Fact]
        public void E621CheckWiki()
        {
            Search.Wiki.SearchResult result = new E621().GetWiki("futanari");
            Assert.Equal<uint>(123, result.id);
            CheckWiki(result);
        }

        [Fact]
        public void Rule34CheckWiki()
        {
            Assert.Throws<Search.Wiki.NoWiki>(delegate () { new Rule34().GetWiki("futanari"); });
        }

        [Fact]
        public void LolibooruCheckWiki()
        {
            Search.Wiki.SearchResult result = new Lolibooru().GetWiki("futanari");
            Assert.Equal<uint>(158, result.id);
            CheckWiki(result);
        }

        [Fact]
        public void YandereCheckWiki()
        {
            Search.Wiki.SearchResult result = new Yandere().GetWiki("futanari");
            Assert.Equal<uint>(167, result.id);
            CheckWiki(result);
        }
    }
}
