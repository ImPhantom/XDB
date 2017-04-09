using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using XDB.Common.Types;

namespace XDB.Modules
{
    [Summary("Chat")]
    [RequireContext(ContextType.Guild)]
    public class Chat : ModuleBase
    {
        [Command("8ball"), Summary("Asks the magic 8 ball a question.")]
        public async Task EightBall([Remainder] string question)
        {
            Random random = new Random();
            var response = Strings.EightBallResponses[random.Next(Strings.EightBallResponses.Length)];
            await ReplyAsync($":8ball: {response}");
        }

        [Command("userinfo"), Summary("Displays your user information.")]
        public async Task UserInfo()
            => await GetUserInfo(Context, Context.User as SocketGuildUser);

        [Command("userinfo"), Summary("Displays a specified users information.")]
        [Name("userinfo `<user>`")]
        public async Task UserInfo(SocketGuildUser user)
            => await GetUserInfo(Context, user);

        [Command("guild"), Alias("server"), Summary("Display's the current guild's information.")]
        public async Task GuildInfo()
        {
            var guild = Context.Guild as SocketGuild;
            var author = new EmbedAuthorBuilder().WithIconUrl(guild.IconUrl).WithName(guild.Name);
            var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithAuthor(author).WithCurrentTimestamp();
            embed.AddField(x =>
            {
                x.Name = "Owner:";
                x.Value = guild.Owner.Mention;
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Guild ID:";
                x.Value = $"`{guild.Id}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Voice Region:";
                x.Value = $"`{guild.VoiceRegionId}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Created:";
                x.Value = $"{guild.CreatedAt.Month}/{guild.CreatedAt.Day}/{guild.CreatedAt.Year}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Roles:";
                x.Value = $"{guild.Roles.Count}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Users:";
                x.Value = $"{guild.MemberCount}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Text Channels:";
                x.Value = $"{guild.TextChannels.Count}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Voice Channels:";
                x.Value = $"{guild.VoiceChannels.Count}";
                x.IsInline = true;
            });
            await ReplyAsync("", false, embed.Build());
        }



        private async Task GetUserInfo(CommandContext context, SocketGuildUser user)
        {
            var author = new EmbedAuthorBuilder().WithName(Context.User.Username).WithIconUrl(Context.User.GetAvatarUrl());
            var embed = new EmbedBuilder().WithColor(new Color(29, 140, 209)).WithAuthor(author).WithCurrentTimestamp();

            embed.AddField(x =>
            {
                x.Name = "Username:";
                x.Value = $"`{user.Username}#{user.Discriminator}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "ID:";
                x.Value = $"`{user.Id}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Created:";
                x.Value = $"{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Joined:";
                x.Value = $"{user.JoinedAt.Value.Month}/{user.JoinedAt.Value.Day}/{user.JoinedAt.Value.Year}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Status:";
                x.Value = $"{user.Status}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Game:";
                x.Value = $"{Strings.GetUserGame(user)}";
                x.IsInline = true;
            });
            if (user.VoiceChannel != null)
            {
                embed.AddField(x =>
                {
                    x.Name = "Voice State: ";
                    x.Value = $"{Strings.GetVoiceState(user)}";
                    x.IsInline = true;
                });
            }
            await ReplyAsync("", false, embed.Build());
        }
    }
}
