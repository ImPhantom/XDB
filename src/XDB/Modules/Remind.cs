using Discord.Commands;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace XDB.Modules
{
    public class Remind : ModuleBase
    {
        [Command("remind")]
        [Remarks("Sets a reminder for the calling user.")]
        [RequireContext(ContextType.Guild)]
        public async Task RemindMe(int ms)
        {
            var timer = new Timer(async _ =>
            {
                await ReplyAsync("Timer ran out.");
            }, null, ms, Timeout.Infinite);
            await ReplyAsync("Timer made?");
        }

        [Command("time")]
        [Remarks("t")]
        [RequireContext(ContextType.Guild)]
        public async Task Time(string time)
        {
            var timespan = ToTimeSpan(time);
            await ReplyAsync($"{timespan.ToString()}");
        }

        private static TimeSpan ToTimeSpan(string time)
        {
            var length = time.Length - 1;
            var value = time.Substring(0, length);
            var type = time.Substring(length, 1);

            switch (type)
            {
                case "d": return TimeSpan.FromDays(double.Parse(value));
                case "m": return TimeSpan.FromMinutes(double.Parse(value));
                case "h": return TimeSpan.FromHours(double.Parse(value));
                default: return TimeSpan.FromHours(double.Parse(value));
            }
        }
    }
}
