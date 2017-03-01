using System.Threading;
using Discord;

namespace XDB.Services
{
    public class TimerService
    {
        private readonly Timer _timer;

        public TimerService(int time, string reminder, ulong user, ulong channel)
        {
            var client = Program.client;
            _timer = new Timer(_ =>
            {
                var c = client.GetChannel(channel) as ITextChannel;
                var u = client.GetUser(user) as IUser;
                if (string.IsNullOrEmpty(reminder))
                    c.SendMessageAsync($":mega: {u.Mention} Timer is up!");
                else
                    c.SendMessageAsync($":mega: {u.Mention} Timer is up! You need to: `{reminder}`");
            }, null, time, Timeout.Infinite);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }
}
