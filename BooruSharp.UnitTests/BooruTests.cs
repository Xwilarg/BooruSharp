using BooruSharp.Booru;
using System.Collections.Generic;

namespace BooruSharp.UnitTests
{
    public class BooruTests
    {
        public static IEnumerable<BooruTestData> BooruParams { get; } = new[]
        {
            new BooruTestData() { BooruType = typeof(Atfbooru) },
            new BooruTestData() { BooruType = typeof(DanbooruDonmai) },
            new BooruTestData() { BooruType = typeof(Derpibooru) },
            new BooruTestData() { BooruType = typeof(E621) },
            new BooruTestData() { BooruType = typeof(E926) },
            new BooruTestData() { BooruType = typeof(Furrybooru) },
            new BooruTestData() { BooruType = typeof(Gelbooru) },
            new BooruTestData() { BooruType = typeof(Konachan) },
            new BooruTestData() { BooruType = typeof(Lolibooru) },
            new BooruTestData() { BooruType = typeof(Ponybooru) },
            new BooruTestData() { BooruType = typeof(Realbooru) },
            new BooruTestData() { BooruType = typeof(Rule34) },
            new BooruTestData() { BooruType = typeof(Safebooru) },
            new BooruTestData() { BooruType = typeof(Sakugabooru) },
            new BooruTestData() { BooruType = typeof(SankakuComplex) },
            new BooruTestData() { BooruType = typeof(Twibooru) },
            new BooruTestData() { BooruType = typeof(Xbooru) },
            new BooruTestData() { BooruType = typeof(Yandere) },
            //new BooruTestData() { BooruType = typeof(Pixiv) }
        };
    }
}
