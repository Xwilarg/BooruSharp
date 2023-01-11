using System;

namespace BooruSharp.UnitTests
{
    public record BooruTestData
    {
        public required Type BooruType;
        public required string[] Tags;
        public required int ValidPostId;
        public required string ExplicitTag;
    }
}
