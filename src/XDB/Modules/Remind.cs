using Discord.Commands;
using System.Threading.Tasks;
using System;
using XDB.Services;

namespace XDB.Modules
{
    public class Remind : ModuleBase
    {
        // UNDONE
        // SERIOUSLY UNDONE
        [Command("remind")]
        [Name("remind `<1s/1m/1h/1d>` `<reminder>`")]
        [Remarks("Sets a reminder for the calling user.")]
        [RequireContext(ContextType.Guild)]
        public async Task RemindMe(string time, [Remainder] string reminder)
        {
            var ms = Convert.ToInt32(ToMS(time));
            var serv = new TimerService(ms, reminder, Context.User.Id, Context.Channel.Id);
            await ReplyAsync($":alarm_clock: Okay, I will remind you in {time}.");
        }

        [Command("remind")]
        [Name("remind `<1s/1m/1h/1d>`")]
        [Remarks("Sets a reminder for the calling user.")]
        [RequireContext(ContextType.Guild)]
        public async Task RemindMe(string time)
        {
            var ms = Convert.ToInt32(ToMS(time));
            var serv = new TimerService(ms, "", Context.User.Id, Context.Channel.Id);
            await ReplyAsync($":alarm_clock: Okay, I will remind you in {time}.");
        }

        private static double ToMS(string time)
        {
            var length = time.Length - 1;
            var value = time.Substring(0, length);
            var type = time.Substring(length, 1);

            switch (type)
            {
                case "d": return TimeSpan.FromDays(double.Parse(value)).TotalMilliseconds;
                case "m": return TimeSpan.FromMinutes(double.Parse(value)).TotalMilliseconds;
                case "h": return TimeSpan.FromHours(double.Parse(value)).TotalMilliseconds;
                case "s": return TimeSpan.FromSeconds(double.Parse(value)).TotalMilliseconds;
                default: return TimeSpan.FromHours(double.Parse(value)).TotalMilliseconds;
            }
        }
    }
}
