using Newtonsoft.Json;

namespace BooruSharp.Search.Post
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        private string fileUrlInternal;

        [JsonProperty("file_url")]
        public string fileUrl
        {
            set { fileUrlInternal = value; }
            get { return fileUrlInternal ?? fileUrl2; }
        }

        [JsonProperty("file-url")]
        public string fileUrl2;

        private string previewUrlInternal;

        [JsonProperty("preview_url")]
        public string previewUrl
        {
            set { previewUrlInternal = value; }
            get { return previewUrlInternal ?? previewUrl2; }
        }

        [JsonProperty("preview-file-url")]
        public string previewUrl2;

        [JsonProperty]
        public string rating;

        private string tagsInternal;

        [JsonProperty]
        public string tags
        {
            set { tagsInternal = value; }
            get { return tagsInternal ?? tags2; }
        }

        [JsonProperty("tag-string")]
        public string tags2;

        [JsonProperty]
        public int id;

        private int? fileSizeInternal;

        [JsonProperty("file_size")]
        public int? fileSize
        {
            set { fileSizeInternal = value; }
            get { return fileSizeInternal ?? fileSize2; }
        }

        [JsonProperty("file-size")]
        public int? fileSize2;

        private int? heightInternal;

        public int? height
        {
            set { heightInternal = value; }
            get { return heightInternal ?? height2; }
        }

        [JsonProperty("image-height")]
        public int? height2;

        private int? widthInternal;

        public int? width
        {
            set { widthInternal = value; }
            get { return widthInternal ?? width2; }
        }

        [JsonProperty("image-width")]
        public int? width2;

        [JsonProperty("preview_height")]
        public int? previewHeight;

        [JsonProperty("preview_width")]
        public int? previewWidth;

        private string createdAtInternal;

        [JsonProperty("created_at")]
        public string createdAt
        {
            set { createdAtInternal = value; }
            get { return createdAtInternal ?? createdAt2; }
        }

        [JsonProperty("created-at")]
        public string createdAt2;

        [JsonProperty]
        public string source;

        [JsonProperty]
        public string score;
    }
#pragma warning restore 0649
}
