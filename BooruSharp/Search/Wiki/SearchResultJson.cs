using Newtonsoft.Json;

namespace BooruSharp.Search.Wiki
{
#pragma warning disable 0649
    internal class SearchResultJson
    {
        [JsonProperty]
        public int id;

        [JsonProperty]
        public string title;

        private string creationInternal;

        [JsonProperty("created_at")]
        public string creation
        {
            set { creationInternal = value; }
            get { return creationInternal ?? creation2; }
        }

        [JsonProperty("created-at")]
        public string creation2;

        private string lastUpdateInternal;

        [JsonProperty("updated_at")]
        public string lastUpdate
        {
            set { lastUpdateInternal = value; }
            get { return lastUpdateInternal ?? lastUpdate2; }
        }

        [JsonProperty("updated-at")]
        public string lastUpdate2;

        [JsonProperty]
        public string body;
    }
#pragma warning restore 0649
}
