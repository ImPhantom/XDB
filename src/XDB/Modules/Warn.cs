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
    [Summary("Warn")]
    [RequireContext(ContextType.Guild)]
    public class Warn : ModuleBase
    {
        [Command("warns"), Summary("Views your personal warns.")]
        public async Task Warns()
        {
            Config.WarnCheck();
            var read = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(read);
            try
            {
                if(!json.Any(x => x.WarnedUser == Context.User.Id))
                    await ReplyAsync(":anger: You do not have any warnings.");
                else
                {
                    var warns = json.Find(x => x.WarnedUser == Context.User.Id);
                    if (!warns.WarnReason.Any()) { await ReplyAsync(":anger: You do not have any warnings."); return; }
                    var warnsout = new StringBuilder();
                    foreach(var warn in warns.WarnReason)
                    {
                        warnsout.AppendLine($"~ {warn}");
                    }
                    await ReplyAsync($"You currently have **{warns.WarnReason.Count}** warns.\n```{warnsout.ToString()}```");
                }
            } catch(Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("warns"), Summary("Checks a specified users warnings.")]
        [Name("warns `<@user>`")]
        public async Task Warns(SocketUser user)
        {
            Config.WarnCheck();
            var read = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(read);
            try
            {
                if (!json.Any(x => x.WarnedUser == user.Id))
                    await ReplyAsync(":anger: That user does not have any warnings.");
                else
                {
                    var warns = json.Find(x => x.WarnedUser == user.Id);
                    if (!warns.WarnReason.Any()) { await ReplyAsync(":anger: That user does not have any warnings."); return; }
                    var warnsout = new StringBuilder();
                    foreach (var warn in warns.WarnReason)
                    {
                        warnsout.AppendLine($"~ {warn}");
                    }
                    await ReplyAsync($"{user.Username} currently has **{warns.WarnReason.Count}** warns.\n```{warnsout.ToString()}```");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("warn"), Summary("Warns a specified user.")]
        [Name("warn `<@user>` `<reason>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task AddWarn(SocketUser user, [Remainder] string reason)
        {
            Config.WarnCheck();
            var read = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(read);
            try
            {
                if (!json.Any(x => x.WarnedUser == user.Id))
                {
                    var newwarn = new UserWarn()
                    {
                        WarnedUser = user.Id,
                        WarnReason = new List<string> { reason }
                    };
                    json.Add(newwarn);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.WarnPath, outjson);
                    await ReplyAsync($":white_check_mark: You warned {user.Username} for: \n\n `{reason}`");
                    var dm = await user.CreateDMChannelAsync();
                    await dm.SendMessageAsync($":anger: You have been warned by **{Context.User.Username}** in **{Context.Guild.Name}** for:\n\n`{reason}`");
                } else
                {
                    json.First(x => x.WarnedUser == user.Id).WarnReason.Add(reason);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.WarnPath, outjson);
                    await ReplyAsync($":white_check_mark: You warned {user.Username} for: \n\n `{reason}`");
                    var dm = await user.CreateDMChannelAsync();
                    await dm.SendMessageAsync($":anger: You have been warned by **{Context.User.Username}** in **{Context.Guild.Name}** for:\n\n`{reason}`");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }

        [Command("removewarn"), Summary("Removes a warn from a specified user by index.")]
        [Name("removewarn `<@user>` `<index>`")]
        [Permissions(AccessLevel.ServerAdmin)]
        public async Task RemoveWarn(SocketUser user, int index)
        {
            Config.WarnCheck();
            var read = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(read);
            try
            {
                index--;
                if (!json.Any(x => x.WarnedUser == user.Id))
                {
                    await ReplyAsync(":anger: That user does not have any warns.");
                }
                else
                {
                    if (index < json.First(x => x.WarnedUser == user.Id).WarnReason.Count)
                    {
                        json.First(x => x.WarnedUser == user.Id).WarnReason.RemoveAt(index);
                        var outjson = JsonConvert.SerializeObject(json);
                        File.WriteAllText(Strings.WarnPath, outjson);
                        await ReplyAsync(":white_check_mark: Removed warn from specified user.");
                    }
                    else
                        await ReplyAsync($":anger: There are no warns at index[`{index}`]");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }
    }
}
