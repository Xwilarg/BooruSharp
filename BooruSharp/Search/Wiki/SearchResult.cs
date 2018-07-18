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
        public uint id;
        public string title;
        public DateTime creation;
        public DateTime lastUpdate;
        public string body;
    }
}
