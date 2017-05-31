using Discord.WebSocket;
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
        private MutingService _muting;
        private RemindService _remind;
        private TempBanService _tempbans;
        public static List<Mute> Mutes { get; set; }
        public static List<Reminder> Reminders { get; set; }
        public static List<TempBan> TempBans { get; set; }

        public Task FetchChecksAsync()
        {
            Mutes = new List<Mute>(_muting.GetActiveMutes());
            Reminders = new List<Reminder>(_remind.GetActiveReminders());
            TempBans = new List<TempBan>(_tempbans.GetActiveBans());
            return Task.CompletedTask;
        }

        public async Task CheckForExpiredMutesAsync()
        {
            var mutes = new List<Mute>();
            foreach (var mute in Mutes.Where(x => DateTime.Compare(DateTime.UtcNow, x.UnmuteTime) > 0 && x.IsActive))
            {
                var guild = _client.GetGuild(mute.GuildId);
                var user = guild.GetUser(mute.UserId);

                var role = guild.GetRole(Config.Load().MutedRoleId);
                await user.RemoveRolesAsync(new SocketRole[] { role });
                await Logging.TryLoggingAsync($":alarm_clock: `{user.Username}#{user.Discriminator}`'s mute for `{mute.Reason}` has expired.");
                MutingService.RemoveMute(mute);
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

                if (string.IsNullOrEmpty(reminder.Reason))
                    await channel?.SendMessageAsync($":mega: {user?.Mention} Timer is up!");
                else
                    await channel?.SendMessageAsync($":mega: {user?.Mention} Timer is up! You need to: `{reminder.Reason}`");
                RemindService.RemoveReminder(reminder);
                reminders.Add(reminder);
            }
            foreach (var reminder in reminders)
                Reminders.Remove(reminder);
        }

        public async Task CheckForExpiredBansAsync()
        {
            var bans = new List<TempBan>();
            foreach (var ban in TempBans.Where(x => DateTime.Compare(DateTime.UtcNow, x.UnbanTime) > 0))
            {
                var guild = _client.GetGuild(ban.GuildId);
                var user = _client.GetUser(ban.BannedUserId);

                await guild.RemoveBanAsync(ban.BannedUserId);
                await Logging.TryLoggingAsync($":clock3:  **{user.Username}#{user.Discriminator}**'s `{(ban.UnbanTime-ban.Timestamp).Humanize().Singularize()}` ban has expired.");
                TempBanService.RemoveTemporaryBan(ban);
                bans.Add(ban);
            }
            foreach (var ban in bans)
                TempBans.Remove(ban);
        }

        public CheckingService(DiscordSocketClient client, MutingService muting, RemindService remind, TempBanService tempbans)
        {
            _client = client;
            _muting = muting;
            _remind = remind;
            _tempbans = tempbans;
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
