using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace XDB.Modules
{
    public class Help : ModuleBase<CommandContext>
    {
        private CommandService serv;

        public Help(CommandService service)
        {
            serv = service;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            string pfx = "~";

            var builder = new EmbedBuilder()
            {
                Color = new Color(25, 212, 84),
                Description = "These are the commands and their modules."
            };

            foreach (var module in serv.Modules)
            {
                string desc = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        desc += $"{pfx}{cmd.Name}\n";
                }

                if (!string.IsNullOrWhiteSpace(desc))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = desc;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        [Name("help `<command>`")]
        public async Task HelpAsync(string command)
        {
            var result = serv.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(25, 212, 84),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Remarks: {cmd.Remarks}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}
