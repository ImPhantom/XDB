using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Humanizer;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Models;
using XDB.Common.Types;
using XDB.Services;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Moderation")]
    [RequireContext(ContextType.Guild)]
    [Permissions(AccessLevel.FullAdmin)]
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("leave"), Summary("Forces the bot to leave its current guild.")]
        public async Task Leave()
            => await Context.Guild.LeaveAsync();

        [Command("ban", RunMode = RunMode.Async), Summary("Bans a user from a guild.")]
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

        /*public async Task LockChannel()
        {
            var permissions = new OverwritePermissions(66560, 456768);
            var users = Context.Channel.GetUsersAsync().ToEnumerable();
            var channel = Context.Channel as SocketTextChannel;
            channel.AddPermissionOverwriteAsync()
        }*/
    }

    [Summary("Administration")]
    [Permissions(AccessLevel.Administrators)]
    [RequireContext(ContextType.Guild)]
    public class Administration : ModuleBase<SocketCommandContext>
    {
        [Command("kick", RunMode = RunMode.Async), Summary("Kicks a user from a guild.")]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ReplyAsync(":heavy_multiplication_x:  **A reason is required.**");
            else
                await ModUtil.KickUserAsync(user, Context, reason);
        }

        [Command("tempban", RunMode = RunMode.Async), Summary("Temporarily bans a user from a guild.")]
        public async Task TempBan(SocketGuildUser user, TimeSpan length, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ReplyAsync(":heavy_multiplication_x: **A reason is required.**");
            else
                await ModUtil.TempBanUserAsync(user, Context, length, reason);
        }
    }

    [Summary("Muting")]
    [Permissions(AccessLevel.Moderator)]
    [RequireContext(ContextType.Guild)]
    public class Muting : ModuleBase<SocketCommandContext>
    {
        [Command("mute", RunMode = RunMode.Async), Summary("Mutes a user")]
        public async Task Mute(SocketGuildUser user, TimeSpan unmuteTime, [Remainder] string reason = "n/a")
        {
            var muteRole = user.Guild.GetRole(Config.Load().MutedRoleId);
            await user.AddRoleAsync(muteRole);
            await user.ModifyAsync(x => x.Mute = true);

            var mute = new Mute()
            {
                GuildId = Context.Guild.Id,
                UserId = user.Id,
                Reason = reason,
                Timestamp = DateTime.UtcNow,
                UnmuteTime = DateTime.UtcNow.Add(unmuteTime),
                IsActive = true
            };
            MutingService.AddMute(mute);
            CheckingService.Mutes.Add(mute);
            var dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($":mute: You have been muted for **{unmuteTime.Humanize()}** due to: \n`{reason}`");
            await Logging.TryLoggingAsync($":mute: **{user.Username}#{user.Discriminator}** has been muted by {Context.User.Username} for:\n `{mute.Reason}`");
            var reply = await ReplyAsync(":ok_hand:");
            await TimedMessage(reply);
        }

        [Command("unmute", RunMode = RunMode.Async), Summary("Un-mutes a user")]
        public async Task UnMute(SocketGuildUser user)
        {
            var muteRole = user.Guild.GetRole(Config.Load().MutedRoleId);
            await user.RemoveRoleAsync(muteRole);
            await user.ModifyAsync(x => x.Mute = false);

            var mutes = MutingService.FetchMutes();
            foreach (var mute in mutes)
            {
                if (mute.UserId == user.Id)
                {
                    MutingService.RemoveMute(mute);
                    if (CheckingService.Mutes.Contains(mute))
                        CheckingService.Mutes.Remove(mute);
                }
            }
            var dm = await user.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync($":loud_sound: You have been unmuted by **{Context.User.Username}**!");
            await Logging.TryLoggingAsync($":loud_sound: **{user.Username}#{user.Discriminator}** has been unmuted by {Context.User.Username}.");
            var reply = await ReplyAsync(":ok_hand:");
            await TimedMessage(reply);
        }

        private static async Task TimedMessage(IMessage message, int ms = 2500)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }

    [Summary("Clean")]
    [Permissions(AccessLevel.Administrators)]
    [RequireContext(ContextType.Guild)]
    public class Cleanup : ModuleBase<SocketCommandContext>
    {
        [Command("clean", RunMode = RunMode.Async), Summary("Cleans all user messages from a channel")]
        public async Task Clean(int amount = 5)
        {
            amount++;
            var channel = Context.Channel as SocketTextChannel;
            var messages = channel.GetMessagesAsync(amount);

            await messages.ForEachAsync(async m =>
            {
                try
                {
                    await channel.DeleteMessagesAsync(m);
                }
                catch (ArgumentOutOfRangeException)
                {
                    await ReplyAsync(":small_blue_diamond: Some messages older than 2 weeks, Try cleaning a lesser amount.");
                    return;
                }
            });
            await Context.Message.DeleteAsync();
        }
    }
}
