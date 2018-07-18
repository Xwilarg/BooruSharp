namespace BooruSharp.TagSearch
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

        public uint id;
        public string name;
        public TagType type;
        public uint count;
    }
}
