using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XDB.Common.Enums
{
    public enum AccessLevel
    {
        Blocked,
        User,
        ServerMod,
        ServerAdmin,
        ServerOwner,
        BotOwner
    }
}
