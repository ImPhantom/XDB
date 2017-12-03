using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;

namespace XDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireAdministratorAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;
            if (user.Roles.Any(x => x.Name == "XDB Administrator") || user.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("This command requires the \"XDB Administrator\" role."));
        }
    }
}
