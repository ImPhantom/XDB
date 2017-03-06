using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Admin")]
    public class Admin : ModuleBase
    {
        [Command("cleanup")]
        [Name("cleanup `<num>` (limit 100)")]
        [Remarks("Cleans up specified amount of messages from channel.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Cleanup(int amt)
        {
            if (amt < 1)
                return;
            await Context.Message.DeleteAsync().ConfigureAwait(false);
            int lim = (amt < 100) ? amt : 100;
            var messages = (await Context.Channel.GetMessagesAsync(limit: lim).Flatten().ConfigureAwait(false));
            await Context.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("cleanup")]
        [Name("cleanup `<@user>` `<num>` (limit 100)")]
        [Remarks("Cleans up specified amount of messages from channel.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Cleanup(SocketGuildUser user, int amt)
        {
            if (amt < 1)
                return;

            if (user.Id == Context.User.Id)
                amt += 1;

            int lim = (amt < 100) ? amt : 100;
            var messages = (await Context.Channel.GetMessagesAsync(limit: lim).Flatten()).Where(m => m.Author.Id == user.Id);
            await Context.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);
        }

        [Command("kick")]
        [Name("kick `<@user>` `<reason>` <(opt)")]
        [Remarks("Kicks user from guild.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ModUtil.KickUserAsync(user, Context, "");
            else
                await ModUtil.KickUserAsync(user, Context, reason);
        }

        [Command("ban")]
        [Name("ban `<@user>` `<reason>` <(opt)")]
        [Remarks("Bans user from guild.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ModUtil.BanUserAsync(user, Context, "");
            else
                await ModUtil.BanUserAsync(user, Context, reason);
        }
    }
}
