using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireStaffAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;
            var roles = new string[] { "Trial-Mod", "Moderator", "Senior Moderator", "Head-Admin", "Admin", "Community Manager", "Owners" };

            if (roles.Any(x => user.Roles.Any(y => y.Name == x)))
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("This command requires you to be a staff member."));
        }
    }
}
