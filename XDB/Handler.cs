using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XDB.Common.Types;
using XDB.Readers;

namespace XDB
{
    public class Handler
    {
        private DiscordSocketClient _client;
        private IDependencyMap _map;
        private CommandService _cmds;

        public Handler(IDependencyMap map)
        {
            _client = map.Get<DiscordSocketClient>();
            _cmds = new CommandService();
            _map = map;
        }

        public async Task Install()
        {
            _cmds.AddTypeReader<TimeSpan>(new TimeStringTypeReader());
            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());
            await ModuleConfig.RemoveDisabledModulesAsync(_cmds);
            _client.MessageReceived += HandleCommand;
        }

        private async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var map = new DependencyMap();
            var context = new SocketCommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(Config.Load().Prefix, ref argPos))
            {
                if (s.Author.IsBot)
                    return;
                var result = await _cmds.ExecuteAsync(context, argPos, map);

                if (!result.IsSuccess)
                {
                    if (result.Error == CommandError.UnknownCommand)
                        return;

                    var embed = new EmbedBuilder().WithColor(new Color(255, 0, 0)).WithTitle("**Error:**").WithDescription(result.ErrorReason);
                    await context.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
        }
    }
}
