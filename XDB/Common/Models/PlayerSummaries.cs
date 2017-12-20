using Newtonsoft.Json;
using System.Collections.Generic;

namespace XDB.Common.Models
{
    public class PlayerSummaries
    {
        [JsonProperty("players")]
        public List<PlayerSummary> Players { get; set; }
    }

    public class PlayerSummariesResponse
    {
        [JsonProperty("response")]
        public PlayerSummaries Response { get; set; }
    }

    public class PlayerSummary
    {
        [JsonProperty("steamid")]
        public ulong Steam64Id { get; set; }

        [JsonProperty("personaname")]
        public string PersonaName { get; set; }

        [JsonProperty("profileurl")]
        public string ProfileUrl { get; set; }

        [JsonProperty("avatar")]
        public string SmallSizeAvatarUrl { get; set; }

        [JsonProperty("avatarmedium")]
        public string MediumSizeAvatarUrl { get; set; }

        [JsonProperty("avatarfull")]
        public string FullSizeAvatarUrl { get; set; }
    }
}
