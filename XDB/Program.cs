using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Types;
using XDB.Services;

namespace XDB
{
    public class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        public static DiscordSocketClient client;
        private Handler cmds;
        private ModerationService _moderation;
        private RemindService _remind;
        private CheckingService _checking;
        private BoardService _board;
        private ListService _lists;
        private AudioService _audio;
        private CachingService _caching;

        public async Task Run()
        {
            BetterConsole.AppendText(Xeno.Masthead);
            Console.Title = Xeno.Status;

            Config.CheckExistence();
            Config.InitializeData();

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,

                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000
            });

            _moderation = new ModerationService();
            _moderation.Initialize();

            _remind = new RemindService();
            _remind.Initialize();

            _checking = new CheckingService(client, _moderation, _remind);
            await _checking.FetchChecksAsync();

            _board = new BoardService(client);
            _board.Initialize();

            _lists = new ListService(client);
            _lists.Initialize();

            _audio = new AudioService();
            _caching = new CachingService();
            await _caching.Initialize();

            var serviceProvider = ConfigureServices();

            cmds = new Handler(serviceProvider);
            await cmds.Install();

            client.Log += (l)
                => Task.Run(()
                => BetterConsole.Log(l.Severity, l.Source, l.Message));

            await client.LoginAsync(TokenType.Bot, Config.Load().Token);
            await client.StartAsync();

            await Task.Delay(6000);
            await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");

            Events.Listen(_checking, _moderation);

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }))
                .AddSingleton<ModerationService>()
                .AddSingleton<RemindService>()
                .AddSingleton<CheckingService>()
                .AddSingleton<ListService>()
                .AddSingleton<AudioService>()
                .AddSingleton<CachingService>();

            return new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }
    }
}
