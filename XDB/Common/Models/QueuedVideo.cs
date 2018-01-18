using Newtonsoft.Json;

namespace XDB.Common.Models
{
    public class QueuedVideo
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        public string VideoId { get; set; }
    }
}
