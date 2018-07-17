namespace BooruSharp.Search
{
    public struct SearchResult
    {
        public SearchResult(string fileUrl, string previewUrl, Rating rating, string[] tags)
        {
            this.fileUrl = fileUrl;
            this.previewUrl = previewUrl;
            this.rating = rating;
            this.tags = tags;
        }
        public readonly string fileUrl;
        public readonly string previewUrl;
        public readonly Rating rating;
        public readonly string[] tags;
    }
}
