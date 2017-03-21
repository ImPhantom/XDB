using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Modules
{
    [Summary("Help")]
    public class Help : ModuleBase<CommandContext>
    {
        private CommandService serv;

        public Help(CommandService service)
        {
            serv = service;
        }

        [Command("help"), Summary("Shows all commands available to you.")]
        [RequireContext(ContextType.Guild)]
        public async Task HelpAsync()
        {
            string prefix = Config.Load().Prefix;

            var builder = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Description = "**These are all the commands you have access to.**"
            };

            foreach (var module in serv.Modules)
            {
                string desc = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        desc += $"{prefix}{cmd.Name}\n";
                }

                if (!string.IsNullOrWhiteSpace(desc))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Summary;
                        x.Value = desc;
                        x.IsInline = false;
                    });
                }
            }

            var dm = await Context.User.CreateDMChannelAsync();
            await dm.SendMessageAsync("", false, builder.Build());
        }

        [Command("help"), Summary("Shows the help for a specified command")]
        [Name("help `<command>`")]
        public async Task HelpAsync(string command)
        {
            var result = serv.Search(Context, command);

            var prefix = Config.Load().Prefix;

            if (!result.IsSuccess)
            {
                await ReplyAsync($":anger: There are no commands matching your input.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Description = $"**Commands similar to:** `{prefix}{command}`"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"**Parameters:**  {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"**Remarks:**  {cmd.Summary}\n" +
                              $"**Syntax:**  {prefix + cmd.Name}";
                    x.IsInline = false;
                });
            }

            var dm = await Context.User.CreateDMChannelAsync();
            await dm.SendMessageAsync("", false, builder.Build());
        }
    }
}
