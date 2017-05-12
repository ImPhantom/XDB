using System.Threading;
using Discord.WebSocket;
using System;

namespace XDB.Services
{
    public class TimerService
    {
        private readonly Timer _timer;

        public TimerService(TimeSpan time, string reminder, ulong user, ulong channel)
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
            }, null, Convert.ToInt32(time.TotalMilliseconds), Timeout.Infinite);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
