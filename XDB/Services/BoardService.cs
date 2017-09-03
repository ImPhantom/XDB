using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using XDB.Common.Models;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Linq;
using System;
using Discord;

namespace XDB.Services
{
    public class BoardService
    {
        private DiscordSocketClient _client;

        public static List<BoardMessage> FetchMessages()
            => JsonConvert.DeserializeObject<List<BoardMessage>>(File.ReadAllText(Xeno.CringePath));

        public async Task AddBoardMessageAsync(BoardMessage message)
        {
            var guild = _client.Guilds.First();
            var channel = guild.Channels.First(x => x.Name == "cringe-hof") as SocketTextChannel;
            var user = _client.GetUser(message.UserId);

            var author = new EmbedAuthorBuilder().WithName(user.Username).WithIconUrl(user.GetAvatarUrl());
            var embed = new EmbedBuilder().WithAuthor(author).WithDescription(message.Message).WithTimestamp(message.Timestamp).WithColor(new Color(71, 237, 149));
            await channel.SendMessageAsync("", embed: embed.Build());

            var _in = FetchMessages();
            try
            {
                _in.Add(message);
                var _out = JsonConvert.SerializeObject(_in);
                using (var stream = new FileStream(Xeno.CringePath, FileMode.Truncate))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(_out);
                    }
                }
            }
            catch (Exception e)
            {
                BetterConsole.LogError("Board", e.ToString());
            }
        }

        public async Task CheckChannelExistence()
        {
            foreach (var guild in _client.Guilds)
            {
                if (!guild.Channels.Any(x => x.Name == "cringe-hof"))
                    await guild.CreateTextChannelAsync("cringe-hof");
            }
        }

        public void Initialize()
        {
            if (!File.Exists(Xeno.CringePath))
            {
                List<BoardMessage> _messages = new List<BoardMessage>();
                using (var file = new FileStream(Xeno.CringePath, FileMode.Create)) { }
                File.WriteAllText(Xeno.CringePath, JsonConvert.SerializeObject(_messages));
            }
        }

        public BoardService(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
