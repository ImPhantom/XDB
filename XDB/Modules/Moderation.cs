using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Types;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Moderation")]
    [RequireContext(ContextType.Guild)]
    [Permissions(AccessLevel.ServerAdmin)]
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("leave"), Summary("Forces the bot to leave its current guild.")]
        public async Task Leave()
            => await Context.Guild.LeaveAsync();

        [Command("kick"), Summary("Kicks a user from a guild.")]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ModUtil.KickUserAsync(user, Context, "");
            else
                await ModUtil.KickUserAsync(user, Context, reason);
        }

        [Command("ban"), Summary("Bans a user from a guild.")]
        public async Task Ban(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ModUtil.BanUserAsync(user, Context, "");
            else
                await ModUtil.BanUserAsync(user, Context, reason);
        }

        [Command("filters"), Summary("Displays all words in the word filter.")]
        public async Task FilterWords()
        {
            var words = new StringBuilder();
            if (!Config.Load().Words.Any()) { await ReplyAsync(":black_medium_small_square:  There are no words in the word filter."); return; }
            foreach (var word in Config.Load().Words)
            {
                words.AppendLine(word);
            }
            await ReplyAsync($":heavy_check_mark:  Currently Blacklisted Words:\n```\n{words.ToString()}\n```");
        }

        [Command("addword"), Summary("Adds a word/string to the word filter.")]
        public async Task FilterAdd([Remainder] string str)
        {
            var cfg = Config.Load();
            if (cfg.Words.Contains(str)) { await ReplyAsync($":black_medium_small_square:  There is already a string matching `{str}` in the word filter."); return; }
            cfg.Words.Add(str.ToLower());
            cfg.Save();
            await ReplyAsync($":heavy_check_mark:  You added `{str}` to the word filter!");
        }

        [Command("delword"), Summary("Removes a word/string from the word filter.")]
        public async Task FilterDel([Remainder] string str)
        {
            var cfg = Config.Load();
            if (!cfg.Words.Contains(str)) { await ReplyAsync($":black_medium_small_square:  There is no string matching `{str}` in the word filter."); return; }
            cfg.Words.Remove(str.ToLower());
            cfg.Save();
            await ReplyAsync($":heavy_multiplication_x:  You removed `{str}` from the word filter!");
        }

        [Command("ignore"), Summary("Adds a channel to the list of ignored channels.")]
        public async Task Ignore(ulong channelid)
        {
            var cfg = Config.Load();
            if (cfg.IgnoredChannels.Contains(channelid)) { await ReplyAsync(":black_medium_small_square:  That channel is already ignored."); return; }
            cfg.IgnoredChannels.Add(channelid);
            cfg.Save();
            var channel = Context.Guild.GetTextChannel(channelid);
            await Logging.TryLoggingAsync($":heavy_check_mark:  **{Context.User.Username}#{Context.User.Discriminator}** added {channel.Mention} (`{channelid}`) to the ignored channels list.");
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

        [Command("unignore"), Summary("Removes a channel from the list of ignored channels.")]
        public async Task DelIgnore(ulong channelid)
        {
            var cfg = Config.Load();
            if (!cfg.IgnoredChannels.Contains(channelid)) { await ReplyAsync(":black_medium_small_square:  That channel is not ignored."); return; }
            cfg.IgnoredChannels.Remove(channelid);
            cfg.Save();
            var channel = Context.Guild.GetTextChannel(channelid);
            await Logging.TryLoggingAsync($":heavy_multiplication_x:  **{Context.User.Username}#{Context.User.Discriminator}** removed {channel.Mention} (`{channelid}`) from the ignored channels list.");
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

    [Group("clean"), Summary("Clean")]
    [Permissions(AccessLevel.ServerAdmin)]
    [RequireContext(ContextType.Guild)]
    public class Cleanup : ModuleBase<SocketCommandContext>
    {
        [Command, Summary("Cleans all user messages from a channel")]
        public async Task AllClean(int amt = 5)
        {
            amt++;
            await Context.Message.DeleteAsync();
            var _messages = await Context.Channel.GetMessagesAsync(amt).Flatten().ConfigureAwait(false);
            await Context.Channel.DeleteMessagesAsync(_messages).ConfigureAwait(false);
            var reply = await ReplyAsync($":grey_exclamation: Deleted {amt} messages.");
            await TimedMessage(reply);
        }

        [Command("user"), Summary("Cleans a specified users messages from the channel.")]
        public async Task UserClean(SocketGuildUser user, int amt = 5)
        {
            amt++;
            await Context.Message.DeleteAsync();
            var _messages = (await Context.Channel.GetMessagesAsync(amt).Flatten()).Where(x => x.Author.Id == user.Id);
            await Context.Channel.DeleteMessagesAsync(_messages).ConfigureAwait(false);
            var reply = await ReplyAsync($":grey_exclamation: Deleted {user.Username}'s last {amt} messages.");
            await TimedMessage(reply);
        }

        [Command("bot"), Summary("Cleans bot messages from a channel.")]
        public async Task BotClean(int amt = 5)
        {
            amt++;
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
