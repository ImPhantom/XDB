using System.Collections.Generic;
using Newtonsoft.Json;

namespace XDB.Common.Models
{
    public class VideoInfo
    {
        [JsonProperty(PropertyName = "items")]
        public List<YTItem> Items { get; set; }
    }

    public class YTItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "contentDetails")]
        public YTContentDetails Details { get; set; }
    }

    public class YTContentDetails
    {
        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }
    }
}
