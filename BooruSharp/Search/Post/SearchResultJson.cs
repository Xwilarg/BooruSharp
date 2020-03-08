using Newtonsoft.Json;

namespace BooruSharp.Search.Post
{
    internal class SearchResultJson
    {
        [JsonProperty("file_url")]
        public string fileUrl
        {
            set { fileUrl = value; }
            get { return fileUrl ?? fileUrl2; }
        }

        [JsonProperty("file-url")]
        public string fileUrl2;

        [JsonProperty("preview_url")]
        public string previewUrl
        {
            set { previewUrl = value; }
            get { return previewUrl ?? previewUrl2; }
        }

        [JsonProperty("preview-file-url")]
        public string previewUrl2;

        [JsonProperty]
        public string rating;

        [JsonProperty]
        public string tags
        {
            set { tags = value; }
            get { return tags ?? tags2; }
        }

        [JsonProperty("tag-string")]
        public string tags2;

        [JsonProperty]
        public int id;

        [JsonProperty("file_size")]
        public int? fileSize
        {
            set { fileSize = value; }
            get { return fileSize ?? fileSize2; }
        }

        [JsonProperty("file-size")]
        public int? fileSize2;

        public int? height
        {
            set { height = value; }
            get { return height ?? height2; }
        }

        [JsonProperty("image-height")]
        public int? height2;

        public int? width
        {
            set { width = value; }
            get { return width ?? width2; }
        }

        [JsonProperty("image-width")]
        public int? width2;

        [JsonProperty("preview_height")]
        public int? previewHeight;

        [JsonProperty("preview_width")]
        public int? previewWidth;

        [JsonProperty("created_at")]
        public string createdAt
        {
            set { createdAt = value; }
            get { return createdAt ?? createdAt2; }
        }

        [JsonProperty("created-at")]
        public string createdAt2;

        [JsonProperty]
        public string source;

        [JsonProperty]
        public string score;
    }
}
