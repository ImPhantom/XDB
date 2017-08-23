using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace XDB.Modules
{
    [Summary("Chat")]
    [RequireContext(ContextType.Guild)]
    public class Chat : ModuleBase<SocketCommandContext>
    {
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
            embed.AddInlineField("Owner:", guild.Owner.Mention);
            embed.AddInlineField("Guild ID:", $"`{guild.Id}`");
            embed.AddInlineField("Voice Region:", $"`{guild.VoiceRegionId}`");
            embed.AddInlineField("Created:", guild.CreatedAt.ToString("M/dd/yyyy"));
            embed.AddInlineField("Roles:", guild.Roles.Count);
            embed.AddInlineField("Users:", guild.MemberCount);
            embed.AddInlineField("Text Channels:", guild.TextChannels.Count);
            embed.AddInlineField("Voice Channels:", guild.VoiceChannels.Count);

            return embed;
        }

        private Embed FetchUserInfoEmbed(SocketGuildUser user)
        {
            var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithAuthor(user.Username, user.GetAvatarUrl()).WithCurrentTimestamp();
            embed.AddInlineField("Username:", $"`{user.Username}#{user.Discriminator}`");
            embed.AddInlineField("ID:", $"`{user.Id}`");
            embed.AddInlineField("Created:", user.CreatedAt.ToString("M/d/yyyy"));
            embed.AddInlineField("Joined:", user.JoinedAt.Value.ToString("M/d/yyyy"));
            embed.AddInlineField("Status:", user.Status);
            embed.AddInlineField("Game:", Xeno.GetUserGame(user));
            if (user.VoiceChannel != null)
                embed.AddInlineField("Voice State:", Xeno.GetVoiceState(user));

            return embed;
        }
    }
}
