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
            if (!ModuleConfig.Load().SteamModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Name == "Steam")); }
            if (!ModuleConfig.Load().MathModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Name == "Maths")); }
            if (!ModuleConfig.Load().WarnModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Name == "Warn")); }
            if (!ModuleConfig.Load().RepModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Name == "Rep")); }
            if (!ModuleConfig.Load().UtilModule) { await _cmds.RemoveModuleAsync(_cmds.Modules.First(x => x.Name == "Utility")); }

            _client.MessageReceived += HandleCommand;
        }

        private async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var map = new DependencyMap();
            map.Add(_cmds);
            var context = new CommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(Config.Load().Prefix, ref argPos))
            {
                var result = await _cmds.ExecuteAsync(context, argPos, map);

                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
