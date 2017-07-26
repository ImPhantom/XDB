using System;
using System.Collections.Generic;
using System.Text;
using Steam.Models.SteamCommunity;
using Discord;
using SteamWebAPI2.Interfaces;
using XDB.Common.Types;
using System.Threading.Tasks;

namespace XDB.Utilities
{
    public class SteamUtil
    {
        // Steam ID handling
        private static bool IsSteamId32(string id)
            => id.StartsWith("STEAM_0:");

        private static bool IsSteamId64(string id)
            => (id.Length == 17) && id.StartsWith("7656");

        public static bool IsSteamURL(string id)
        {
            string url = id.Replace("https://", "").Replace("http://", "");
            return url.StartsWith("steamcommunity.com/id/") || url.StartsWith("steamcommunity.com/profiles/");
        }

        private static string ToSteamID32(string input)
        {
            Int64 steamId1 = Convert.ToInt64(input.Substring(0)) % 2;
            Int64 steamId2a = Convert.ToInt64(input.Substring(0, 4)) - 7656;
            Int64 steamId2b = Convert.ToInt64(input.Substring(4)) - 1197960265728;
            steamId2b = steamId2b - steamId1;

            return "STEAM_0:" + steamId1 + ":" + ((steamId2a + steamId2b) / 2);
        }

        private static string ToSteamID64(string input)
        {
            string[] split = input.Replace("STEAM_", "").Split(':');
            return (76561197960265728 + (Convert.ToInt64(split[2]) * 2) + Convert.ToInt64(split[1])).ToString();
        }

        public static async Task<ulong> ParseSteamId(string input)
        {
            var _api = new SteamUser(Config.Load().SteamApiKey);

            if (IsSteamId32(input))
                return ulong.Parse(ToSteamID64(input));
            else if (IsSteamId64(input))
                return ulong.Parse(input);
            else if (!IsSteamURL(input))
            {
                var id = await _api.ResolveVanityUrlAsync(input);
                return id.Data;
            }
            else
                return 0;
        }


        public static Color GetActivityColor(Steam.Models.SteamCommunity.UserStatus status, string playing)
        {
            if (status == Steam.Models.SteamCommunity.UserStatus.Offline)
                return new Color(86, 86, 86);
            else if (status == Steam.Models.SteamCommunity.UserStatus.Online && playing != null)
                return new Color(143, 185, 59);
            else if (status == Steam.Models.SteamCommunity.UserStatus.Online && playing == null)
                return new Color(83, 164, 196);
            else
                return new Color(83, 164, 196);
        }

        public static string GetServerOS(string os)
        {
            switch (os)
            {
                case "l":
                    return "Linux";
                case "w":
                    return "Windows";
                case "m":
                    return "Mac";
                case "o":
                    return "Mac";
                default:
                    return "Invalid OS Code";
            }
        }
    }
}
