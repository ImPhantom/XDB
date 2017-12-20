using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Models;
using XDB.Common.Types;
using XDB.Services;
using XDB.Utilities;

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

            client.UserVoiceStateUpdated += OnVoiceStateChange;
            client.UserJoined += OnUserJoined;
            client.UserLeft += OnUserLeave;
            client.UserUpdated += OnUserUpdate;
            client.GuildMemberUpdated += OnGuildMemberUpdate;
            client.MessageDeleted += OnMessageDelete;
            client.MessageUpdated += OnMessageUpdate;
            client.ReactionAdded += OnReactionAdded;

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
                .AddSingleton<AudioService>();

            return new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }

        #region Events
        private async Task OnVoiceStateChange(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (user.Id == client.CurrentUser.Id)
                if (after.VoiceChannel == null)
                    await _audio.LeaveAudio(before.VoiceChannel.Guild);

            if (_checking.Mutes.Any(x => x.UserId == user.Id))
            {
                var mute = _moderation.FetchMutes().First(x => x.UserId == user.Id);
                var _user = user as SocketGuildUser;
                if (mute.Type != MuteType.Text)
                    await _moderation.ApplyMuteAsync(_user, MuteType.Voice);
            }
        }

        private async Task OnUserJoined(SocketGuildUser user)
        {
            if (user.IsBot)
                return;

            if (_moderation.FetchMutes().Any(x => x.UserId == user.Id))
            {
                var mute = _moderation.FetchMutes().First(x => x.UserId == user.Id);
                await _moderation.ApplyMuteAsync(user, mute.Type);
            }

            if (Config.Load().Welcome)
            {
                var channel = user.Guild.TextChannels.First(x => x.Name == "chat");
                var message = Config.Load().WelcomeMessage.Replace("{mention}", user.Mention).Replace("{username}", user.Username);
                await channel.SendMessageAsync(message);
            }

            if (Config.Load().ExtraLogging)
            {
                var age = DateTimeOffset.Now - user.CreatedAt;
                if (age < TimeSpan.FromDays(1))
                    await Logging.TryLoggingAsync($":white_check_mark: (**New Account**) `{user.Username}#{user.Discriminator}` has joined the server! (Age: {age.Humanize()})");
                else
                    await Logging.TryLoggingAsync($":white_check_mark:  `{user.Username}#{user.Discriminator}` has joined the server!");
            }

            await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");
        }

        private async Task OnUserLeave(SocketGuildUser user)
        {
            if (user.IsBot)
                return;
            if (Config.Load().ExtraLogging)
            {
                var bans = await user.Guild.GetBansAsync();
                if (bans.Any(x => x.User.Id == user.Id))
                    return;
                await Logging.TryLoggingAsync($":x:  `{user.Username}#{user.Discriminator}` has left the server!");
            }

            await client.SetGameAsync($"{Config.Load().Prefix}help | Users: {client.Guilds.Sum(x => x.Users.Count())}");
        }

        private async Task OnUserUpdate(SocketUser before, SocketUser after)
        {
            if (Config.Load().ExtraLogging)
                if (before.Username != after.Username)
                    await Logging.TryLoggingAsync($":white_small_square: `{before.Username}#{before.Discriminator}` has changed their username to `{after.Username}#{after.Discriminator}`");
        }

        private async Task OnGuildMemberUpdate(SocketGuildUser before, SocketGuildUser after)
        {
            if (Config.Load().ExtraLogging)
                if (before.Nickname != after.Nickname)
                    await Logging.LogNicknamesAsync(before, after);
        }

        private async Task OnMessageDelete(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            if (Config.Load().ExtraLogging)
            {
                if (!message.HasValue)
                    return;
                var msg = await message.GetOrDownloadAsync();
                if (msg.Content.Contains("~") || msg.Author.IsBot || Config.Load().IgnoredChannels.Contains(channel.Id))
                    return;

                await Logging.TryLoggingAsync($":heavy_multiplication_x: (`#{channel.Name}`) **{msg.Author.Username}** deleted their message:\n{msg.Content.Replace("@", "@\u200B")}");
            }
        }

        private async Task OnMessageUpdate(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            if (Config.Load().ExtraLogging)
            {
                var old = await before.GetOrDownloadAsync();
                if (old.Content != after.Content)
                {
                    if (after.Author.IsBot || Config.Load().IgnoredChannels.Contains(channel.Id))
                        return;
                    await Logging.TryLoggingAsync($":heavy_plus_sign: **{after.Author.Username}** edited their message:\n**Before:** {old.Content.Replace("@", "@\u200B")}\n**After:** {after.Content.Replace("@", "@\u200B")}");
                }
            }
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (Config.Load().Owners.Contains(reaction.User.Value.Id))
            {
                if (!message.HasValue)
                    return;
                var _message = await message.GetOrDownloadAsync();
                var _board = new BoardService(client);
                await _board.CheckChannelExistence();
                if (reaction.Emote.Name == "🚫")
                {
                    var board = new BoardMessage()
                    {
                        Message = _message.Content,
                        UserId = _message.Author.Id,
                        Timestamp = _message.Timestamp
                    };
                    await _board.AddBoardMessageAsync(board);
                    await _message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                }
            }
        }
        #endregion
    }
}
