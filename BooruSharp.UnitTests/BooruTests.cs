using BooruSharp.Search;
using BooruSharp.Search.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    public class BooruTests : IClassFixture<TestsFixture>
    {
        // Because of compile error
        public static IEnumerable<object[]> BooruParams => Utils.BooruParams;

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetRandomImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            if (booru.CanSearchWithNoTag)
            {
                var res = await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync(); });
                var res2 = await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync(); });
                await Utils.ValidatePostAsync(res, Array.Empty<string>());
                await Utils.ValidatePostAsync(res2, Array.Empty<string>());
                Assert.NotEqual(res.ID, res2.ID);
            }
            else
            {
                await Assert.ThrowsAsync<FeatureUnavailable>(async () => { await booru.GetRandomPostAsync(); });
            }
        }

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetRandomImageWith1TagAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            var targetTag = data.Tags.Take(1).ToArray();
            var res = await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync(targetTag); });
            var res2 = await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync(targetTag); });
            await Utils.ValidatePostAsync(res, targetTag);
            await Utils.ValidatePostAsync(res2, targetTag);
            Assert.NotEqual(res.ID, res2.ID);
        }

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetRandomImageWith3TagsAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            var targetTag = data.Tags.Take(3).ToArray();
            if (booru.MaxNumberOfTags == -1)
            {
                var res = await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync(targetTag); });
                await Utils.ValidatePostAsync(res, targetTag);
            }
            else
            {
                await Assert.ThrowsAsync<TooManyTags>(async () => { await booru.GetRandomPostAsync(targetTag); });
            }
        }

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetInvalidImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            await Assert.ThrowsAsync<InvalidPostException>(async () => {
                await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync("azazazazaz"); });
            });
        }

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetExplicitImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            Assert.True((booru.IsSafe && data.ExplicitTag == null) || (!booru.IsSafe && data.ExplicitTag != null));
            if (!booru.IsSafe)
            {
                var post = await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync(data.ExplicitTag); });
                Assert.Equal(Rating.Explicit, post.Rating);
            }
        }

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetImageByIdAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            if (booru.HasPostByIdAPI)
            {
                var res = await Utils.DoWebRequest(async () => { return await booru.GetPostByIdAsync(data.ValidPostId); });
                await Utils.ValidatePostAsync(res, Array.Empty<string>());
            }
            else
            {
                await Assert.ThrowsAsync<FeatureUnavailable>(async () => { await booru.GetPostByIdAsync(data.ValidPostId); });
            }
        }

        [SkippableTheory]
        [MemberData(nameof(BooruParams))]
        public async Task GetImageWithInvalidPostID(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            if (booru.HasPostByIdAPI)
            {
                await Assert.ThrowsAsync<InvalidPostException>(async () => {
                    await Utils.DoWebRequest(async () => { return await booru.GetPostByIdAsync(int.MaxValue); });
                });
            }
            else
            {
                await Assert.ThrowsAsync<FeatureUnavailable>(async () => { await booru.GetPostByIdAsync(int.MaxValue); });
            }
        }
    }
}
