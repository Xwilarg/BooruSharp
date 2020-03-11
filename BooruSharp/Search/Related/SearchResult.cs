namespace BooruSharp.Search.Related
{
    public struct SearchResult
    {
        public SearchResult(string name, int? count)
        {
            this.name = name;
            this.count = count;
        }

        /// <summary>
        /// Name of the tag
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Number of occurences of the tag
        /// </summary>
        public readonly int? count;
    }
}
