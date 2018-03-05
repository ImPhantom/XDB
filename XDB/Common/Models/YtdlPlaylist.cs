using System.Collections.Generic;
using Newtonsoft.Json;

namespace XDB.Common.Models
{
    public class YtdlPlaylist
    {
        [JsonProperty("entries")]
        public List<Video> Videos { get; set; }
    }

    public class Video
    {
        [JsonProperty("display_id")]
        public string VideoId { get; set; }

        [JsonProperty("uploader")]
        public string UploadedBy { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("url")]
        public string PlayUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("formats")]
        public List<Format> Formats { get; set; }
    }

    public class Format
    {
        [JsonProperty("acodec")]
        public string AudioCodec { get; set; }

        [JsonProperty("ext")]
        public string Extension { get; set; }

        [JsonProperty("url")]
        public string PlayUrl { get; set; }
    }
}
