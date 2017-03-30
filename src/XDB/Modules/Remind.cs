using Discord.Commands;
using System.Threading.Tasks;
using System;
using XDB.Services;

namespace XDB.Modules
{
    [Summary("Remind")]
    [RequireContext(ContextType.Guild)]
    public class Remind : ModuleBase
    {
        // UNDONE
        // SERIOUSLY UNDONE
        [Command("remind"), Summary("Sets a reminder for you.")]
        [Name("remind `<1s/1m/1h/1d>` `<reminder>`")]
        public async Task RemindMe(string time, [Remainder] string reminder = "")
        {
            var serv = new TimerService(time, reminder, Context.User.Id, Context.Channel.Id);
            await ReplyAsync($":alarm_clock: Okay, I will remind you in {time}.");
        }
    }
}
