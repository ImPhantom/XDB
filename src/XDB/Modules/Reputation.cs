using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;

namespace XDB.Modules
{
    public class Reputation : ModuleBase
    {
        //Leaderboard? might need json
        [Command("leaderboard")]
        [Remarks("Shows the leading users in reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task Leaderboard()
        {
            await ReplyAsync("Leaderboard not implemented yet. *sorry*");
        }

        [Command("rep")]
        [Remarks("Views a users reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task Rep(IGuildUser user)
        {
            ConfigCheck(user);
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
                        await ReplyAsync($":large_blue_circle: **{user.Username}'s** reputation: {rep}");
                    }
                }
            }
        }

        [Command("addrep")]
        [Remarks("Add reputation to a user.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddRep(IGuildUser user)
        {
            ConfigCheck(user);
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
                                    await ReplyAsync($":large_blue_circle: **{user.Username}'s** reputation: {rep}");
                                }
                            }
                        }
                    }
                }
            }
        }

        [Command("delrep")]
        [Remarks("Deletes reputation from a user.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelRep(IGuildUser user)
        {
            ConfigCheck(user);
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
                                    await ReplyAsync($":large_blue_circle: **{user.Username}'s** reputation: {rep}");
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ConfigCheck(IGuildUser user)
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"reputation/{user.Id}.txt");
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "reputation")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "reputation"));

            if(!File.Exists(path))
            {
                using (var file = new FileStream(path, FileMode.Create))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        writer.Write("0");
                        writer.Dispose();
                    }
                }
            } else
            {
                return;
            }
            
        }
    }
}
