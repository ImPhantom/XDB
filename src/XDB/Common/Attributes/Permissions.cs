﻿using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Enums;
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

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            var access = GetPermission(context);

            if (access >= Level)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError(":anger: You do not have permission to use this command."));
        }

        public AccessLevel GetPermission(ICommandContext c)
        {
            if (c.User.IsBot)
                return AccessLevel.Blocked;

            if (Config.Load().Owners.Contains(c.User.Id))
                return AccessLevel.BotOwner;

            var user = c.User as SocketGuildUser;
            if (user != null)
            {
                if (c.Guild.OwnerId == user.Id)
                    return AccessLevel.ServerOwner;

                if (user.GuildPermissions.Administrator)
                    return AccessLevel.ServerAdmin;

                if (user.GuildPermissions.ManageMessages ||
                    user.GuildPermissions.BanMembers ||
                    user.GuildPermissions.KickMembers)
                    return AccessLevel.ServerMod;
            }

            return AccessLevel.User;
        }
    }
}
