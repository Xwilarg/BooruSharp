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
        public async Task GetInvalidImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            await Assert.ThrowsAsync<InvalidTags>(async () => {
                await Utils.DoWebRequest(async () => { return await booru.GetRandomPostAsync("azazazazaz"); });
            });
        }
    }
}
