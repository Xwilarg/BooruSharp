namespace BooruSharp.UnitTests
{
    public class TestsFixture
    {
        public TestsFixture()
        {
            Utils.Client = new();
            Utils.Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 BooruSharp");
        }
    }
}
