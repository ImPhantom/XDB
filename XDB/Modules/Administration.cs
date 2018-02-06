using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common;
using XDB.Common.Attributes;
using XDB.Common.Types;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Administration")]
    [RequireContext(ContextType.Guild)]
    [RequirePermission(Permission.GuildAdmin)]
    public class Administration : XenoBase
    {
        [Command("ban", RunMode = RunMode.Async), Summary("Bans a user from a guild.")]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ReplyAsync(":heavy_multiplication_x: **A reason is required.**");
            else
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await Logging.TryLoggingAsync($":hammer: **{Context.User.Username}** has banned {user.Mention}\n**Reason:** `{reason}`");
                await dmChannel.SendMessageAsync($":small_blue_diamond: You were banned from **{Context.Guild.Name}**\n**Reason:** `{reason}`");
                await Context.Guild.AddBanAsync(user, reason: $"{reason} (banned by: {Context.User.Username})");
                await ReplyThenRemoveAsync(":ok_hand:");
            }
        }

        [Command("clean", RunMode = RunMode.Async), Summary("Cleans all user messages from a channel")]
        public async Task Clean(int amount = 10)
        {
            await Context.Message.DeleteAsync();
            var messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            if (amount <= 100)
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            else
                foreach (var message in messages)
                    await message.DeleteAsync().ConfigureAwait(false);
        }

        [Command("clean", RunMode = RunMode.Async), Summary("Cleans a users messages from a channel")]
        public async Task CleanUser(IGuildUser user, int amount = 10)
        {
            await Context.Message.DeleteAsync();
            var messages = (await Context.Channel.GetMessagesAsync(amount).FlattenAsync()).Where(x => x.Author.Id == user.Id);
            if (amount <= 100)
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
            else
                foreach (var message in messages)
                    await message.DeleteAsync().ConfigureAwait(false);
        }

        [Command("ignore"), Summary("Adds a channel to the list of ignored channels.")]
        public async Task Ignore(ulong channelid)
        {
            var cfg = Config.Load();
            if (!cfg.IgnoredChannels.Contains(channelid))
            {
                cfg.IgnoredChannels.Add(channelid);
                cfg.Save();
                var channel = Context.Guild.GetTextChannel(channelid);
                await Logging.TryLoggingAsync($":heavy_check_mark:  **{Context.User.Username}#{Context.User.Discriminator}** added {channel.Mention} (`{channelid}`) to the ignored channels list.");
            } else
            {
                cfg.IgnoredChannels.Remove(channelid);
                cfg.Save();
                var channel = Context.Guild.GetTextChannel(channelid);
                await Logging.TryLoggingAsync($":heavy_multiplication_x:  **{Context.User.Username}#{Context.User.Discriminator}** removed {channel.Mention} (`{channelid}`) from the ignored channels list.");
            }
        }

        [Command("ignored"), Summary("Displays all ignored channels.")]
        public async Task Ignored()
        {
            var ignored = new StringBuilder();
            if (!Config.Load().IgnoredChannels.Any()) { await ReplyAsync(":black_medium_small_square:  There are no ignored channels!"); return; }
            foreach (var channel in Config.Load().IgnoredChannels)
            {
                var chan = Context.Guild.GetTextChannel(channel);
                ignored.AppendLine($"**#{chan.Name}** -- `{chan.Id}`");
            }
            await ReplyAsync($":heavy_check_mark:  Currently ignored channels:\n{ignored.ToString()}");
        }

        [Command("welcome"), Summary("Sets the welcome message.")]
        public async Task Welcome([Remainder] string message)
        {
            var cfg = Config.Load();
            cfg.WelcomeMessage = message;
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You changed the welcome message to: \n\n{message}");
        }
    }
}
