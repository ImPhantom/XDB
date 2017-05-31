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
        //private RatelimitService _rate;
        private MutingService _muting;
        private RemindService _remind;
        private TempBanService _tempbans;
        private CheckingService _checking;

        public async Task Run()
        {
            BetterConsole.AppendText(Strings.XDB_Header);
            Console.Title = Strings.XDB_Title;

            Config.CheckExistence();

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
#if RELEASE
                LogLevel = LogSeverity.Verbose,
#else
                LogLevel = LogSeverity.Debug,
#endif
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000
            });

            //Commented out cause its a rough version...

            //_rate = new RatelimitService(client, _registry, _checking);
            //await _rate.LoadConfigurationAsync();
            //if (_rate.IsEnabled)
            // _rate.Enable(_rate.Limit);

            _muting = new MutingService();
            _muting.InitializeMutes();

            _remind = new RemindService();
            _remind.Initialize();

            _tempbans = new TempBanService();
            _tempbans.Initialize();

            _checking = new CheckingService(client, _muting, _remind, _tempbans);
            await _checking.FetchChecksAsync();

            var serviceProvider = ConfigureServices();

            cmds = new Handler(serviceProvider);
            await cmds.Install();

            client.Log += (l)
                => Task.Run(()
                => BetterConsole.Log(l.Severity, l.Source, l.Message));

            await client.LoginAsync(TokenType.Bot, Config.Load().Token);
            await client.StartAsync();

            await Task.Delay(3000);
            await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");

            Events.Listen();

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }))
                //.AddSingleton<RatelimitService>()
                .AddSingleton<MutingService>()
                .AddSingleton<RemindService>()
                .AddSingleton<TempBanService>()
                .AddSingleton<CheckingService>();

            return new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }
    }
}
