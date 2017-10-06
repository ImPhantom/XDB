using System;
using System.Collections.Generic;
using System.Text;

namespace XDB.Common.Models
{
    public class ChannelTodo
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public List<string> Items { get; set; }
    }
}
