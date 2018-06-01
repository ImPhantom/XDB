using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequirePermissionAttribute : PreconditionAttribute
    {
        private Permission _permission;
        public RequirePermissionAttribute(Permission permission) 
            => _permission = permission;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;

            if (_permission == Permission.GuildAdmin && user.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else if (_permission == Permission.XDBAdministrator && user.Roles.Any(x => x.Name == "XDB Administrator") || user.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else if (_permission == Permission.XDBModerator && user.Roles.Any(x => x.Name == "XDB Moderator") || user.Roles.Any(x => x.Name == "XDB Administrator") || user.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError($"`{Config.Load().Prefix}{command.Name}` requires you to have the `{_permission}` permission."));
        }
    }

    public enum Permission
    {
        GuildAdmin,
        XDBAdministrator,
        XDBModerator
    }
}
