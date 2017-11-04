using Newtonsoft.Json;

namespace XDB.Common.Models
{
    public class CachedVideo
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }

        [JsonProperty(PropertyName = "webpage_url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "display_id")]
        public string VideoId { get; set; }

        [JsonProperty(PropertyName = "_filename")]
        public string Filename { get; set; }
    }
}
