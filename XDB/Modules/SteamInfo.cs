using Discord.Commands;
using System.Threading.Tasks;
using System;
using Discord;
using System.Linq;
using SteamWebAPI2.Interfaces;
using XDB.Common.Types;
using XDB.Common;

namespace XDB.Modules
{
    [Summary("Steam")]
    [RequireContext(ContextType.Guild)]
    public class SteamInfo : ModuleBase<SocketCommandContext>
    {
        [Command("steamuser"), Summary("Provides all information about a steam user.")]
        public async Task FetchSteamUser(ulong id)
        {
            var _api = new SteamUser(Config.Load().SteamApiKey);
            var response = await _api.GetPlayerSummaryAsync(id);
            var data = response.Data;

            if (data.ProfileState != 1)
                await ReplyAsync("That user has not setup their community profile.");

            var author = new EmbedAuthorBuilder().WithIconUrl(data.AvatarFullUrl).WithName(data.Nickname);
            var embed = new EmbedBuilder().WithAuthor(author).WithFooter($"Info fetched on {DateTime.UtcNow}").WithColor(GetActivityColor(data.UserStatus, data.PlayingGameName));
            embed.AddInlineField("Status:", data.UserStatus);
            embed.AddInlineField("SteamID:", data.SteamId);
            embed.AddInlineField("Visibility:", data.ProfileVisibility);
            embed.AddInlineField("Date Created:", data.AccountCreatedDate.ToString("M/d/yyyy")); 
            if(data.PlayingGameName != null)
                embed.AddInlineField("Playing:", data.PlayingGameName);

            await ReplyAsync("", embed: embed);
        }

        [Command("steamgame"), Summary("Fetches information about a steam game.")]
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

                await ReplyAsync($"game: {app.Name}, appid: {app.AppId}");
                var details = await _store.GetStoreAppDetailsAsync(app.AppId);
                var embed = new EmbedBuilder().WithTitle(details.Name).WithThumbnailUrl(details.HeaderImage).WithFooter(new EmbedFooterBuilder().WithText($"Current Players: {ret.Data}, Released: {details.ReleaseDate.Date}")).WithDescription($"{details.AboutTheGame.Substring(0, 675)}...\n\n**Publisher(s):** {string.Join(", ", details.Publishers)}\n**Store Link:** http://store.steampowered.com/app/{details.SteamAppId}/");
                await ReplyAsync("", embed: embed);
            } else
            {
                await ReplyAsync("Game not found, try typing the full game title");
            }
        }

        private Color GetActivityColor(Steam.Models.SteamCommunity.UserStatus status, string playing)
        {
            if (status == Steam.Models.SteamCommunity.UserStatus.Offline)
                return new Color(86, 86, 86);
            else if (status == Steam.Models.SteamCommunity.UserStatus.Online && playing != null)
                return new Color(143, 185, 59);
            else if (status == Steam.Models.SteamCommunity.UserStatus.Online && playing == null)
               return new Color(83, 164, 196);
            else
                return new Color(86, 86, 86);
        }
    }
}
