using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Humanizer;

namespace XDB.Common.Attributes
{
    //
    //      Code is from: https://github.com/Joe4evr/Discord.Addons/blob/master/src/Discord.Addons.Preconditions/RatelimitAttribute.cs
    //

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RatelimitAttribute : PreconditionAttribute
    {
        private readonly uint _Limit;
        private readonly bool _LimitDMs;
        private readonly bool _LimitAdmins;
        private readonly TimeSpan _LimitPeriod;
        private readonly Dictionary<ulong, CommandTimeout> _Tracker = new Dictionary<ulong, CommandTimeout>();

        public RatelimitAttribute(uint times, double period, Measure measure, bool dms = false, bool admins = false)
        {
            _Limit = times;
            _LimitDMs = dms;
            _LimitAdmins = admins;

            switch (measure)
            {
                case Measure.Days:
                    _LimitPeriod = TimeSpan.FromDays(period);
                    break;
                case Measure.Hours:
                    _LimitPeriod = TimeSpan.FromHours(period);
                    break;
                case Measure.Minutes:
                    _LimitPeriod = TimeSpan.FromMinutes(period);
                    break;
            }
        }

        public RatelimitAttribute(uint times, TimeSpan period, bool dms = false, bool admins = false)
        {
            
            _Limit = times;
            _LimitDMs = dms;
            _LimitAdmins = admins;
            _LimitPeriod = period;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_LimitDMs && context.Channel is IPrivateChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());

            if(_LimitAdmins && context.User is IGuildUser gu && gu.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());

            var now = DateTime.UtcNow;
            var timeout = (_Tracker.TryGetValue(context.User.Id, out var t)
                && ((now - t.FirstInvoke) < _LimitPeriod))
                    ? t : new CommandTimeout(now);

            timeout.TimesInvoked++;

            if(timeout.TimesInvoked <= _Limit)
            {
                _Tracker[context.User.Id] = timeout;
                return Task.FromResult(PreconditionResult.FromSuccess());
            } else
                return Task.FromResult(PreconditionResult.FromError($"You are currently restricted from using this command. (Expires: `{_LimitPeriod.Humanize()}`)"));
        }

        private class CommandTimeout
        {
            public uint TimesInvoked { get; set; }
            public DateTime FirstInvoke { get; set; }

            public CommandTimeout(DateTime timeStarted)
            {
                FirstInvoke = timeStarted;
            }
        }
    }

    public enum Measure
    {
        Days,
        Hours,
        Minutes
    }
}
