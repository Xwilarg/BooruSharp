namespace BooruSharp.Search.Tag
{
    public struct SearchResult
    {
        public SearchResult(int id, string name, TagType type, int count)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.count = count;
        }
        public readonly int id;
        public readonly string name;
        public readonly TagType type;
        public readonly int count;
    }
}
