using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireRoleByNameAttribute : PreconditionAttribute
    {
        private string _roleName;
        public RequireRoleByNameAttribute(string roleName)
            => _roleName = roleName;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;
            if (user.Roles.Any(y => y.Name == _roleName))
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError($"`{Config.Load().Prefix}{command.Name}` requires you to have the `{_roleName}` role."));
        }
    }
}
