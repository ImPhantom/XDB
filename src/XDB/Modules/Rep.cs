using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Enums;
using XDB.Common.Types;

namespace XDB.Modules
{
    public class Rep : ModuleBase
    {
        [Command("leaderboard")]
        [Remarks("Shows the leading users in reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task Leaderboard()
        {
            Config.RepCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
            var read = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(read);

            var topreps = json.OrderByDescending(x => x.Rep).Take(10);
            var str = new StringBuilder();
            str.AppendLine("```");
            foreach(var rep in topreps)
            {
                var user = await Context.Client.GetUserAsync(rep.Id);
                if (user == null)
                    str.AppendLine($"NULL ~ {rep.Rep.ToString()}");
                else
                    str.AppendLine($"{user.Username} ~ {rep.Rep.ToString()}");
            }
            str.AppendLine("```");
            await ReplyAsync(":grey_exclamation: Current top reputations.");
            await ReplyAsync($"{str.ToString()}");
        }

        [Command("rep")]
        [Remarks("Views your reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task MyRep()
        {
            Config.RepCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
            var filetext = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json"));
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if(!json.Any(x => x.Id == Context.User.Id))
                {
                    var defrep = new UserRep() { Id = Context.User.Id, Rep = 0 };
                    json.Add(defrep);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                }
                var rep = json.First(x => x.Id == Context.User.Id).Rep;
                if (rep < 0)
                    await ReplyAsync($":red_circle: **{Context.User.Username}'s** reputation: {rep}");
                else
                    await ReplyAsync($":white_circle: **{Context.User.Username}'s** reputation: {rep}");
            }
            catch(Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("rep")]
        [Name("rep `<@user>`")]
        [Remarks("Views a users reputation.")]
        [RequireContext(ContextType.Guild)]
        public async Task CheckRep(IGuildUser user)
        {
            Config.RepCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
            var filetext = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json"));
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    var defrep = new UserRep() { Id = user.Id, Rep = 0 };
                    json.Add(defrep);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                }
                var rep = json.First(x => x.Id == user.Id).Rep;
                if (rep < 0)
                    await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                else
                    await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("addrep")]
        [Name("addrep `<@user>`")]
        [Remarks("Add reputation to a user.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddRep(IGuildUser user)
        {
            Config.RepCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
            var filetext = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json"));
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    var defrep = new UserRep() { Id = user.Id, Rep = 1 };
                    json.Add(defrep);
                    await ReplyAsync($":white_circle: **{user.Username}'s** reputation: 1");
                    var defout = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, defout);
                } else
                {
                    json.First(x => x.Id == user.Id).Rep++;
                    var rep = json.First(x => x.Id == user.Id).Rep;
                    if (rep < 0)
                        await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                    else
                        await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("delrep")]
        [Name("delrep `<@user>`")]
        [Remarks("Deletes reputation from a user.")]
        [RequireContext(ContextType.Guild)]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelRep(IGuildUser user)
        {
            Config.RepCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json");
            var filetext = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"rep/reputations.json"));
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    var defrep = new UserRep() { Id = user.Id, Rep = -1 };
                    json.Add(defrep);
                    await ReplyAsync($":red_circle: **{user.Username}'s** reputation: -1");
                    var defout = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, defout);
                }
                else
                {
                    json.First(x => x.Id == user.Id).Rep--;
                    var rep = json.First(x => x.Id == user.Id).Rep;
                    if (rep < 0)
                        await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                    else
                        await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}
