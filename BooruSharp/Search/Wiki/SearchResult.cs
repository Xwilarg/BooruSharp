using System;

namespace BooruSharp.Search.Wiki
{
    public struct SearchResult
    {
        public SearchResult(uint id, string title, DateTime creation, DateTime lastUpdate, string body)
        {
            this.id = id;
            this.title = title;
            this.creation = creation;
            this.lastUpdate = lastUpdate;
            this.body = body;
        }
        public readonly uint id;
        public readonly string title;
        public readonly DateTime creation;
        public readonly DateTime lastUpdate;
        public readonly string body;
    }
}
