﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Models;
using XDB.Common.Types;
using XDB.Utilities;
using System.Threading;
using Humanizer;

namespace XDB.Services
{
    public class CheckingService
    {
        private readonly Timer _timer;

        private DiscordSocketClient _client;
        private ModerationService _moderation;
        private RemindService _remind;
        public List<Mute> Mutes { get; set; }
        public List<Reminder> Reminders { get; set; }
        public List<TempBan> TempBans { get; set; }

        public Task FetchChecksAsync()
        {
            Mutes = new List<Mute>(_moderation.FetchActiveMutes());
            Reminders = new List<Reminder>(_remind.FetchActiveReminders());
            TempBans = new List<TempBan>(_moderation.FetchActiveTempBans());
            return Task.CompletedTask;
        }

        public async Task CheckForExpiredMutesAsync()
        {
            var mutes = new List<Mute>();
            foreach (var mute in Mutes.Where(x => DateTime.Compare(DateTime.UtcNow, x.UnmuteTime) > 0 && x.IsActive))
            {
                var guild = _client.GetGuild(mute.GuildId);
                var user = guild.GetUser(mute.UserId);
                if (user == null)
                {
                    _moderation.RemoveMute(mute);
                    Mutes.Remove(mute);
                    BetterConsole.LogError("Muting", "Error trying to unmute unknown user, mute removed.");
                    return;
                }

                var role = guild.GetRole(Config.Load().MutedRoleId);
                await user.RemoveRolesAsync(new SocketRole[] { role });
                await user.ModifyAsync(x => x.Mute = false);
                await Logging.TryLoggingAsync($":alarm_clock: `{user.Username}#{user.Discriminator}`'s mute for `{mute.Reason}` has expired.");
                _moderation.RemoveMute(mute);
                mutes.Add(mute);
            }
            foreach (var mute in mutes)
                Mutes.Remove(mute);
        }

        public async Task CheckForExpiredRemindersAsync()
        {
            var reminders = new List<Reminder>();
            foreach (var reminder in Reminders.Where(x => DateTime.Compare(DateTime.UtcNow, x.RemindTime) > 0))
            {
                var guild = _client.GetGuild(reminder.GuildId);
                var channel = guild.GetChannel(reminder.ChannelId) as SocketTextChannel;
                var user = guild.GetUser(reminder.UserId);
                if(user == null)
                {
                    _remind.RemoveReminder(reminder);
                    Reminders.Remove(reminder);
                    BetterConsole.LogError("Remind", "Error trying to remind an unknown user, reminder deleted.");
                    return;
                }

                if (string.IsNullOrEmpty(reminder.Reason))
                    await channel?.SendMessageAsync($":mega: {user?.Mention} Timer is up!");
                else
                    await channel?.SendMessageAsync($":mega: {user?.Mention} Timer is up! You need to: `{reminder.Reason}`");
                _remind.RemoveReminder(reminder);
                reminders.Add(reminder);
            }
            foreach (var reminder in reminders)
                Reminders.Remove(reminder);
        }

        public async Task CheckForExpiredBansAsync()
        {
            var _bans = new List<TempBan>();
            foreach (var ban in TempBans.Where(x => DateTime.Compare(DateTime.UtcNow, x.UnbanTime) > 0))
            {
                var guild = _client.GetGuild(ban.GuildId);
                var bans = await guild.GetBansAsync();

                if (bans.Any(x => x.User.Id == ban.BannedUserId))
                {
                    var user = bans.First(x => x.User.Id == ban.BannedUserId).User;
                    await guild.RemoveBanAsync(ban.BannedUserId);
                    await Logging.TryLoggingAsync($":clock3:  **{user.Username}#{user.Discriminator}**'s `{(ban.UnbanTime - ban.Timestamp).Humanize().Singularize()}` ban has expired.");
                    _moderation.RemoveTemporaryBan(ban);
                    _bans.Add(ban);
                } else
                {
                    _moderation.RemoveTemporaryBan(ban);
                    TempBans.Remove(ban);
                    BetterConsole.LogError("Tempban", "Error trying to unban user (ban was not found)");
                    return;
                }
            }
            foreach (var ban in _bans)
                TempBans.Remove(ban);
        }

        public CheckingService(DiscordSocketClient client, ModerationService moderation, RemindService remind)
        {
            _client = client;
            _moderation = moderation;
            _remind = remind;
            Mutes = new List<Mute>();
            Reminders = new List<Reminder>();
            TempBans = new List<TempBan>();

            // Start the timer
            _timer = new Timer(async _ =>
            {
                await CheckForExpiredMutesAsync();
                await CheckForExpiredRemindersAsync();
                await CheckForExpiredBansAsync();
            }, null, TimeSpan.FromSeconds(45), TimeSpan.FromSeconds(30));
        }
    }
}
