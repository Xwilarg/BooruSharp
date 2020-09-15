using System;

namespace BooruSharp.Utils
{
    internal static class TagUtils
    {
        public static Search.Tag.TagType Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("String cannot be null, empty, or whitespace.", nameof(value));

            if (StringComparer.OrdinalIgnoreCase.Equals(value, "tag"))
                return Search.Tag.TagType.Trivia;

            if (Enum.TryParse<Search.Tag.TagType>(value, true, out var type))
                return type;

            throw new ArgumentException($"Invalid tag '{value}'.", nameof(value));
        }
    }
}
