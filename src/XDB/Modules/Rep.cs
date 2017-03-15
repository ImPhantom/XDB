using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
    [Summary("Reputation")]
    [Group("rep")]
    [RequireContext(ContextType.Guild)]
    public class Rep : ModuleBase
    {
        [Command("top"), Summary("Displays the users with the highest reputation.")]
        [Name("rep top")]
        [Alias("leaderboard")]
        public async Task Leaderboard()
        {
            Config.RepCheck();
            var read = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(read);

            var topreps = json.OrderByDescending(x => x.Rep).Take(10);
            var embed = new EmbedBuilder() { Color = new Color(21, 144, 232) };
            var str = new StringBuilder();
            foreach(var rep in topreps)
            {
                var user = await Context.Client.GetUserAsync(rep.Id);
                if (user == null)
                {
                    embed.AddField(x => {
                        x.Name = "**null_user**";
                        x.Value = $"Reputation: {rep.Rep}";
                        x.IsInline = false;
                    });
                } else
                {
                    embed.AddField(x => {
                        x.Name = $"**{user.Username}**";
                        x.Value = $"Reputation: {rep.Rep}";
                        x.IsInline = false;
                    });
                }
            }
            await ReplyAsync(":star: **Top 10 Reputations:**");
            await ReplyAsync("", false, embed.Build());
        }

        [Command, Summary("Displays your reputation.")]
        [Name("rep")]
        public async Task MyRep()
        {
            Config.RepCheck();
            var filetext = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if(!json.Any(x => x.Id == Context.User.Id))
                {
                    var defrep = new UserRep() { Id = Context.User.Id, Rep = 0 };
                    json.Add(defrep);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, outjson);
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

        [Command("user"), Summary("Displays a specified users reputation.")]
        [Name("rep user `<@user>`")]
        public async Task CheckRep(SocketUser user)
        {
            Config.RepCheck();
            var filetext = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    var defrep = new UserRep() { Id = user.Id, Rep = 0 };
                    json.Add(defrep);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, outjson);
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

        [Command("add"), Summary("Adds reputation to a user.")]
        [Name("rep add `<@user>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddRep(SocketUser user)
        {
            Config.RepCheck();
            var filetext = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    var defrep = new UserRep() { Id = user.Id, Rep = 1 };
                    json.Add(defrep);
                    await ReplyAsync($":white_circle: **{user.Username}'s** reputation: 1");
                    var defout = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, defout);
                } else
                {
                    json.First(x => x.Id == user.Id).Rep++;
                    var rep = json.First(x => x.Id == user.Id).Rep;
                    if (rep < 0)
                        await ReplyAsync($":red_circle: **{user.Username}'s** reputation: {rep}");
                    else
                        await ReplyAsync($":white_circle: **{user.Username}'s** reputation: {rep}");
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, outjson);
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("del"), Summary("Deletes reputation from a user.")]
        [Name("rep del `<@user>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task DelRep(SocketUser user)
        {
            Config.RepCheck();
            var filetext = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    var defrep = new UserRep() { Id = user.Id, Rep = -1 };
                    json.Add(defrep);
                    await ReplyAsync($":red_circle: **{user.Username}'s** reputation: -1");
                    var defout = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, defout);
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
                    File.WriteAllText(Strings.RepPath, outjson);
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}
