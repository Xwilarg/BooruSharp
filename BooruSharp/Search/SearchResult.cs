namespace BooruSharp.Search
{
    public struct SearchResult
    {
        public SearchResult(string fileUrl, string previewUrl, Rating rating)
        {
            this.fileUrl = fileUrl;
            this.previewUrl = previewUrl;
            this.rating = rating;
        }
        public readonly string fileUrl;
        public readonly string previewUrl;
        public readonly Rating rating;
    }
}
