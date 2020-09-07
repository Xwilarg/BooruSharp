using System;

namespace BooruSharp.Search.Wiki
{
    /// <summary>
    /// Represents a wiki entry API search result.
    /// </summary>
    public struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="id">The ID of the wiki entry.</param>
        /// <param name="title">The name of the described tag.</param>
        /// <param name="creation">The date when the wiki entry was created.</param>
        /// <param name="lastUpdate">The date of the latest update to the wiki entry.</param>
        /// <param name="body">The tag description.</param>
        public SearchResult(int id, string title, DateTime creation, DateTime lastUpdate, string body)
        {
            this.id = id;
            this.title = title;
            this.creation = creation;
            this.lastUpdate = lastUpdate;
            this.body = body;
        }

        /// <summary>
        /// Gets the ID of the wiki entry.
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Gets the name of the described tag.
        /// </summary>
        public readonly string title;

        /// <summary>
        /// Gets the date when the wiki entry was created.
        /// </summary>
        public readonly DateTime creation;

        /// <summary>
        /// Gets the date of the latest update to the wiki entry.
        /// </summary>
        public readonly DateTime lastUpdate;

        /// <summary>
        /// Gets the tag description.
        /// </summary>
        public readonly string body;
    }
}
