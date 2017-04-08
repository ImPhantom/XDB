using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Models;
using XDB.Common.Types;

namespace XDB.Utilities
{
    public class Warning
    {
        public static async Task WarnUserAsync(CommandContext context, SocketGuildUser user, string reason)
        {
            Config.WarnCheck();
            var warns = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(warns);
            var dm = await user.CreateDMChannelAsync();
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
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.WarnPath, _json);
                    await Logging.TryLoggingAsync($":heavy_check_mark: {user.Username} has been warned by {context.User.Username} for:\n`{reason}`");
                    await dm.SendMessageAsync($":heavy_multiplication_x: You have been warned by **{context.User.Username}** in **{context.Guild.Name}** for:\n\n`{reason}`");
                }
                else
                {
                    json.First(x => x.WarnedUser == user.Id).WarnReason.Add(reason);
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.WarnPath, _json);
                    await Logging.TryLoggingAsync($":heavy_check_mark: {user.Username} has been warned by {context.User.Username} for:\n`{reason}`");
                    await dm.SendMessageAsync($":heavy_multiplication_x: You have been warned by **{context.User.Username}** in **{context.Guild.Name}** for:\n\n`{reason}`");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task RemoveWarnAsync(CommandContext context, SocketGuildUser user, int index)
        {
            Config.WarnCheck();
            var warns = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(warns);
            try
            {
                index--;
                if (!json.Any(x => x.WarnedUser == user.Id))
                    await context.Channel.SendMessageAsync(":small_blue_diamond: That user does not have any warns.");
                else
                {
                    if (index < json.First(x => x.WarnedUser == user.Id).WarnReason.Count)
                    {
                        var reason = json.First(x => x.WarnedUser == user.Id).WarnReason[index];
                        json.First(x => x.WarnedUser == user.Id).WarnReason.RemoveAt(index);
                        var _json = JsonConvert.SerializeObject(json);
                        File.WriteAllText(Strings.WarnPath, _json);
                        await Logging.TryLoggingAsync($":heavy_check_mark: {context.User.Username} has removed {user.Username}'s warn for:\n`{reason}`");
                    }
                    else
                        await context.Channel.SendMessageAsync($":small_blue_diamond: There are no warns at index[`{index}`]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task GetUserWarns(CommandContext context, SocketGuildUser user)
        {
            Config.WarnCheck();
            var allwarns = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(allwarns);
            try
            {
                if (!json.Any(x => x.WarnedUser == user.Id))
                    await context.Channel.SendMessageAsync($":small_blue_diamond: {user.Username}'s warns: **0**");
                else
                {
                    var warns = json.Find(x => x.WarnedUser == user.Id);
                    if (!warns.WarnReason.Any()) { await context.Channel.SendMessageAsync($":small_blue_diamond: {user.Username}'s warns: **0**"); return; }
                    var _warns = new StringBuilder();
                    foreach (var warn in warns.WarnReason)
                    {
                        _warns.AppendLine($"~ {warn}");
                    }
                    await context.Channel.SendMessageAsync($"{user.Username} currently has **{warns.WarnReason.Count}** warns.\n```{_warns.ToString()}```");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
