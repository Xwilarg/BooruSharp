using BooruSharp.Booru;
using System.Threading.Tasks;
using Xunit;

namespace BooruSharp.UnitTests
{
    // TODO: Find a way to check if commands are available on website or not

    public class OtherTests
    {// TODO: Need to test these with others boorus
        [Fact]
        public async Task GelbooruTagCharacterAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Character,
                (await (await Boorus.GetAsync<Gelbooru>()).GetTagAsync("cirno")).Type);
        }

        [Fact]
        public async Task GelbooruTagCopyrightAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Copyright,
                (await (await Boorus.GetAsync<Gelbooru>()).GetTagAsync("kantai_collection")).Type);
        }

        [Fact]
        public async Task GelbooruTagArtistAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Artist,
                (await (await Boorus.GetAsync<Gelbooru>()).GetTagAsync("mtu_(orewamuzituda)")).Type);
        }

        [Fact]
        public async Task GelbooruTagMetadataAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Metadata,
                (await (await Boorus.GetAsync<Gelbooru>()).GetTagAsync("highres")).Type);
        }

        [Fact]
        public async Task GelbooruTagTriviaAsync()
        {
            Assert.Equal(
                Search.Tag.TagType.Trivia,
                (await (await Boorus.GetAsync<Gelbooru>()).GetTagAsync("futanari")).Type);
        }
    }
}
