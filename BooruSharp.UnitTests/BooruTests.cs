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
        public async Task GetRandomImageAsync(object data)
        {
            var booru = await Utils.GetAsync(((BooruTestData)data).BooruType);
            await booru.GetRandomPostAsync();
        }
    }
}
