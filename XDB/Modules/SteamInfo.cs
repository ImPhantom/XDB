using Discord.Commands;
using System.Threading.Tasks;
using System;
using Discord;
using System.Linq;
using SteamWebAPI2.Interfaces;
using XDB.Common.Types;
using XDB.Common;
using System.Diagnostics;
using XDB.Utilities;
using XDB.Common.Attributes;
using Phantom.CSourceQuery;
using System.Net;
using System.Text;
using Humanizer;

namespace XDB.Modules
{
    [Summary("Steam")]
    [RequireContext(ContextType.Guild)]
    public class SteamInfo : XenoBase
    {
        [Ratelimit(3, 1, Measure.Hours)]
        [Command("query"), Alias("sourcequery", "sq"), Summary("Querys a source server for its public information.")]
        public async Task QuerySource(string queryIp)
        {
            var sw = Stopwatch.StartNew();
            var ip = queryIp.Split(':');
            if (ip.Length != 2)
                return;

            var endpoint = new IPEndPoint(IPAddress.Parse(ip[0]), int.Parse(ip[1]));

            var sq = new SourceQuery();
            var serverInfo = sq.QueryServer(endpoint);

            var embed = new EmbedBuilder()
                .WithColor(new Color(29, 140, 209))
                .WithTitle($"{serverInfo.Name}")
                .WithDescription($"To get the full playerlist, run:\n`{Config.Load().Prefix}players {queryIp}`")
                .WithFooter($"Generated in: {sw.ElapsedMilliseconds}ms");
            embed.AddField("Map:", $"`{serverInfo.Map}`", true);
            embed.AddField("Players:", $"{serverInfo.PlayerCount}/{serverInfo.MaxPlayers}", true);
            embed.AddField("Gamemode:", $"{serverInfo.Game}", true);
            embed.AddField("Server OS:", $"{serverInfo.OS} ({serverInfo.Dedicated})", true);
            embed.AddField("VAC Status:", $"{serverInfo.VAC}", true);

            await ReplyAsync("", embed: embed.Build());
        }

        [Ratelimit(3, 1, Measure.Hours)]
        [Command("players"), Alias("sqp"), Summary("Querys a source server for information on its players.")]
        public async Task QueryPlayers(string queryIp)
        {
            var sw = Stopwatch.StartNew();
            var ip = queryIp.Split(':');
            if (ip.Length != 2)
                return;

            var endpoint = new IPEndPoint(IPAddress.Parse(ip[0]), int.Parse(ip[1]));

            var sq = new SourceQuery();
            var serverInfo = sq.QueryServer(endpoint);
            var playerList = sq.QueryPlayers(endpoint);

            var str = new StringBuilder();
            str.AppendLine();

            var maxSize = (playerList.Select(x => x.Name).ToArray()).Max(x => x.Length) + 6;
            foreach(var player in playerList)
            {
                var name = player.Name.PadRight(maxSize);
                str.AppendLine(string.Format("{0}{1}", name, (new TimeSpan(0, 0, Convert.ToInt32(player.Time)).Humanize())));
            }
                

            var embed = new EmbedBuilder()
                .WithColor(new Color(29, 140, 209))
                .WithTitle($"{serverInfo.Name} ({serverInfo.PlayerCount}/{serverInfo.MaxPlayers})")
                .WithDescription($"```{str.ToString()}```")
                .WithFooter($"Generated in: {sw.ElapsedMilliseconds}ms");

            await ReplyAsync("", embed: embed.Build());
        }


        [Command("steamuser"), Alias("suser"), Summary("Provides all information about a steam user.")]
        public async Task FetchSteamUser(string input)
        {
            var _api = new SteamUser(Config.Load().SteamApiKey);
            var id = await SteamUtil.ParseSteamId(input);
            if (id == 0)
            {
                await SendErrorEmbedAsync("Could not parse steamid/vanity url.");
                return;
            }

            var response = await _api.GetPlayerSummaryAsync(id);
            var data = response.Data;

            if (data.ProfileState != 1)
                await SendErrorEmbedAsync("That user has not setup their community profile.");

            var author = new EmbedAuthorBuilder().WithIconUrl(data.AvatarFullUrl).WithName(data.Nickname);
            var embed = new EmbedBuilder().WithAuthor(author).WithFooter($"Info fetched on {DateTime.UtcNow}").WithColor(SteamUtil.GetActivityColor(data.UserStatus, data.PlayingGameName)).WithDescription(data.ProfileUrl);
            embed.AddField("Status:", data.UserStatus.ToString(), true);
            embed.AddField("SteamID:", data.SteamId.ToString(), true);

            if (data.ProfileVisibility == Steam.Models.SteamCommunity.ProfileVisibility.Public)
            {
                embed.AddField("Date Created:", data.AccountCreatedDate.ToString("M/d/yyyy"), true);
                if (data.PlayingGameName != null)
                    embed.AddField("Playing:", data.PlayingGameName, true);
                if (data.RealName != null)
                    embed.AddField("Real Name:", data.RealName, true);
            }
            await ReplyAsync("", embed: embed.Build());
        }

        [Command("steamgame"), Alias("sgame"), Summary("Fetches information about a steam game.")]
        public async Task FetchSteamGame([Remainder] string game)
        {
            var _api = new SteamApps(Config.Load().SteamApiKey);
            var stats = new SteamUserStats(Config.Load().SteamApiKey);
            var _store = new SteamStore();
            var response = await _api.GetAppListAsync();
            var data = response.Data;

            var seps = new char[] { '\'' };

            if (data.Any(x => x.Name.ToLower().Replace("-", " ").Replace(seps, "") == game.ToLower()))
            {
                var app = data.First(x => x.Name.ToLower().Replace("-", " ").Replace(seps, "") == game.ToLower());
                var ret = await stats.GetNumberOfCurrentPlayersForGameAsync(app.AppId);

                var details = await _store.GetStoreAppDetailsAsync(app.AppId);
                var embed = new EmbedBuilder().WithTitle(details.Name).WithThumbnailUrl(details.HeaderImage).WithFooter(new EmbedFooterBuilder().WithText($"Current Players: {ret.Data}, Released: {details.ReleaseDate.Date}")).WithDescription($"{details.AboutTheGame.Substring(0, 675)}...\n\n**Publisher(s):** {string.Join(", ", details.Publishers)}\n**Store Link:** http://store.steampowered.com/app/{details.SteamAppId}/");
                await ReplyAsync("", embed: embed.Build());
            }
            else
            {
                await ReplyAsync(":x: Game not found, try typing the full game title");
            }
        }
    }
}
