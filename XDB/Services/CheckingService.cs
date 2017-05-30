using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
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
        public static List<Mute> Mutes { get; set; }

        public Task FetchChecksAsync()
        {
            Mutes = new List<Mute>(_muting.GetActiveMutes());
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

        public CheckingService(DiscordSocketClient client, MutingService muting)
        {
            _client = client;
            _muting = muting;
            Mutes = new List<Mute>();

            // Start the timer
            _timer = new Timer(async _ =>
            {
                await CheckForExpiredMutesAsync();
            }, null, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(45));
        }
    }
}
