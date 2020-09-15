using System;

namespace BooruSharp.Utils
{
    internal static class RatingUtils
    {
        /// <summary>
        /// Converts a string to its maching <see cref="Search.Post.Rating"/> value.
        /// </summary>
        public static Search.Post.Rating Parse(string rating)
        {
            if (string.IsNullOrWhiteSpace(rating))
                throw new ArgumentException("String cannot be null, empty, or whitespace.", nameof(rating));

            switch (rating[0])
            {
                case 's': case 'S': return Search.Post.Rating.Safe;
                case 'q': case 'Q': return Search.Post.Rating.Questionable;
                case 'e': case 'E': return Search.Post.Rating.Explicit;
                default: throw new ArgumentException($"Invalid rating '{rating}'.", nameof(rating));
            }
        }
    }
}
