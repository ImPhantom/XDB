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
            Random rand = new Random();
            var response = Strings.EightBallResponses[rand.Next(Strings.EightBallResponses.Length)];
            await ReplyAsync($"**You asked:** `{question}` \n**Reponse:** *{response}*");
        }

        [Command("userinfo"), Summary("Displays your user information.")]
        public async Task UserInfo()
        {
            var date = $"{Context.User.CreatedAt.Month}/{Context.User.CreatedAt.Day}/{Context.User.CreatedAt.Year}";
            var avurl = Context.User.GetAvatarUrl();
            var user = new EmbedAuthorBuilder()
            {
                Name = Context.User.Username,
                IconUrl = avurl
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Author = user
            };
            embed.AddField(x =>
            {
                x.Name = "Username:";
                x.Value = $"{Context.User.Username}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Discriminator:";
                x.Value = $"{Context.User.Discriminator}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "ID:";
                x.Value = $"`{Context.User.Id}`";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Created:";
                x.Value = date;
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Status:";
                x.Value = $"{Context.User.Status}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Game:";
                x.Value = $"{GetUserGame(Context)}";
                x.IsInline = true;
            });
            await ReplyAsync("", false, embed.Build());
        }

        [Command("userinfo"), Summary("Displays a specified users information.")]
        [Name("userinfo `<user>`")]
        public async Task UserInfo(SocketUser user)
        {
            var date = $"{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}";
            var avurl = user.GetAvatarUrl();
            var auth = new EmbedAuthorBuilder()
            {
                Name = user.Username,
                IconUrl = avurl
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                Author = auth
            };
            embed.AddField(x =>
            {
                x.Name = "Username:";
                x.Value = $"{user.Username}";
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Discriminator:";
                x.Value = $"{user.Discriminator}";
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
                x.Value = date;
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
                x.Value = $"{GetUserGame(user)}";
                x.IsInline = true;
            });
            await ReplyAsync("", false, embed.Build());
        }

        [Command("serverinfo"), Summary("Display's the current guild's information.")]
        public async Task ServerInfo()
        {
            var guild = Context.Guild as SocketGuild;
            var date = $"{guild.CreatedAt.Month}/{guild.CreatedAt.Day}/{guild.CreatedAt.Year}";

            var footer = new EmbedFooterBuilder()
            {
                Text = $"Server ID: {guild.Id}"
            };
            var embed = new EmbedBuilder()
            {
                Color = new Color(29, 140, 209),
                ThumbnailUrl = guild.IconUrl,
                Footer = footer
            };
            embed.AddField(x =>
            {
                x.Name = "Owner:";
                x.Value = guild.Owner.Mention;
                x.IsInline = true;
            });
            embed.AddField(x =>
            {
                x.Name = "Server Name:";
                x.Value = $"{guild.Name}";
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
                x.Value = date;
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
        
        private string GetUserGame(CommandContext ctx)
        {
            if(!ctx.User.Game.HasValue)
                return $"`N/A`";
            else
                return $"`{ctx.User.Game}`";
        }

        private string GetUserGame(SocketUser user)
        {
            if (!user.Game.HasValue)
                return $"`N/A`";
            else
                return $"`{user.Game}`";
        }
    }
}
