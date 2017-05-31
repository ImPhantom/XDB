using System;

namespace XDB.Common.Models
{
    public class TempBan
    {
        public ulong GuildId { get; set; }
        public ulong BannedUserId { get; set; }
        public ulong AdminUserId { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime UnbanTime { get; set; }
    }
}
