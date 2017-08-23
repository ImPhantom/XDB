using Discord.Commands;
using System.Threading.Tasks;
using System;
using XDB.Services;
using XDB.Common.Models;
using Humanizer;

namespace XDB.Modules
{
    [Summary("Remind")]
    [RequireContext(ContextType.Guild)]
    public class Remind : ModuleBase<SocketCommandContext>
    {
        private RemindService _remind;
        private CheckingService _checking;

        [Command("remind"), Summary("Sets a reminder for you.")]
        public async Task RemindMe(TimeSpan time, [Remainder] string reminder = "")
        {
            var _reminder = new Reminder()
            {
                GuildId = Context.Guild.Id,
                ChannelId = Context.Channel.Id,
                UserId = Context.User.Id,
                Reason = reminder,
                Timestamp = DateTime.UtcNow,
                RemindTime = DateTime.UtcNow.Add(time)
            };
            _remind.AddReminder(_reminder);
            _checking.Reminders.Add(_reminder);
            await ReplyAsync($":alarm_clock: Okay, I will remind you in {time.Humanize()}.");
        }

        public Remind(RemindService remind, CheckingService checking)
        {
            _remind = remind;
            _checking = checking;
        }
    }
}
