namespace BooruSharp.Search.Tag
{
    public struct SearchResult
    {
        public SearchResult(uint id, string name, TagType type, uint count)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.count = count;
        }
        public readonly uint id;
        public readonly string name;
        public readonly TagType type;
        public readonly uint count;
    }
}
