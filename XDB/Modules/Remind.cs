using Discord.Commands;
using System.Threading.Tasks;
using System;
using XDB.Services;

namespace XDB.Modules
{
    [Summary("Remind")]
    [RequireContext(ContextType.Guild)]
    public class Remind : ModuleBase<SocketCommandContext>
    {
        // UNDONE
        // SERIOUSLY UNDONE
        [Command("remind"), Summary("Sets a reminder for you.")]
        public async Task RemindMe(TimeSpan time, [Remainder] string reminder = "")
        {
            var serv = new TimerService(time, reminder, Context.User.Id, Context.Channel.Id);
            await ReplyAsync($":alarm_clock: Okay, I will remind you in {time.TotalMinutes} minutes.");
        }
    }
}
