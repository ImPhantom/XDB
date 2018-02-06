using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace XDB.Common
{
    public class XenoBase : ModuleBase<SocketCommandContext>
    {
        public async Task<IUserMessage> ReplyThenRemoveAsync(string content, TimeSpan? timeout = null)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            var reply = await base.ReplyAsync(content).ConfigureAwait(false);
            _ = Task.Delay(timeout.Value).ContinueWith(_ => reply.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
            return reply;
        }

        public async Task<IUserMessage> SendErrorEmbedAsync(string error, string source = null)
        {
            var embed = new EmbedBuilder().WithColor(new Color(255, 0, 0)).WithTitle("Error:").WithDescription(error);
            if (source != null)
                embed.Title = $"({source}) Error:";
            return await base.ReplyAsync("", embed: embed.Build());
        }
    }
}
