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
    [RequireContext(ContextType.Guild)]
    [Permissions(AccessLevel.ServerAdmin)]
    public class Admin : ModuleBase
    {
        [Command("kick"), Summary("Kicks a user from a guild.")]
        [Name("kick `<@user>` `<reason>` <(opt)")]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ModUtil.KickUserAsync(user, Context, "");
            else
                await ModUtil.KickUserAsync(user, Context, reason);
        }

        [Command("ban"), Summary("Bans a user from a guild.")]
        [Name("ban `<@user>` `<reason>` <(opt)")]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ModUtil.BanUserAsync(user, Context, "");
            else
                await ModUtil.BanUserAsync(user, Context, reason);
        }   
    }

    [Group("clean"), Summary("Clean")]
    [Permissions(AccessLevel.ServerAdmin)]
    [RequireContext(ContextType.Guild)]
    public class Cleanup : ModuleBase
    {
        [Command, Summary("Cleans all user messages from a channel")]
        [Name("clean `<num>` (limit 100)")]
        public async Task AllClean(int amt = 5)
        {
            await Context.Message.DeleteAsync();
            var _messages = await Context.Channel.GetMessagesAsync(amt).Flatten().ConfigureAwait(false);
            await Context.Channel.DeleteMessagesAsync(_messages).ConfigureAwait(false);
            var reply = await ReplyAsync($":grey_exclamation: Deleted {amt} messages.");
            await TimedMessage(reply);
        }

        [Command("user"), Summary("Cleans a specified users messages from the channel.")]
        [Name("clean user `<@user>` `<num>` (limit 100)")]
        public async Task UserClean(SocketGuildUser user, int amt = 5)
        {
            await Context.Message.DeleteAsync();
            var _messages = (await Context.Channel.GetMessagesAsync(amt).Flatten()).Where(x => x.Author.Id == user.Id);
            await Context.Channel.DeleteMessagesAsync(_messages).ConfigureAwait(false);
            var reply = await ReplyAsync($":grey_exclamation: Deleted {user.Username}'s last {amt} messages.");
            await TimedMessage(reply);
        }

        [Command("bot"), Summary("Cleans bot messages from a channel.")]
        [Name("clean bot `<num>` (limit 100)")]
        public async Task BotClean(int amt = 5)
        {
            await Context.Message.DeleteAsync();
            var _messages = (await Context.Channel.GetMessagesAsync(amt).Flatten()).Where(x => x.Author.IsBot);
            await Context.Channel.DeleteMessagesAsync(_messages).ConfigureAwait(false);
            var reply = await ReplyAsync($":grey_exclamation: Deleted the last {amt} bot messages.");
            await TimedMessage(reply);
        }

        private async Task TimedMessage(IMessage message, int ms = 5000)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }
}
