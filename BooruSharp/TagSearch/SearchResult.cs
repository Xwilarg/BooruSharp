namespace BooruSharp.TagSearch
{
    public struct SearchResult
    {
        public SearchResult(uint id, string name, TagType type)
        {
            this.id = id;
            this.name = name;
            this.type = type;
        }

        public uint id;
        public string name;
        public TagType type;
    }
}
