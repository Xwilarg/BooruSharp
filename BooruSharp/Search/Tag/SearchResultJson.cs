using Newtonsoft.Json;

namespace BooruSharp.Search.Tag
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        [JsonProperty]
        public string id;

        private string nameInternal;

        [JsonProperty]
        public string name
        {
            set { nameInternal = value; }
            get { return nameInternal ?? name2; }
        }

        [JsonProperty("tag")]
        public string name2;

        private string typeInternal;

        [JsonProperty]
        public string type
        {
            set { typeInternal = value; }
            get { return typeInternal ?? type2; }
        }

        [JsonProperty("category")]
        public string type2;

        private string countInternal;

        [JsonProperty]
        public string count
        {
            set { countInternal = value; }
            get { return countInternal ?? count2; }
        }

        [JsonProperty("post_count")]
        public string count2;
    }
#pragma warning restore 0649
}
