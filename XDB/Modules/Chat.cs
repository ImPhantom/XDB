using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using XDB.Common.Attributes;

namespace XDB.Modules
{
    [Summary("Chat")]
    [RequireContext(ContextType.Guild)]
    public class Chat : ModuleBase<SocketCommandContext>
    {
        [Command("help"), Summary("Displays the XDB Changelog for this version.")]
        public async Task Help()
            => await ReplyAsync("You can find a command list here:\n https://github.com/ImPhantom/XDB/wiki/XDB-Command-List");

        [Command("8ball"), Summary("Asks the magic 8 ball a question.")]
        public async Task EightBall([Remainder] string question)
        {
            Random random = new Random();
            var response = Xeno.EightBallResponses[random.Next(Xeno.EightBallResponses.Length)];
            await ReplyAsync($":8ball: {response}");
        }

        [Command("user"), Summary("Displays your user information.")]
        public async Task UserInfo()
            => await ReplyAsync("", embed: FetchUserInfoEmbed(Context.User as SocketGuildUser));

        [Command("user"), Summary("Displays a specified users information.")]
        public async Task UserInfo(SocketGuildUser user)
            => await ReplyAsync("", embed: FetchUserInfoEmbed(user));

        [Command("guild"), Alias("server"), Summary("Display's the current guild's information.")]
        public async Task GuildInfo()
            => await ReplyAsync("", embed: FetchGuildInfoEmbed(Context.Guild));

        private Embed FetchGuildInfoEmbed(SocketGuild guild)
        {
            var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithAuthor(guild.Name, guild.IconUrl).WithCurrentTimestamp();
            embed.AddField("Owner:", guild.Owner.Mention, true);
            embed.AddField("Guild ID:", $"`{guild.Id}`", true);
            embed.AddField("Voice Region:", $"`{guild.VoiceRegionId}`", true);
            embed.AddField("Created:", guild.CreatedAt.ToString("M/dd/yyyy"), true);
            embed.AddField("Roles:", guild.Roles.Count.ToString(), true);
            embed.AddField("Users:", guild.MemberCount.ToString(), true);
            embed.AddField("Text Channels:", guild.TextChannels.Count.ToString(), true);
            embed.AddField("Voice Channels:", guild.VoiceChannels.Count.ToString(), true);

            return embed.Build();
        }

        private Embed FetchUserInfoEmbed(SocketGuildUser user)
        {
            var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithAuthor(user.Username, user.GetAvatarUrl()).WithCurrentTimestamp();
            embed.AddField("Username:", $"`{user.Username}#{user.Discriminator}`", true);
            embed.AddField("ID:", $"`{user.Id}`", true);
            embed.AddField("Created:", user.CreatedAt.ToString("M/d/yyyy"), true);
            embed.AddField("Joined:", user.JoinedAt.Value.ToString("M/d/yyyy"), true);
            embed.AddField("Status:", user.Status.ToString(), true);
            embed.AddField("Game:", Xeno.GetUserGame(user), true);
            if (user.VoiceChannel != null)
                embed.AddField("Voice State:", Xeno.GetVoiceState(user), true);

            return embed.Build();
        }
    }
}
