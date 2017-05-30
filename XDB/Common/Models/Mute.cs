using System;
using System.ComponentModel.DataAnnotations;

namespace XDB.Common.Models
{
    public class Mute
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime UnmuteTime { get; set; }
    }
}
