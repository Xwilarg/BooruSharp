using System;

namespace BooruSharp.Search.Wiki
{
    public struct SearchResult
    {
        public SearchResult(int id, string title, DateTime creation, DateTime lastUpdate, string body)
        {
            this.id = id;
            this.title = title;
            this.creation = creation;
            this.lastUpdate = lastUpdate;
            this.body = body;
        }

        /// <summary>
        /// Id of the wiki entry
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Name of the tag described
        /// </summary>
        public readonly string title;

        /// <summary>
        /// Date when the entry was created
        /// </summary>
        public readonly DateTime creation;

        /// <summary>
        /// Date of the latest update to the entry
        /// </summary>
        public readonly DateTime lastUpdate;

        /// <summary>
        /// Tag description
        /// </summary>
        public readonly string body;
    }
}
