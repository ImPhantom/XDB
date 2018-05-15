using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Humanizer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common;
using XDB.Common.Attributes;
using XDB.Common.Models;
using XDB.Common.Types;
using XDB.Services;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Moderation")]
    [RequirePermission(Permission.XDBAdministrator)]
    [RequireContext(ContextType.Guild)]
    public class Moderation : XenoBase
    {
        private ModerationService _moderation;
        private CheckingService _checking;

        [Command("kick", RunMode = RunMode.Async), Summary("Kicks a user from a guild.")]
        public async Task Kick(SocketGuildUser user, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ReplyAsync(":heavy_multiplication_x:  **A reason is required.**");
            else
            {
                var dmChannel = await user.GetOrCreateDMChannelAsync();
                await Logging.TryLoggingAsync($":heavy_check_mark:  **{Context.User.Username}** has kicked {user.Mention}\n**Reason:** `{reason}`");
                await dmChannel.SendMessageAsync($":small_blue_diamond: You were kicked from **{Context.Guild.Name}**\n**Reason:** `{reason}`");
                await user.KickAsync($"{reason} (kicked by: {Context.User.Username})");
                await ReplyOkReactionAsync();
            }
        }

        [Command("tempban", RunMode = RunMode.Async), Summary("Temporarily bans a user from a guild.")]
        public async Task TempBan(SocketGuildUser user, TimeSpan length, [Remainder] string reason = "")
        {
            if (string.IsNullOrEmpty(reason))
                await ReplyAsync(":heavy_multiplication_x: **A reason is required.**");
            else
            {
                var dm = await user.GetOrCreateDMChannelAsync();
                await Logging.TryLoggingAsync($":hammer:  **{Context.User.Username}#{Context.User.Discriminator}** has banned `{user.Username}#{user.Discriminator}` for __{length.Humanize()}__\n**Reason:** `{reason}`");
                await dm.SendMessageAsync($":hammer:  You have been temporarily banned from **{Context.Guild.Name}** for: `{length.Humanize()}`\n**Reason:** {reason}");
                var ban = new TempBan()
                {
                    GuildId = Context.Guild.Id,
                    BannedUserId = user.Id,
                    AdminUserId = Context.User.Id,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow,
                    UnbanTime = DateTime.UtcNow.Add(length)
                };
                _moderation.AddTemporaryBan(ban);
                _checking.TempBans.Add(ban);
                await Context.Guild.AddBanAsync(user, reason: $"{reason} (temp-banned by: {Context.User.Username})");

                await Context.Message.DeleteAsync();
                await ReplyOkReactionAsync();
            }
        }

        [Command("tempbans")]
        public async Task TempBans()
        {
            var tempbans = _moderation.FetchActiveTempBans();
            var str = new StringBuilder();
            if (tempbans.Any())
            {
                foreach (var ban in tempbans)
                    str.Append($"<@{ban.BannedUserId}> ({(ban.UnbanTime - ban.Timestamp).Humanize()})\n**Reason:** {ban.Reason}\n**Ends in:** `{(ban.UnbanTime - DateTime.UtcNow).Humanize(2)}`");
                await ReplyAsync("", embed: new EmbedBuilder().WithColor(Xeno.RandomColor()).WithTitle("Temporary Bans").WithDescription(str.ToString()).Build());
            }
            else
                await ReplyAsync("", embed: Xeno.InfoEmbed("No active temporary bans found."));
        }

        [Command("mute", RunMode = RunMode.Async), Summary("Mutes a user")]
        public async Task Mute(SocketGuildUser user, TimeSpan unmuteTime, MuteType type, [Remainder] string reason = "n/a")
        {
            var dm = await user.GetOrCreateDMChannelAsync();
            await _moderation.ApplyMuteAsync(user, type);

            var mute = new Mute()
            {
                GuildId = Context.Guild.Id,
                UserId = user.Id,
                Reason = reason,
                Type = type,
                Timestamp = DateTime.UtcNow,
                UnmuteTime = DateTime.UtcNow.Add(unmuteTime),
                IsActive = true
            };
            _moderation.AddMute(mute);
            _checking.Mutes.Add(mute);
            await ReplyOkReactionAsync();
            await Logging.TryLoggingAsync($":mute: **{user.Username}#{user.Discriminator}** has recieved a `{unmuteTime.Humanize()}` mute by {Context.User.Username} for:\n `{mute.Reason}`");
            await dm.SendMessageAsync($":mute: (`{Context.Guild.Name}`) You have been muted for `{unmuteTime.Humanize()}` by **{Context.User.Username}**\n__Reason:__ {reason}");
        }

        [Command("unmute", RunMode = RunMode.Async), Summary("Un-mutes a user")]
        public async Task UnMute(SocketGuildUser user)
        {
            var muteRole = user.Guild.GetRole(Config.Load().MutedRoleId);
            var dm = await user.GetOrCreateDMChannelAsync();
            await user.RemoveRoleAsync(muteRole);
            await user.ModifyAsync(x => x.Mute = false);

            var mutes = _moderation.FetchMutes();
            foreach (var mute in mutes)
            {
                if (mute.UserId == user.Id)
                {
                    _moderation.RemoveMute(mute);
                    if (_checking.Mutes.Contains(mute))
                        _checking.Mutes.Remove(mute);
                }
            }
            await ReplyOkReactionAsync();
            await Logging.TryLoggingAsync($":loud_sound: **{user.Username}#{user.Discriminator}** has been unmuted by {Context.User.Username}.");
            await dm.SendMessageAsync($":loud_sound: (`{Context.Guild.Name}`) You have been unmuted by **{Context.User.Username}**!");
        }

        [Command("warn"), Summary("Warns a user for doing something wrong")]
        public async Task WarnUser(SocketGuildUser user, [Remainder] string reason)
        {
            await _moderation.WarnUserAsync(user, reason);
            await ReplyOkReactionAsync();
            await Logging.TryLoggingAsync($":heavy_check_mark: `{user.Username}#{user.Discriminator}` has been warned by `{Context.User.Username}#{Context.User.Discriminator}` for:\n`{reason}`");
            await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync($":heavy_multiplication_x: You have been warned by **{Context.User.Username}#{Context.User.Discriminator}** in **{Context.Guild.Name}** for:\n`{reason}`");
        }

        [Command("removewarn"), Summary("Removes a warn from a specified user.")]
        public async Task RemoveWarn(SocketGuildUser user, int index)
        {
            var warnings = _moderation.FetchWarnings();
            if (warnings.TryGetValue(user.Id, out List<string> _warnings))
            {
                var warn = _warnings.ElementAt(index - 1);
                _warnings.Remove(warn);
                await Xeno.SaveJsonAsync(Xeno.WarningsPath, JsonConvert.SerializeObject(warnings));
                await ReplyOkReactionAsync();
                await Logging.TryLoggingAsync($":heavy_check_mark: `{Context.User.Username}#{Context.User.Discriminator}` has removed `{user.Username}#{user.Discriminator}`'s warn for:\n`{warn}`");
            }
            else
                await SendErrorEmbedAsync($"`{ user.Username}#{user.Discriminator}` has no warnings.");
        }

        [Command("warns"), Summary("Views all warns for a specified user")]
        public async Task ViewWarns(SocketGuildUser user)
        {
            var warnings = _moderation.FetchWarnings();
            if (warnings.TryGetValue(user.Id, out List<string> _warnings))
            {
                var str = new StringBuilder();
                var count = 1;
                _warnings.ForEach(x =>
                {
                    str.Append($"{count}. {x}\n");
                    count++;
                });
                await ReplyAsync($":small_blue_diamond: `{user.Username}#{user.Discriminator}` has {_warnings.Count()} warnings:\n```{str.ToString()}```");
            }
            else
                await SendErrorEmbedAsync($"`{user.Username}#{user.Discriminator}` has no warnings.");
        }


        public Moderation(ModerationService moderation, CheckingService checking)
        {
            _moderation = moderation;
            _checking = checking;
        }
    }
}
