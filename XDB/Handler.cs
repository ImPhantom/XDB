using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using XDB.Common.Types;
using XDB.Readers;
using System.Collections.Generic;

namespace XDB
{
    public class Handler
    {
        private DiscordSocketClient _client;
        private IServiceProvider _provider;
        private CommandService _cmds;

        public Handler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _cmds = _provider.GetService<CommandService>();
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

            var context = new SocketCommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(Config.Load().Prefix, ref argPos))
            {
                if (Config.Load().BotChannelWhitelist)
                    if (!Config.Load().WhitelistedChannels.Contains(s.Channel.Id))
                    {
                        var management = new List<ulong>() { 186253663398789120, 93765631177920512, 99710940601135104, 97675548985143296 };

                        if (!management.Contains(s.Author.Id))
                            if(!msg.Content.StartsWith("~tag"))
                                return;
                    }
                    
                if (s.Author.IsBot)
                    return;
                var result = await _cmds.ExecuteAsync(context, argPos, _provider);

                if (!result.IsSuccess)
                {
                    if (result.Error == CommandError.UnknownCommand)
                        return;

                    await context.Channel.SendMessageAsync("", false, Xeno.ErrorEmbed(result.ErrorReason));
                }
            }
        }
    }
}
