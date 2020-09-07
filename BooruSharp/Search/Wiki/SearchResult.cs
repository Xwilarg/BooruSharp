using System;

namespace BooruSharp.Search.Wiki
{
    /// <summary>
    /// Represents a wiki entry API search result.
    /// </summary>
    public readonly struct SearchResult
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
            ID = id;
            Title = title;
            Creation = creation;
            LastUpdate = lastUpdate;
            Body = body;
        }

        /// <summary>
        /// Gets the ID of the wiki entry.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets the name of the described tag.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the date when the wiki entry was created.
        /// </summary>
        public DateTime Creation { get; }

        /// <summary>
        /// Gets the date of the latest update to the wiki entry.
        /// </summary>
        public DateTime LastUpdate { get; }

        /// <summary>
        /// Gets the tag description.
        /// </summary>
        public string Body { get; }
    }
}
