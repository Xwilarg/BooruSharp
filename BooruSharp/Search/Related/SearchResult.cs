namespace BooruSharp.Search.Related
{
    public struct SearchResult
    {
        public SearchResult(string name, uint count)
        {
            this.name = name;
            this.count = count;
        }
        public readonly string name;
        public readonly uint count;
    }
}
