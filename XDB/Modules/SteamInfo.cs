using Discord.Commands;
using System.Threading.Tasks;
using System;
using Discord;
using System.Linq;
using SteamWebAPI2.Interfaces;
using XDB.Common.Types;
using XDB.Common;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Steam")]
    [RequireContext(ContextType.Guild)]
    public class SteamInfo : ModuleBase<SocketCommandContext>
    {
        [Command("query"), Summary("Querys a source server for its public information.")]
        public async Task QuerySourceServer(string queryIp)
        {
            var sw = Stopwatch.StartNew();
            var ip = queryIp.Split(':');
            if (ip.Length != 2)
                return;

            var url = $"http://45.63.78.183/query.php?ip={ip[0]}&port={ip[1]}";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                var stream = await content.ReadAsStreamAsync();
                var reader = new StreamReader(stream);
                string _streamContent;
                while ((_streamContent = reader.ReadLine()) != null)
                {
                    string vac;
                    _streamContent = Regex.Replace(_streamContent, "<br />", Environment.NewLine);
                    string[] _content = _streamContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    if (_content[6] == "1") vac = "Yes"; else vac = "No";
                    sw.Stop();
                    var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithDescription($"__**{_content[1]}**__\n\n").WithFooter($"Generated in: {sw.ElapsedMilliseconds}ms");
                    embed.AddField("Map:", $"`{_content[4]}`", true);
                    embed.AddField("Players:", $"{_content[2]}/{_content[3]}", true);
                    embed.AddField("Game:", _content[0], true);
                    embed.AddField("Gamemode:", $"`{_content[5]}`", true);
                    embed.AddField("OS:", SteamUtil.GetServerOS(_content[7]), true);
                    embed.AddField("VAC Secure:", vac, true);

                    await ReplyAsync("" , embed: embed.Build());
                }
            }
        }

        [Command("steamuser"), Alias("suser"), Summary("Provides all information about a steam user.")]
        public async Task FetchSteamUser(string input)
        {
            var _api = new SteamUser(Config.Load().SteamApiKey);
            var id = await SteamUtil.ParseSteamId(input);
            if (id == 0)
            {
                await ReplyAsync("", embed: Xeno.ErrorEmbed("Could not parse steamid/vanity url"));
                return;
            }

            var response = await _api.GetPlayerSummaryAsync(id);
            var data = response.Data;

            if (data.ProfileState != 1)
                await ReplyAsync("That user has not setup their community profile.");

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
            } else
            {
                await ReplyAsync(":x: Game not found, try typing the full game title");
            }
        }
    }
}
