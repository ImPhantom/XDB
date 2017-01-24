using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace XDB.Modules
{
    public class Reputation : ModuleBase
    {
        //Leaderboard? might need json

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
                    await ReplyAsync($":grey_exclamation: **{user.Username}'s** reputation: {reader.ReadToEnd()}");
                }
            }
        }

        [Command("addrep")]
        [Remarks("Add reputation to a user.")]
        [RequireContext(ContextType.Guild)]
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
                                await ReplyAsync($":white_check_mark: Added reputation. (__Users reputation:__ {rep})");
                            }
                        }
                    }
                }
            }
        }

        [Command("delrep")]
        [Remarks("Deletes reputation from a user.")]
        [RequireContext(ContextType.Guild)]
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
                                await ReplyAsync($":white_check_mark: Removed reputation. (__Users reputation:__ {rep})");
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
