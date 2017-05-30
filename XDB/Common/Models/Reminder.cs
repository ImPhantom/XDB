using System;
using System.Collections.Generic;
using System.Text;

namespace XDB.Common.Models
{
    public class Reminder
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime RemindTime { get; set; }
    }
}
