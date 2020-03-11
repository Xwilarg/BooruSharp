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

        /// <summary>
        /// If of the tag
        /// </summary>
        public readonly int id;

        /// <summary>
        /// Name of the tag
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Type of the tag (character, copyright, etc...)
        /// </summary>
        public readonly TagType type;

        /// <summary>
        /// Number of occurences of the tag
        /// </summary>
        public readonly int count;
    }
}
