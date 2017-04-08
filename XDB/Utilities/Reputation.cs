using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Types;

namespace XDB.Utilities
{
    public class Reputation
    {
        public static async Task CheckReputationAsync(CommandContext context, SocketGuildUser user)
        {
            Config.RepCheck();
            var reputations = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(reputations);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    json.Add(new UserRep() { Id = user.Id, Rep = 0 });
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, _json);
                }
                var rep = json.First(x => x.Id == user.Id).Rep;
                if (rep < 0)
                    await context.Channel.SendMessageAsync($":black_medium_small_square: **{user.Username}'s** reputation: {rep}");
                else
                    await context.Channel.SendMessageAsync($":white_medium_small_square: **{user.Username}'s** reputation: {rep}");

                await context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task GetLeaderboardAsync(CommandContext context)
        {
            // TODO remove users from leaderboard that are not in the guild/unknown
            Config.RepCheck();
            var reputations = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(reputations);

            var topreps = json.OrderByDescending(x => x.Rep).Take(10);
            var embed = new EmbedBuilder().WithColor(new Color(21, 144, 232));
            foreach (var rep in topreps)
            {
                var user = await context.Client.GetUserAsync(rep.Id);
                if (user == null)
                {
                    embed.AddField(x => {
                        x.Name = "**unknown_user**";
                        x.Value = $"Reputation: {rep.Rep}";
                        x.IsInline = false;
                    });
                }
                else
                {
                    embed.AddField(x => {
                        x.Name = $"**{user.Username}**";
                        x.Value = $"Reputation: {rep.Rep}";
                        x.IsInline = false;
                    });
                }
            }
            await context.Channel.SendMessageAsync(":star: **Top 10 Reputations:**", false, embed.Build());
        }

        public static async Task AddReputationAsync(CommandContext context, SocketGuildUser user)
        {
            Config.RepCheck();
            var reputations = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(reputations);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    json.Add(new UserRep() { Id = user.Id, Rep = 1 });
                    await context.Channel.SendMessageAsync($":white_medium_small_square: **{user.Username}'s** reputation: 1");
                    File.WriteAllText(Strings.RepPath, JsonConvert.SerializeObject(json));
                }
                else
                {
                    json.First(x => x.Id == user.Id).Rep++;
                    var rep = json.First(x => x.Id == user.Id).Rep;
                    if (rep < 0)
                        await context.Channel.SendMessageAsync($":black_medium_small_square: **{user.Username}'s** reputation: {rep}");
                    else
                        await context.Channel.SendMessageAsync($":white_medium_small_square: **{user.Username}'s** reputation: {rep}");
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, _json);
                }
                await context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task RemoveReputationAsync(CommandContext context, SocketGuildUser user)
        {
            Config.RepCheck();
            var reputations = File.ReadAllText(Strings.RepPath);
            var json = JsonConvert.DeserializeObject<List<UserRep>>(reputations);
            try
            {
                if (!json.Any(x => x.Id == user.Id))
                {
                    json.Add(new UserRep() { Id = user.Id, Rep = -1 });
                    await context.Channel.SendMessageAsync($":black_medium_small_square: **{user.Username}'s** reputation: -1");
                    File.WriteAllText(Strings.RepPath, JsonConvert.SerializeObject(json));
                }
                else
                {
                    json.First(x => x.Id == user.Id).Rep--;
                    var rep = json.First(x => x.Id == user.Id).Rep;
                    if (rep < 0)
                        await context.Channel.SendMessageAsync($":black_medium_small_square: **{user.Username}'s** reputation: {rep}");
                    else
                        await context.Channel.SendMessageAsync($":white_medium_small_square: **{user.Username}'s** reputation: {rep}");
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.RepPath, _json);
                }
                await context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
