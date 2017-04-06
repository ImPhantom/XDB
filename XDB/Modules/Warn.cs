using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;
using XDB.Common.Types;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Warn")]
    [RequireContext(ContextType.Guild)]
    public class Warn : ModuleBase
    {
        [Command("warns"), Summary("Views your personal warns.")]
        public async Task Warns()
            => await Warning.GetUserWarns(Context, Context.User as SocketGuildUser);

        [Command("warns"), Summary("Checks a specified users warnings.")]
        [Name("warns `<@user>`")]
        public async Task Warns(SocketGuildUser user)
            => await Warning.GetUserWarns(Context, user);

        [Command("warn"), Summary("Warns a specified user.")]
        [Name("warn `<@user>` `<reason>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddWarn(SocketGuildUser user, [Remainder] string reason)
            => await Warning.WarnUserAsync(Context, user, reason);

        [Command("removewarn"), Summary("Removes a warn from a specified user by index.")]
        [Name("removewarn `<@user>` `<index>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task RemoveWarn(SocketGuildUser user, int index)
            => await Warning.RemoveWarnAsync(Context, user, index);
    }
}
