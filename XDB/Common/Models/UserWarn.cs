using System.Collections.Generic;

namespace XDB.Common.Models
{
    public class UserWarn
    {
        public ulong WarnedUser { get; set; }
        public List<string> WarnReason { get; set; }
    }
}
