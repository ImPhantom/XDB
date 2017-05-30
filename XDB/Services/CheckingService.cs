using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Models;
using XDB.Common.Types;
using XDB.Utilities;
using System.Threading;

namespace XDB.Services
{
    public class CheckingService
    {
        private readonly Timer _timer;

        private DiscordSocketClient _client;
        private MutingService _muting;
        private RemindService _remind;
        public static List<Mute> Mutes { get; set; }
        public static List<Reminder> Reminders { get; set; }

        public Task FetchChecksAsync()
        {
            Mutes = new List<Mute>(_muting.GetActiveMutes());
            Reminders = new List<Reminder>(_remind.GetActiveReminders());
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

        public CheckingService(DiscordSocketClient client, MutingService muting, RemindService remind)
        {
            _client = client;
            _muting = muting;
            _remind = remind;
            Mutes = new List<Mute>();
            Reminders = new List<Reminder>();

            // Start the timer
            _timer = new Timer(async _ =>
            {
                await CheckForExpiredMutesAsync();
                await CheckForExpiredRemindersAsync();
            }, null, TimeSpan.FromSeconds(45), TimeSpan.FromSeconds(30));
        }
    }
}
