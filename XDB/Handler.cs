using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB
{
    public class Handler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();                          

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());

            //Module Config Shit
            if (!ModuleConfig.Load().ChatModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Chat"));  }
            if (!ModuleConfig.Load().AdminModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Admin")); }
            if (!ModuleConfig.Load().MathModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Maths")); }
            if (!ModuleConfig.Load().UtilModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Utility")); }
            if (!ModuleConfig.Load().WarnModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Warn")); }
            if (!ModuleConfig.Load().RepModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Rep")); }
            if (!ModuleConfig.Load().TodoModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Todo")); }
            if (!ModuleConfig.Load().SteamModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Steam")); }
            if (!ModuleConfig.Load().RemindModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Summary == "Remind")); }


            _client.MessageReceived += HandleCommand;
        }

        private async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var map = new DependencyMap();
            var context = new CommandContext(_client, msg);

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
