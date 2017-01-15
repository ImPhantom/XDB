using Discord;
using Discord.Commands;
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
        private string header = @"          
        -----------------------------------------------------------------------------------------------------   
                                             __   _______  ____  
                                             \ \ / /  __ \|  _ \ 
                                              \ V /| |  | | |_) |
                                               > < | |  | |  _ < 
                                              / ^ \| |__| | |_) |
                                             /_/ \_\_____/|____/ 
        -----------------------------------------------------------------------------------------------------
                                                         ";

        public async Task Run()
        {
            Console.WriteLine(header);
            Console.Title = $"XDB ({DiscordConfig.Version})";

            CheckForConfig();

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });

            client.Log += (l)
                => Task.Run(()
                => Console.WriteLine($"[{l.Severity}] {l.Source}: {l.Exception?.ToString() ?? l.Message}"));

            await client.LoginAsync(TokenType.Bot, Config.Load().Token);
            await client.ConnectAsync();

            Events.Events.initEvents();

            var map = new DependencyMap();
            map.Add(client);

            cmds = new Handler();
            await cmds.Install(map);

            await Task.Delay(-1);
        }

        private void CheckForConfig()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "cfg")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "cfg"));

            string location = Path.Combine(AppContext.BaseDirectory, "cfg/config.json");

            if(!File.Exists(location))
            {
                var cfg = new Config();
                Console.WriteLine("The configuration file has been created at 'cfg\\config.json', " +
                              "please fill out the config and restart XDB.");
                Console.Write("Token: ");
                cfg.Token = Console.ReadLine();
                cfg.Save();
            }
            Console.WriteLine("[XDB] Config Loaded Successfully.");
        }
    }
}
