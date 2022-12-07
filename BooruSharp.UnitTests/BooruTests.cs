using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    public class BooruTests
    {
        // Because of compile error
        public static IEnumerable<object[]> BooruParams => Utils.BooruParams;

        [Theory]
        [MemberData(nameof(BooruParams))]
        public async Task GetRandomImageAsync(BooruTestData data)
        {
            var booru = await Utils.GetAsync(data.BooruType);
            await booru.GetRandomPostAsync();
        }
    }
}
