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
using XDB.Common.Types;

namespace XDB.Modules
{
    public class Warn : ModuleBase
    {
        [Command("warns")]
        public async Task Warns()
        {
            Config.WarnCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");
            var read = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(read);
            try
            {
                if(!json.Any(x => x.WarnedUser == Context.User.Id))
                {
                    var warn = new UserWarn()
                    {
                        WarnedUser = Context.User.Id,
                        WarnReason = new List<string> { "" }
                    };
                    json.Add(warn);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":anger: You do not have any warnings.");
                } else
                {
                    var warns = json.Find(x => x.WarnedUser == Context.User.Id);
                    if(warns.WarnReason.Any(x => string.IsNullOrEmpty(x))) { warns.WarnReason.RemoveAll(str => string.IsNullOrEmpty(str));  }
                    var warnsout = new StringBuilder();
                    foreach(var warn in warns.WarnReason)
                    {
                        warnsout.AppendLine($"~ {warn}");
                    }
                    await ReplyAsync($"Your current warns >\n{warnsout.ToString()}");
                }
            } catch(Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("warns")]
        public async Task Warns(IGuildUser user)
        {
            Config.WarnCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");
            var read = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(read);
            try
            {
                if (!json.Any(x => x.WarnedUser == user.Id))
                {
                    var warn = new UserWarn()
                    {
                        WarnedUser = user.Id,
                        WarnReason = new List<string> { null }
                    };
                    json.Add(warn);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":anger: You do not have any warnings.");
                }
                else
                {
                    var warns = json.Find(x => x.WarnedUser == user.Id);
                    if (warns.WarnReason.Any(x => string.IsNullOrEmpty(x))) { warns.WarnReason.RemoveAll(str => string.IsNullOrEmpty(str)); }
                    var warnsout = new StringBuilder();
                    foreach (var warn in warns.WarnReason)
                    {
                        warnsout.AppendLine($"~ {warn}");
                    }
                    await ReplyAsync($"{user.Username}'s warns >\n{warnsout.ToString()}");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("warn")]
        public async Task AddWarn(IGuildUser user, [Remainder] string reason)
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");
            var read = File.ReadAllText(path);
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
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":white_check_mark: Warned the specified user.");
                    var dm = await user.CreateDMChannelAsync();
                    await dm.SendMessageAsync($"You have been warned by **{Context.User.Username}** in **{Context.Guild.Name}** for:\n`{reason}`");
                } else
                {
                    json.First(x => x.WarnedUser == user.Id).WarnReason.Add(reason);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":white_check_mark: Warned the specified user.");
                    var dm = await user.CreateDMChannelAsync();
                    await dm.SendMessageAsync($"You have been warned by **{Context.User.Username}** in **{Context.Guild.Name}** for:\n`{reason}`");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }

        [Command("removewarn")]
        public async Task RemoveWarn(IGuildUser user, int index)
        {
            var path = Path.Combine(AppContext.BaseDirectory, $"warn/warns.json");
            var read = File.ReadAllText(path);
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
                        File.WriteAllText(path, outjson);
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
