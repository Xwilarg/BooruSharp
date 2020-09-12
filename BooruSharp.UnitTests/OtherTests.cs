using BooruSharp.Booru;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    // TODO: Find a way to check if commands are available on website or not

    public class OtherTests
    {
        [Fact]
        public async Task GelbooruTagCharacterAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Character,
                (await Boorus.Get<Gelbooru>().GetTagAsync("cirno")).Type);
        }

        [Fact]
        public async Task GelbooruTagCopyrightAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Copyright,
                (await Boorus.Get<Gelbooru>().GetTagAsync("kantai_collection")).Type);
        }

        [Fact]
        public async Task GelbooruTagArtistAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Artist,
                (await Boorus.Get<Gelbooru>().GetTagAsync("mtu_(orewamuzituda)")).Type);
        }

        [Fact]
        public async Task GelbooruTagMetadataAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Metadata,
                (await Boorus.Get<Gelbooru>().GetTagAsync("uncensored")).Type);
        }

        [Fact]
        public async Task GelbooruTagTriviaAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Trivia,
                (await Boorus.Get<Gelbooru>().GetTagAsync("futanari")).Type);
        }
    }
}
