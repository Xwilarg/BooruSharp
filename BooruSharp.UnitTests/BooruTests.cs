using BooruSharp.Search;
using System;
using System.Collections.Generic;
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
                await Utils.ValidatePostAsync(res, Array.Empty<string>());
            }
            else
            {
                await Assert.ThrowsAsync<FeatureUnavailable>(async () => { await booru.GetRandomPostAsync(); });
            }
        }
    }
}
