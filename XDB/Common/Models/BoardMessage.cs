using System;

namespace XDB.Common.Models
{
    public class BoardMessage
    {
        public string Message { get; set; }
        public ulong UserId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
