using Discord;
using Discord.WebSocket;
using System;
using System.IO;
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

            CheckForConfig();

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true
            });

            client.Log += (l)
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));

            await client.LoginAsync(TokenType.Bot, Config.Load().Token);
            await client.StartAsync();

            Events.Events.initEvents();

            cmds = new Handler();
            await cmds.Install(client);

            await Task.Delay(-1);
        }

        private void CheckForConfig()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "cfg")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "cfg"));

            if (!File.Exists(Strings.ConfigPath))
            {
                var cfg = new Config();
                Console.WriteLine(Strings.XDB_ConfigCreated);
                Console.Write("Token: ");
                cfg.Token = Console.ReadLine();
                cfg.Save();
            }
            if(!File.Exists(Strings.ModulePath)) { var mdls = new ModuleConfig(); mdls.Save(); }
            Console.WriteLine(Strings.XDB_ConfigLoaded);
        }
    }
}
