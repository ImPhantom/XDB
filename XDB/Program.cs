using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB
{
    public class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        public static DiscordSocketClient client;
        private Handler cmds;

        public async Task Run()
        {
            Console.WriteLine(Strings.XDB_Header);
            Console.Title = Strings.XDB_Title;

            Config.CheckExistence();

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000
            });

            client.Log += (l)
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));

            await client.LoginAsync(TokenType.Bot, Config.Load().Token);
            await client.StartAsync();

            await Task.Delay(3000);
            await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");

            Events.Listen();

            cmds = new Handler();
            await cmds.Install(client);

            await Task.Delay(-1);
        }
    }
}
