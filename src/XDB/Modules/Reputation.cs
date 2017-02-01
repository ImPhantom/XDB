using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;
using XDB.Common.Types;

namespace XDB.Modules
{
    public class Reputation : ModuleBase
    {
        //UNDONE: Leaderboard
        [Command("leaderboard")]
        [Remarks("Shows the leading users in reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task Leaderboard()
        {
            await ReplyAsync(":unamused: Leaderboard not implemented yet. *sorry*");
        }

        [Command("rep")]
        [Remarks("Views your reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task Rep()
        {
            var user = Context.User;
            Config.RepCheck(user);
            var path = Path.Combine(AppContext.BaseDirectory, $"reputation/{user.Id}.txt");
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    int rep;
                    int.TryParse(reader.ReadToEnd(), out rep);
                    if (rep < 0)
                    {
                        await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                    }
                    else
                    {
                        await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                    }
                }
            }
        }

        [Command("rep")]
        [Name("rep `<@user>`")]
        [Remarks("Views a users reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task Rep(IGuildUser user)
        {
            Config.RepCheck(user);
            var path = Path.Combine(AppContext.BaseDirectory, $"reputation/{user.Id}.txt");
            using (var file = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    int rep;
                    int.TryParse(reader.ReadToEnd(), out rep);
                    if(rep < 0){
                        await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                    } else {
                        await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                    }
                }
            }
        }

        [Command("addrep")]
        [Name("addrep `<@user>`")]
        [Remarks("Add reputation to a user.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddRep(IGuildUser user)
        {
            Config.RepCheck(user);
            var path = Path.Combine(AppContext.BaseDirectory, $"reputation/{user.Id}.txt");
            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var reader = new StreamReader(file))
                {
                    int rep;
                    int.TryParse(reader.ReadToEnd(), out rep);
                    rep++;
                    file.Dispose();
                    using (var empty = new FileStream(path, FileMode.Truncate))
                    {
                        empty.Dispose();
                        using (var write = new FileStream(path, FileMode.Append, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(write))
                            {
                                writer.Write(rep);
                                if (rep < 0)
                                {
                                    await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                                }
                                else
                                {
                                    await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("delrep")]
        [Name("delrep `<@user>`")]
        [Remarks("Deletes reputation from a user.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelRep(IGuildUser user)
        {
            Config.RepCheck(user);
            var path = Path.Combine(AppContext.BaseDirectory, $"reputation/{user.Id}.txt");
            using (var file = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var reader = new StreamReader(file))
                {
                    int rep;
                    int.TryParse(reader.ReadToEnd(), out rep);
                    rep--;
                    file.Dispose();
                    using (var empty = new FileStream(path, FileMode.Truncate))
                    {
                        empty.Dispose();
                        using (var write = new FileStream(path, FileMode.Append, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(write))
                            {
                                writer.Write(rep);
                                if (rep < 0)
                                {
                                    await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                                }
                                else
                                {
                                    await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
