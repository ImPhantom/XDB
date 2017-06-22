﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Permissions : PreconditionAttribute
    {
        private AccessLevel Level;

        public Permissions(AccessLevel level)
        {
            Level = level;
        }

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var access = GetPermission(context);

            if (access >= Level)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError("You do not have permission to use this command."));
        }

        public AccessLevel GetPermission(ICommandContext c)
        {
            if (c.User.IsBot)
                return AccessLevel.Blocked;

            if (Config.Load().Moderators.Contains(c.User.Id))
                return AccessLevel.Moderator;

            if (Config.Load().Administrators.Contains(c.User.Id))
                return AccessLevel.Administrators;

            if (Config.Load().Owners.Contains(c.User.Id))
                return AccessLevel.BotOwner;

            var user = c.User as SocketGuildUser;
            if (user != null)
            {
                if (c.Guild.OwnerId == user.Id)
                    return AccessLevel.GuildOwner;

                if (user.GuildPermissions.Administrator)
                    return AccessLevel.FullAdmin;
            }

            return AccessLevel.User;
        }
    }
}
