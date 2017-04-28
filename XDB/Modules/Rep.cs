using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Reputation")]
    [Group("rep")]
    [RequireContext(ContextType.Guild)]
    public class Rep : ModuleBase<SocketCommandContext>
    {
        [Command, Summary("Displays your reputation.")]
        [Name("rep")]
        public async Task MyRep()
            => await Reputation.CheckReputationAsync(Context, Context.User as SocketGuildUser);

        [Command("user"), Summary("Displays a specified users reputation.")]
        [Name("rep user `<@user>`")]
        public async Task CheckRep(SocketGuildUser user)
            => await Reputation.CheckReputationAsync(Context, user);

        [Command("top"), Summary("Displays the users with the highest reputation.")]
        [Name("rep top")]
        [Alias("leaderboard")]
        public async Task Leaderboard()
            => await Reputation.GetLeaderboardAsync(Context);

        [Command("add"), Summary("Adds reputation to a user.")]
        [Name("rep add `<@user>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddRep(SocketGuildUser user)
            => await Reputation.AddReputationAsync(Context, user);

        [Command("del"), Summary("Deletes reputation from a user.")]
        [Name("rep del `<@user>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelRep(SocketGuildUser user)
            => await Reputation.RemoveReputationAsync(Context, user);
    }
}
