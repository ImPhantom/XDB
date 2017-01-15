using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace XDB
{
    public class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        public static DiscordSocketClient client;
        private Handler cmds;

        public async Task Run()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });

            client.Log += (l)
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));

            string token = "MjU2MjQzNTEzNzU3MjcwMDI4.C1xKKA.8CJnTi69dinJrXtHyonw3jZhTJc";

            await client.LoginAsync(TokenType.Bot, token);
            await client.ConnectAsync();

            Events.Events.initEvents();

            var map = new DependencyMap();
            map.Add(client);

            cmds = new Handler();
            await cmds.Install(map);

            await Task.Delay(-1);
        }
    }
}
