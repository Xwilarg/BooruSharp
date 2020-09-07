using BooruSharp.Booru;
using BooruSharp.Others;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    public class BooruTests
    {
        private const int _randomPostCount = 5;

        [Fact]
        public void IsBooruAuthSet()
        {
            var b = new Gelbooru();
            Assert.True(b.Auth == null);
            b.Auth = new BooruAuth("AAA", "AAA");
            Assert.False(b.Auth == null);
        }

        [SkipIfNoEnvTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task UnsetFavoriteErrorAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            var id = (await General.GetRandomPostAsync(booru)).ID;
            booru.Auth = new BooruAuth("AAA", "AAA");
            if (!booru.HasFavoriteAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.RemoveFavoriteAsync(id));
            else if (t == typeof(Gelbooru))
                await Assert.ThrowsAsync<Search.AuthentificationInvalid>(async () => await booru.RemoveFavoriteAsync(id));
        }

        [SkipIfNoEnvTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task SetFavoriteErrorAsync(Type t)
        {
            var booru = (ABooru)Activator.CreateInstance(t);
            if (booru is Pixiv pixiv)
                await Assert.ThrowsAsync<Search.AuthentificationInvalid>(async () => await pixiv.LoginAsync("AAA", "AAA"));
            else
            {
                booru.Auth = new BooruAuth("AAA", "AAA");
                if (!booru.HasFavoriteAPI)
                    await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.AddFavoriteAsync(800));
                else
                    await Assert.ThrowsAsync<Search.AuthentificationInvalid>(async () => await booru.AddFavoriteAsync(800));
            }
        }

        [SkipIfNoEnvTheory]
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
        // [InlineData(typeof(Xbooru))] // Xbooru allow to add post with invalid ID
        [InlineData(typeof(Yandere))]
        [InlineData(typeof(Pixiv))]
        public async Task SetFavoriteInvalidIdAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);

            if (!booru.HasFavoriteAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.AddFavoriteAsync(int.MaxValue));
            else
            {
                // Pixiv doesn't support authorization using Auth property.
                if (!(booru is Pixiv))
                {
                    string name = t.Name.ToUpperInvariant();
                    booru.Auth = new BooruAuth(
                        Environment.GetEnvironmentVariable(name + "_USER_ID"),
                        Environment.GetEnvironmentVariable(name + "_PASSWORD_HASH"));
                }

                await Assert.ThrowsAsync<Search.InvalidPostId>(async () => await booru.AddFavoriteAsync(int.MaxValue));
            }
        }

        [SkipIfNoEnvTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task SetFavoriteAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);

            if (!booru.HasFavoriteAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.AddFavoriteAsync(10));
            else
            {
                var id = (await General.GetRandomPostAsync(booru)).ID;

                // Pixiv doesn't support authorization using Auth property.
                if (!(booru is Pixiv))
                {
                    string name = t.Name.ToUpperInvariant();
                    booru.Auth = new BooruAuth(
                        Environment.GetEnvironmentVariable(name + "_USER_ID"),
                        Environment.GetEnvironmentVariable(name + "_PASSWORD_HASH"));
                }

                await booru.AddFavoriteAsync(id);
                await booru.RemoveFavoriteAsync(id);
            }
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task GetByMd5Async(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasPostByMd5API)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetPostByMd5Async("0"));
            else
            {
                Search.Post.SearchResult result1;
                do
                {
                    result1 = await General.GetRandomPostAsync(booru);
                } while (result1.MD5 == null);
                var result2 = await booru.GetPostByMd5Async(result1.MD5);
                Assert.Equal(result1.ID, result2.ID);
            }
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task GetByIdAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasPostByIdAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetPostByIdAsync(0));
            else
            {
                Search.Post.SearchResult result1 = await General.GetRandomPostAsync(booru);
                var result2 = await booru.GetPostByIdAsync(result1.ID);
                Assert.Equal(result1.ID, result2.ID);
            }
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task GetLastPostsAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (booru.NoEmptyPostSearch)
                await Assert.ThrowsAsync<ArgumentException>(async () => await booru.GetLastPostsAsync());
            else
            {
                var results = await booru.GetLastPostsAsync();
                Assert.NotInRange(results.Length, 0, 1);
                Assert.NotEqual(results[0].ID, results[1].ID);
            }
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621), "kantai_collection", "swimwear")]
        [InlineData(typeof(E926), "kantai_collection", "swimwear")]
        [InlineData(typeof(Furrybooru), "kantai_collection")]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan), "hibiki_(kancolle)")]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "swimsuit", "asian")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection", "explosions")]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru), "kantai_collection")]
        [InlineData(typeof(Yandere), "kantai_collection", "swimsuits")]
        [InlineData(typeof(Pixiv), "響(艦隊これくしょん)", "水着艦娘")]
        public async Task GetLastPostsWithTagsAsync(Type t, string tag = "hibiki_(kantai_collection)", string tag2 = "swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            Search.Post.SearchResult[] results;
            results = await booru.GetLastPostsAsync(tag, tag2);
            Assert.NotInRange(results.Length, 0, 1);
            Assert.NotEqual(results[0].ID, results[1].ID);
            foreach (var elem in results)
            {
                Assert.Contains(elem.Tags, t => t.Contains(tag));
                Assert.Contains(elem.Tags, t => t.Contains(tag2));
            }
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621), "kantai_collection", "swimwear")]
        [InlineData(typeof(E926), "kantai_collection", "swimwear")]
        [InlineData(typeof(Furrybooru), "kantai_collection")]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan), "hibiki_(kancolle)")]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "swimsuit", "asian")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection", "explosions")]
        [InlineData(typeof(SankakuComplex))]
        [InlineData(typeof(Xbooru), "kantai_collection")]
        [InlineData(typeof(Yandere), "kantai_collection", "swimsuits")]
        [InlineData(typeof(Pixiv), "響(艦隊これくしょん)", "水着")]
        public async Task GetPostCountAsync(Type t, string tag = "hibiki_(kantai_collection)", string tag2 = "swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasPostCountAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetPostCountAsync());
            else
            {
                int countEmpty = booru.NoEmptyPostSearch
                    ? int.MaxValue
                    // Pixiv doesn't handle PostCount with no tag
                    : await booru.GetPostCountAsync();
                var countOne = await booru.GetPostCountAsync(tag);
                var countTwo = await booru.GetPostCountAsync(tag, tag2);
                Assert.NotEqual(0, countEmpty);
                Assert.NotEqual(0, countOne);
                Assert.NotEqual(0, countTwo);
                Assert.InRange(countOne, countTwo, countEmpty);
            }
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "small_breasts")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection")]
        [InlineData(typeof(SankakuComplex), "small_breasts")]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        [InlineData(typeof(Pixiv), "スク水")]
        public async Task GetRandomAsync(Type t, string tag = "school_swimsuit")
        {
            await General.CheckGetRandomAsync(await Boorus.GetAsync(t), tag);
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "small_breasts")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection")]
        [InlineData(typeof(SankakuComplex), "small_breasts")]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        [InlineData(typeof(Pixiv), "スク水")]
        public async Task GetRandomsAsync(Type t, string tag = "school_swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasMultipleRandomAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await General.CheckGetRandomsAsync(booru, tag));
            else
                await General.CheckGetRandomsAsync(booru, tag);
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621))]
        [InlineData(typeof(E926))]
        [InlineData(typeof(Furrybooru))]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan))]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "small_breasts")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection")]
        [InlineData(typeof(SankakuComplex), "small_breasts")]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        [InlineData(typeof(Pixiv), "スク水")]
        public async Task GetRandomsTooManyAsync(Type t, string tag = "school_swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasMultipleRandomAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetRandomPostsAsync(int.MaxValue, tag));
            else
            {
                var result = await booru.GetRandomPostsAsync(int.MaxValue, tag);
                Assert.NotEmpty(result);
                foreach (var r in result)
                    Assert.Contains(r.Tags, t => t.Contains(tag));
            }
        }

        [Fact]
        public async Task SetHttpClientAsync()
        {
            var booru = new Gelbooru();
            HttpClient hc = new HttpClient();
            hc.DefaultRequestHeaders.Add("User-Agent", "BooruSharp.Unit-Tests");
            booru.HttpClient = hc;
            await General.CheckGetRandomAsync(booru, "kantai_collection");
            Assert.Single(hc.DefaultRequestHeaders.GetValues("User-Agent"));
            Assert.Contains("BooruSharp.Unit-Tests", hc.DefaultRequestHeaders.GetValues("User-Agent"));
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621), "kantai_collection")]
        [InlineData(typeof(E926), "kantai_collection")]
        [InlineData(typeof(Furrybooru), "kantai_collection")]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan), "hibiki_(kancolle)")]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "school_swimsuit", "small_breasts")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection", "explosions")]
        [InlineData(typeof(SankakuComplex), "hibiki_(kantai_collection)", "old_school_swimsuit")]
        [InlineData(typeof(Xbooru), "kantai_collection")]
        [InlineData(typeof(Yandere), "kantai_collection")]
        [InlineData(typeof(Pixiv), "響(艦隊これくしょん)", "水着艦娘")]
        public async Task GetRandom2TagsAsync(Type t, string tag = "hibiki_(kantai_collection)", string tag2 = "school_swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            var result = await booru.GetRandomPostAsync(tag, tag2);
            Assert.Contains(result.Tags, t => t.Contains(tag));
            Assert.Contains(result.Tags, t => t.Contains(tag2));
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru))]
        [InlineData(typeof(DanbooruDonmai))]
        [InlineData(typeof(E621), "kantai_collection")]
        [InlineData(typeof(E926), "kantai_collection")]
        [InlineData(typeof(Furrybooru), "kantai_collection")]
        [InlineData(typeof(Gelbooru))]
        [InlineData(typeof(Konachan), "hibiki_(kancolle)")]
        [InlineData(typeof(Lolibooru))]
        [InlineData(typeof(Realbooru), "school_swimsuit", "small_breasts")]
        [InlineData(typeof(Rule34))]
        [InlineData(typeof(Safebooru))]
        [InlineData(typeof(Sakugabooru), "kantai_collection", "explosions")]
        [InlineData(typeof(SankakuComplex), "hibiki_(kantai_collection)", "old_school_swimsuit")]
        [InlineData(typeof(Xbooru), "kantai_collection")]
        [InlineData(typeof(Yandere), "kantai_collection")]
        [InlineData(typeof(Pixiv), "響(艦隊これくしょん)", "スク水")]
        public async Task GetRandoms2TagsAsync(Type t, string tag = "hibiki_(kantai_collection)", string tag2 = "school_swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasMultipleRandomAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetRandomPostsAsync(5, tag, tag2));
            else
            {
                var result = await booru.GetRandomPostsAsync(5, tag, tag2);
                Assert.NotEmpty(result);
                foreach (var r in result)
                {
                    Assert.Contains(r.Tags, t => t.Contains(tag));
                    Assert.Contains(r.Tags, t => t.Contains(tag2));
                }
            }
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru), false)]
        [InlineData(typeof(DanbooruDonmai), true)]
        [InlineData(typeof(E621), false, "sea", "loli", "swimwear")]
        [InlineData(typeof(E926), false, "sea", "breasts", "swimwear")]
        [InlineData(typeof(Furrybooru), false, "water")]
        [InlineData(typeof(Gelbooru), false)]
        [InlineData(typeof(Konachan), false, "water")]
        [InlineData(typeof(Lolibooru), false)]
        [InlineData(typeof(Realbooru), false, "water")]
        [InlineData(typeof(Rule34), false)]
        [InlineData(typeof(Safebooru), false)]
        [InlineData(typeof(Sakugabooru), false, "kantai_collection", "explosions", "fire")]
        [InlineData(typeof(SankakuComplex), false, "ocean", "loli", "swimsuit")]
        [InlineData(typeof(Xbooru), false, "ocean", "small_breasts")]
        [InlineData(typeof(Yandere), false, "see_through", "loli", "swimsuits")]
        [InlineData(typeof(Pixiv), false, "東方", "貧乳", "水着")]
        public async Task TooManyTagsAsync(
            Type t, bool throwError, string tag = "ocean", string tag2 = "flat_chest", string tag3 = "swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            Search.Post.SearchResult result;
            if (throwError)
            {
                await Assert.ThrowsAsync<Search.TooManyTags>(async () =>
                {
                    result = await booru.GetRandomPostAsync(tag, tag2, tag3);
                });
            }
            else
            {
                result = await booru.GetRandomPostAsync(tag, tag2, tag3);
                Assert.Contains(result.Tags, x => x.Contains(tag));
                Assert.Contains(result.Tags, x => x.Contains(tag2));
                Assert.Contains(result.Tags, x => x.Contains(tag3));
            }
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru), false)]
        [InlineData(typeof(DanbooruDonmai), true)]
        [InlineData(typeof(E621), false, "sea", "loli", "swimwear")]
        [InlineData(typeof(E926), false, "sea", "loli", "swimwear")]
        [InlineData(typeof(Furrybooru), false, "water")]
        [InlineData(typeof(Gelbooru), false)]
        [InlineData(typeof(Konachan), false, "water")]
        [InlineData(typeof(Lolibooru), false)]
        [InlineData(typeof(Realbooru), false, "water")]
        [InlineData(typeof(Rule34), false)]
        [InlineData(typeof(Safebooru), false)]
        [InlineData(typeof(Sakugabooru), false, "kantai_collection", "explosions", "fire")]
        [InlineData(typeof(SankakuComplex), false, "ocean", "loli", "swimsuit")]
        [InlineData(typeof(Xbooru), false, "ocean", "small_breasts")]
        [InlineData(typeof(Yandere), false, "see_through", "loli", "swimsuits")]
        [InlineData(typeof(Pixiv), false, "水", "貧乳", "水着")]
        public async Task TooManyTagsManyAsync(
            Type t, bool throwError, string tag = "ocean", string tag2 = "flat_chest", string tag3 = "swimsuit")
        {
            var booru = await Boorus.GetAsync(t);
            Search.Post.SearchResult[] result;
            if (throwError)
            {
                await Assert.ThrowsAsync<Search.TooManyTags>(async () =>
                {
                    result = await booru.GetRandomPostsAsync(_randomPostCount, tag, tag2, tag3);
                });
            }
            else if (!booru.HasMultipleRandomAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(
                    async () => await booru.GetRandomPostsAsync(_randomPostCount, tag, tag2, tag3));
            else
            {
                result = await booru.GetRandomPostsAsync(_randomPostCount, tag, tag2, tag3);
                foreach (var r in result)
                {
                    Assert.Contains(tag, r.Tags);
                    Assert.Contains(tag2, r.Tags);
                    Assert.Contains(tag3, r.Tags);
                }
            }
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task GetRandomFailAsync(Type t)
        {
            await Assert.ThrowsAsync<Search.InvalidTags>(
                async () => await (await Boorus.GetAsync(t)).GetRandomPostAsync("someInvalidTag"));
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task GetRandomsFailAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasMultipleRandomAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetRandomPostsAsync(5, "someInvalidTag"));
            else
                Assert.Empty(await booru.GetRandomPostsAsync(5, "someInvalidTag"));
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv), "パンスト")]
        public async Task CheckTagAsync(Type t, string tag = "pantyhose")
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasTagByIdAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetTagAsync(tag));
            else
                await General.CheckTagAsync(booru, tag);
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task CheckTagFailAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasTagByIdAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetTagAsync("someRandomTag"));
            else
                await Assert.ThrowsAsync<Search.InvalidTags>(() => booru.GetTagAsync("someRandomTag"));
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv), "艦隊こ", false)]
        public async Task CheckTagsAsync(Type t, string tag, bool onlyOnce)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasTagByIdAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetTagAsync(tag));
            else if (onlyOnce)
                Assert.NotEmpty(await booru.GetTagsAsync(tag));
            else
                Assert.NotInRange((await booru.GetTagsAsync(tag)).Length, 0, 1);
        }

        [SkippableTheory]
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
        [InlineData(typeof(SankakuComplex), "kantai_collection", 458437)]
        [InlineData(typeof(Xbooru), "hibiki_(kantai_collection)", 151883)]
        [InlineData(typeof(Yandere), "hibiki_(kancolle)", 98153)]
        [InlineData(typeof(Pixiv), "響(艦隊これくしょん)", -1)]
        public async Task TagIdAsync(Type t, string tag, int tagId)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasTagByIdAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetTagAsync(tagId));
            else
                Assert.Equal(tag, (await booru.GetTagAsync(tagId)).Name);
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task TagIdFailAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasTagByIdAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetTagAsync(int.MaxValue));
            else
                await Assert.ThrowsAsync<Search.InvalidTags>(() => booru.GetTagAsync(int.MaxValue));
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru), "highres", 82)]
        [InlineData(typeof(DanbooruDonmai), "futanari", 3589)]
        [InlineData(typeof(E621), "futanari", 123)]
        [InlineData(typeof(E926), "futanari", 123)]
        [InlineData(typeof(Furrybooru), "futanari", -1)]
        [InlineData(typeof(Gelbooru), "futanari", -1)]
        [InlineData(typeof(Konachan), "futanari", 757)]
        [InlineData(typeof(Lolibooru), "futanari", 158)]
        [InlineData(typeof(Realbooru), "futanari", -1)]
        [InlineData(typeof(Rule34), "futanari", -1)]
        [InlineData(typeof(Safebooru), "futanari", -1)]
        [InlineData(typeof(Sakugabooru), "animated", 13)]
        [InlineData(typeof(SankakuComplex), "blush", 826)]
        [InlineData(typeof(Xbooru), "futanari", -1)]
        [InlineData(typeof(Yandere), "futanari", 167)]
        [InlineData(typeof(Pixiv), "ふたなり", -1)]
        public async Task CheckWikiAsync(Type t, string tag, int? id)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasWikiAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetWikiAsync(tag));
            else
            {
                Search.Wiki.SearchResult result = await booru.GetWikiAsync(tag);
                Assert.Equal(id, result.ID);
                General.CheckWiki(result);
            }
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task CheckWikiFailAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasWikiAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetWikiAsync("yetAnotherTag"));
            else
                await Assert.ThrowsAsync<Search.InvalidTags>(() => booru.GetWikiAsync("yetAnotherTag"));
        }

        [SkippableTheory]
        [InlineData(typeof(Atfbooru), "kantai_collection", "anchor_symbol")]
        [InlineData(typeof(DanbooruDonmai), "kantai_collection", "serafuku")]
        [InlineData(typeof(E621), "sky", "cloud")]
        [InlineData(typeof(E926), "sky", "cloud")]
        [InlineData(typeof(Furrybooru), "sky", "cloud")]
        [InlineData(typeof(Gelbooru), "sky", "cloud")]
        [InlineData(typeof(Konachan), "sky", "clouds")]
        [InlineData(typeof(Lolibooru), "sky", "cloud")]
        [InlineData(typeof(Realbooru), "sky", "clouds")]
        [InlineData(typeof(Rule34), "sky", "clouds")]
        [InlineData(typeof(Safebooru), "sky", "clouds")]
        [InlineData(typeof(Sakugabooru), "kantai_collection", "explosions")]
        [InlineData(typeof(SankakuComplex), "sky", "clouds")]
        [InlineData(typeof(Xbooru), "sky", "clouds")]
        [InlineData(typeof(Yandere), "landscape", "wallpaper")]
        [InlineData(typeof(Pixiv), "空", "雲")]
        public async Task CheckRelatedAsync(Type t, string tag, string related)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasRelatedAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetRelatedAsync(tag));
            else
            {
                Search.Related.SearchResult[] result = await booru.GetRelatedAsync(tag);
                General.CheckRelated(result);
                Assert.Contains(result, x => x.Name == related);
            }
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task CheckRelatedFailAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasRelatedAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetRelatedAsync("thisWillFail"));
            else
                Assert.Empty(await booru.GetRelatedAsync("thisWillFail"));
        }

        [SkippableTheory]
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
        [InlineData(typeof(SankakuComplex), 48)]
        [InlineData(typeof(Xbooru), 740157)]
        [InlineData(typeof(Yandere), 619494)]
        [InlineData(typeof(Pixiv), -1)]
        public async Task CheckCommentAsync(Type t, int id)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasCommentAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetCommentsAsync(id));
            else
                General.CheckComment(await booru.GetCommentsAsync(id));
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task CheckCommentFailAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasCommentAPI)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetCommentsAsync(int.MaxValue));
            else
                Assert.Empty(await booru.GetCommentsAsync(int.MaxValue));
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task CheckLastCommentAsync(Type t)
        {
            var booru = await Boorus.GetAsync(t);
            if (!booru.HasSearchLastComment)
                await Assert.ThrowsAsync<Search.FeatureUnavailable>(async () => await booru.GetLastCommentsAsync());
            else
                General.CheckComment(await booru.GetLastCommentsAsync());
        }

        [SkippableTheory]
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
        [InlineData(typeof(Pixiv))]
        public async Task CheckAvailableAsync(Type t)
        {
            await (await Boorus.GetAsync(t)).CheckAvailabilityAsync();
        }

        /*[SkippableTheory]
        public async Task CheckNotAvailable(Type t)
        {
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await ((Booru.Booru)Activator.CreateInstance(t, (BooruAuth)null)).CheckAvailability());
        }*/

        [SkippableTheory]
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
        [InlineData(typeof(SankakuComplex), "pussy_line")]
        [InlineData(typeof(Xbooru))]
        [InlineData(typeof(Yandere))]
        [InlineData(typeof(Pixiv), "おまんこ")]
        public async Task CheckIsSafeAsync(Type t, string explicitTag = "pussy")
        {
            ABooru b = await Boorus.GetAsync(t);
            bool isSafe = b.IsSafe;
            bool foundExplicit = false;
            for (int i = 0; i < 10; i++)
            {
                var image = await b.GetRandomPostAsync(explicitTag);
                if (isSafe && image.FileUrl != null)
                    Assert.NotEqual(Search.Post.Rating.Explicit, image.Rating);
                if (image.Rating == Search.Post.Rating.Explicit)
                    foundExplicit = true;
            }
            if (!isSafe)
                Assert.True(foundExplicit);

        }
    }
}
