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
using XDB.Common.Models;
using XDB.Common.Types;

namespace XDB.Utilities
{
    public class Warning
    {
        public static async Task WarnUserAsync(SocketCommandContext context, SocketGuildUser user, string reason)
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
                    var reply = await context.Channel.SendMessageAsync(":ok_hand:");
                    await TimedMessage(reply);
                    await Logging.TryLoggingAsync($":heavy_check_mark: `{user.Username}#{user.Discriminator}` has been warned by `{context.User.Username}#{context.User.Discriminator}` for:\n`{reason}`");
                    await dm.SendMessageAsync($":heavy_multiplication_x: You have been warned by **{context.User.Username}#{context.User.Discriminator}** in **{context.Guild.Name}** for:\n\n`{reason}`");
                }
                else
                {
                    json.First(x => x.WarnedUser == user.Id).WarnReason.Add(reason);
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.WarnPath, _json);
                    var reply = await context.Channel.SendMessageAsync(":ok_hand:");
                    await TimedMessage(reply);
                    await Logging.TryLoggingAsync($":heavy_check_mark: `{user.Username}#{user.Discriminator}` has been warned by `{context.User.Username}#{context.User.Discriminator}` for:\n`{reason}`");
                    await dm.SendMessageAsync($":heavy_multiplication_x: You have been warned by **{context.User.Username}#{context.User.Discriminator}** in **{context.Guild.Name}** for:\n\n`{reason}`");
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Warning", e.ToString());
            }
        }

        public static async Task RemoveWarnAsync(SocketCommandContext context, SocketGuildUser user, int index)
        {
            Config.WarnCheck();
            var warns = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(warns);
            try
            {
                index--;
                if (context.User.Id == user.Id && !Config.Load().Owners.Contains(context.User.Id))
                {
                    await context.Channel.SendMessageAsync(":heavy_multiplication_x:  You cannot remove warns from yourself.");
                    return;
                }
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
                        var reply = await context.Channel.SendMessageAsync(":ok_hand:");
                        await TimedMessage(reply);
                        await Logging.TryLoggingAsync($":heavy_check_mark: `{context.User.Username}#{context.User.Discriminator}` has removed `{user.Username}#{user.Discriminator}`'s warn for:\n`{reason}`");
                    }
                    else
                        await context.Channel.SendMessageAsync($":small_blue_diamond: There are no warns at index[`{index}`]");
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Warning", e.ToString());
            }
        }

        public static async Task GetUserWarns(SocketCommandContext context, SocketGuildUser user)
        {
            Config.WarnCheck();
            var allwarns = File.ReadAllText(Strings.WarnPath);
            var json = JsonConvert.DeserializeObject<List<UserWarn>>(allwarns);
            try
            {
                if (!json.Any(x => x.WarnedUser == user.Id))
                    await context.Channel.SendMessageAsync($":small_blue_diamond: `{user.Username}#{user.Discriminator}`'s warns: **0**");
                else
                {
                    var warns = json.Find(x => x.WarnedUser == user.Id);
                    if (!warns.WarnReason.Any()) { await context.Channel.SendMessageAsync($":small_blue_diamond: `{user.Username}#{user.Discriminator}`'s warns: **0**"); return; }
                    var _warns = new StringBuilder();
                    foreach (var warn in warns.WarnReason)
                    {
                        _warns.AppendLine($"~ {warn}");
                    }
                    await context.Channel.SendMessageAsync($":small_blue_diamond: `{user.Username}#{user.Discriminator}` currently has **{warns.WarnReason.Count}** warns.\n```{_warns.ToString()}```");
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Warning", e.ToString());
            }
        }

        private static async Task TimedMessage(IMessage message, int ms = 2500)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }
}
