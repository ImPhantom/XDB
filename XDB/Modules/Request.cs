using Discord.Commands;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using XDB.Common.Types;
using XDB.Utilities;
using Newtonsoft.Json;
using XDB.Common.Models;
using System.Linq;
using Discord;
using Humanizer;
using XDB.Common.Attributes;

namespace XDB.Modules
{
    public class Request : ModuleBase
    {
        [Command("request")]
        [RequireStaff]
        public async Task RequestBan(string steamId, TimeSpan length, [Remainder] string reason)
        {
            var valid = await ValidateSteamId(steamId);
            if (valid != null)
            {
                await Context.Message.DeleteAsync();
                var id32 = SteamUtil.ToSteam32(steamId);
                var author = new EmbedAuthorBuilder().WithName(valid.PersonaName).WithIconUrl(valid.MediumSizeAvatarUrl);
                var embed = new EmbedBuilder().WithAuthor(author).WithColor(Xeno.RandomColor());
                embed.AddField("SteamIDs:", $"{id32}  ( {valid.Steam64Id} )", false);
                embed.AddField("Requested By:", $"{Context.User.Username}#{Context.User.Discriminator}", true);
                embed.AddField("Duration:", $"{length.Humanize()}", true);
                embed.AddField("Reason:", $"{reason} (Banned on behalf of: {Context.User.Username})");
                embed.AddField("Command:", $"`ulx banid {id32} {length.TotalMinutes} \"{reason}\"`");
                await ReplyAsync("<@!169591360695959553>", embed: embed.Build());
            } else
            {
                await ReplyAsync("", embed: Xeno.ErrorEmbed("Could not validate SteamID"));
            }
        }

        private async Task<PlayerSummary> ValidateSteamId(string input)
        {
            ulong communityId;

            if (input.StartsWith("STEAM_"))
                communityId = ulong.Parse(SteamUtil.Steam32ToSteam64(input));
            else if (input.Length == 17 && input.StartsWith("7656"))
                communityId = ulong.Parse(input);
            else
                return null;

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={Config.Load().SteamApiKey}&steamids={communityId}");
                var parsed = JsonConvert.DeserializeObject<PlayerSummariesResponse>(response);
                if (parsed.Response.Players.Count > 0)
                    return parsed.Response.Players.First();
                else
                    return null;
            }
        }
    }
}
