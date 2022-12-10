using BooruSharp.Search;
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

        [Theory]
        [MemberData(nameof(BooruParams))]
        public async Task GetRandomImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            if (booru.CanSearchWithNoTag)
            {
                var res = await booru.GetRandomPostAsync();
                var res2 = await booru.GetRandomPostAsync();
                await Utils.ValidatePostAsync(res, Array.Empty<string>());
                await Utils.ValidatePostAsync(res2, Array.Empty<string>());
                Assert.NotEqual(res.ID, res2.ID);
            }
            else
            {
                await Assert.ThrowsAsync<FeatureUnavailable>(async () => { await booru.GetRandomPostAsync(); });
            }
        }

        [Theory]
        [MemberData(nameof(BooruParams))]
        public async Task GetRandomImageWith1TagAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            var targetTag = data.Tags.Take(1).ToArray();
            var res = await booru.GetRandomPostAsync(targetTag);
            var res2 = await booru.GetRandomPostAsync(targetTag);
            await Utils.ValidatePostAsync(res, targetTag);
            await Utils.ValidatePostAsync(res2, targetTag);
            Assert.NotEqual(res.ID, res2.ID);
        }

        [Theory]
        [MemberData(nameof(BooruParams))]
        public async Task GetInvalidImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            await Assert.ThrowsAsync<InvalidTags>(async () => { await booru.GetRandomPostAsync("azeazeazeazeaze"); });
        }
    }
}
