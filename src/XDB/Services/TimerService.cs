using System.Threading;
using Discord.WebSocket;
using System;

namespace XDB.Services
{
    public class TimerService
    {
        private readonly Timer _timer;

        public TimerService(string time, string reminder, ulong user, ulong channel)
        {
            var client = Program.client;
            _timer = new Timer(_ =>
            {
                var chan = client.GetChannel(channel) as SocketTextChannel;
                var usr = client.GetUser(user) as SocketUser;
                if (string.IsNullOrEmpty(reminder))
                    chan?.SendMessageAsync($":mega: {usr.Mention} Timer is up!");
                else
                    chan?.SendMessageAsync($":mega: {usr.Mention} Timer is up! You need to: `{reminder}`");
            }, null, Convert.ToInt32(ToMS(time)), Timeout.Infinite);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
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
