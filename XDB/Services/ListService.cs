using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Models;

namespace XDB.Services
{
    //
    //      TODO: Permissions for channel lists (as of now, anyone in the channel can edit/remove/clear/add list items)
    //      

    public class ListService
    {
        private DiscordSocketClient _client;

        public List<UserTodo> FetchLists()
            => JsonConvert.DeserializeObject<List<UserTodo>>(File.ReadAllText(Xeno.TodoPath));

        public List<ChannelTodo> FetchChannelLists()
            => JsonConvert.DeserializeObject<List<ChannelTodo>>(File.ReadAllText(Xeno.DevListPath));

        #region Fetching Lists

        // To fetch all lists depending on channel
        public string FetchListContent(ulong userId, ulong channelId)
        {
            var _todo = FetchLists();
            var _devc = FetchChannelLists();
            if (_devc.Any(x => x.ChannelId == channelId))
                return FetchChannelListItems(channelId);
            else
                return FetchListItems(userId);
            // Check for null in ~todo then it failed to get either devchannel list or todo list
        }

        // For dev channel lists

        public string FetchChannelListItems(ulong channelId)
        {
            try
            {
                var _in = FetchChannelLists();
                if (_in.Any(x => x.ChannelId == channelId))
                {
                    var list = _in.First(x => x.ChannelId == channelId);
                    if (!list.Items.Any())
                        return ":eight_pointed_black_star:  This todo list is empty.";

                    var ol = new StringBuilder();
                    int count = 1;
                    foreach (var item in list.Items)
                    {
                        ol.AppendLine($"{count}. {item}");
                        count++;
                    }
                    return $":small_blue_diamond: **Channel Todo List:**```{ol.ToString()}```";
                }
                else
                    return ":eight_pointed_black_star:  This todo list is empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        // For Todo Lists

        public string FetchListItems(ulong userId)
        {
            try
            {
                var _in = FetchLists();
                if (_in.Any(x => x.Id == userId))
                {
                    var list = _in.First(x => x.Id == userId);
                    if (!list.ListItems.Any())
                        return ":eight_pointed_black_star:  Your todo list is empty.";

                    var ol = new StringBuilder();
                    int count = 1;
                    foreach (var item in list.ListItems)
                    {
                        ol.AppendLine($"{count}. {item}");
                        count++;
                    }
                    return $":small_blue_diamond: **Your Todo List:**```{ol.ToString()}```";
                }
                else
                    return ":eight_pointed_black_star:  Your todo list is empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }
        #endregion

        #region Adding List Items
        public async Task<string> AddListItemAsync(ulong userId, ulong guildId, ulong channelId, string item)
        {
            var _todo = FetchLists();
            var _dev = FetchChannelLists();
            if (_dev.Any(x => x.ChannelId == channelId))
                return await AddChannelListItemAsync(guildId, channelId, item);
            else
                return await AddTodoListItemAsync(userId, item);
        }

        private async Task<string> AddTodoListItemAsync(ulong userId, string item)
        {
            try
            {
                var _in = FetchLists();
                if (!_in.Any(x => x.Id == userId))
                {
                    var list = new UserTodo() { Id = userId, ListItems = new List<string> { item } };
                    _in.Add(list);
                    var _out = JsonConvert.SerializeObject(_in);
                    await Save(Xeno.TodoPath, _out);
                    return FetchListItems(userId);
                }
                else
                {
                    _in.First(x => x.Id == userId).ListItems.Add(item);
                    var _out = JsonConvert.SerializeObject(_in);
                    await Save(Xeno.TodoPath, _out);
                    return FetchListItems(userId);
                }

            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        private async Task<string> AddChannelListItemAsync(ulong guildId, ulong channelId, string item)
        {
            try
            {
                var _in = FetchChannelLists();
                _in.First(x => x.ChannelId == channelId).Items.Add(item);
                var _out = JsonConvert.SerializeObject(_in);
                await Save(Xeno.DevListPath, _out);
                await UpdateChannelTopic(_in.First(x => x.ChannelId == channelId));
                return FetchChannelListItems(_in.First(x => x.ChannelId == channelId).ChannelId);
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }
        #endregion

        #region Edit List Items
        public async Task<string> EditListItemAsync(ulong userId, ulong guildId, ulong channelId, int index, string content)
        {
            var _todo = FetchLists();
            var _dev = FetchChannelLists();
            if (_dev.Any(x => x.ChannelId == channelId))
                return await EditChannelListItemAsync(channelId, index, content);
            else
                return await EditTodoListItemAsync(userId, index, content);
        }

        private async Task<string> EditTodoListItemAsync(ulong userId, int index, string content)
        {
            try
            {
                var _in = FetchLists();
                if (_in.Any(x => x.Id == userId))
                {
                    if (!_in.First(x => x.Id == userId).ListItems.Any())
                        return ":eight_pointed_black_star:  Your todo list is empty.";
                    index--;
                    if (index < _in.First(x => x.Id == userId).ListItems.Count)
                    {
                        _in.First(x => x.Id == userId).ListItems[index] = content;
                        var _out = JsonConvert.SerializeObject(_in);
                        await Save(Xeno.TodoPath, _out);
                        return FetchListItems(userId);
                    }
                    else
                        return ":heavy_multiplication_x:  There is no list item for that index.";
                }
                else
                    return ":eight_pointed_black_star:  Your todo list is empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        private async Task<string> EditChannelListItemAsync(ulong channelId, int index, string content)
        {
            try
            {
                var _in = FetchChannelLists();
                if (_in.Any(x => x.ChannelId == channelId))
                {
                    if (!_in.First(x => x.ChannelId == channelId).Items.Any())
                        return ":eight_pointed_black_star:  Your todo list is empty.";
                    index--;
                    if (index < _in.First(x => x.ChannelId == channelId).Items.Count)
                    {
                        _in.First(x => x.ChannelId == channelId).Items[index] = content;
                        var _out = JsonConvert.SerializeObject(_in);
                        await Save(Xeno.DevListPath, _out);
                        await UpdateChannelTopic(_in.First(x => x.ChannelId == channelId));
                        return FetchChannelListItems(_in.First(x => x.ChannelId == channelId).ChannelId);
                    }
                    else
                        return ":heavy_multiplication_x:  There is no list item for that index.";
                }
                else
                    return ":eight_pointed_black_star:  The todo list is empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        #endregion

        #region Remove List Items
        public async Task<string> RemoveListItemAsync(ulong userId, ulong guildId, ulong channelId, int index)
        {
            var _todo = FetchLists();
            var _dev = FetchChannelLists();
            if (_dev.Any(x => x.ChannelId == channelId))
                return await RemoveChannelListItemAsync(channelId, index);
            else
                return await RemoveTodoListItemAsync(userId, index);
        }

        private async Task<string> RemoveTodoListItemAsync(ulong userId, int index)
        {
            try
            {
                var _in = FetchLists();
                if (_in.Any(x => x.Id == userId))
                {
                    if (!_in.First(x => x.Id == userId).ListItems.Any())
                        return ":eight_pointed_black_star:  Your todo list is empty.";
                    index--;
                    if (index < _in.First(x => x.Id == userId).ListItems.Count)
                    {
                        _in.First(x => x.Id == userId).ListItems.RemoveAt(index);
                        var _out = JsonConvert.SerializeObject(_in);
                        await Save(Xeno.TodoPath, _out);
                        return FetchListItems(userId);
                    }
                    else
                        return ":heavy_multiplication_x:  There is no list item for that index.";
                }
                else
                    return ":eight_pointed_black_star:  Your todo list is empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        private async Task<string> RemoveChannelListItemAsync(ulong channelId, int index)
        {
            try
            {
                var _in = FetchChannelLists();
                if (_in.Any(x => x.ChannelId == channelId))
                {
                    if (!_in.First(x => x.ChannelId == channelId).Items.Any())
                        return ":eight_pointed_black_star:  The todo list is empty.";
                    index--;
                    if (index < _in.First(x => x.ChannelId == channelId).Items.Count)
                    {
                        _in.First(x => x.ChannelId == channelId).Items.RemoveAt(index);
                        var _out = JsonConvert.SerializeObject(_in);
                        await Save(Xeno.DevListPath, _out);
                        await UpdateChannelTopic(_in.First(x => x.ChannelId == channelId));
                        return FetchChannelListItems(_in.First(x => x.ChannelId == channelId).ChannelId);
                    }
                    else
                        return ":heavy_multiplication_x:  There is no list item for that index.";
                }
                else
                    return ":eight_pointed_black_star:  The todo list is empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }
        #endregion

        #region Clear List
        public async Task<string> ClearListAsync(ulong userId, ulong channelId)
        {
            var _todo = FetchLists();
            var _dev = FetchChannelLists();
            if (_dev.Any(x => x.ChannelId == channelId))
                return await ClearChannelListAsync(channelId);
            else
                return await ClearTodoListAsync(userId);
        }

        private async Task<string> ClearTodoListAsync(ulong userId)
        {
            try
            {
                var _in = FetchLists();
                if (_in.Any(x => x.Id == userId))
                {
                    if (!_in.First(x => x.Id == userId).ListItems.Any())
                        return ":eight_pointed_black_star:  Your todo list is already empty.";
                    else
                    {
                        _in.First(x => x.Id == userId).ListItems.Clear();
                        var _out = JsonConvert.SerializeObject(_in);
                        await Save(Xeno.TodoPath, _out);
                        return ":heavy_check_mark: Successfully cleared your todo list.";
                    }
                }
                else
                    return ":eight_pointed_black_star:  Your todo list is already empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        private async Task<string> ClearChannelListAsync(ulong channelId)
        {
            try
            {
                var _in = FetchChannelLists();
                if (_in.Any(x => x.ChannelId == channelId))
                {
                    if (!_in.First(x => x.ChannelId == channelId).Items.Any())
                        return ":eight_pointed_black_star:  This todo list is already empty.";
                    else
                    {
                        _in.First(x => x.ChannelId == channelId).Items.Clear();
                        var _out = JsonConvert.SerializeObject(_in);
                        await Save(Xeno.DevListPath, _out);
                        await UpdateChannelTopic(_in.First(x => x.ChannelId == channelId));
                        return ":heavy_check_mark: Successfully cleared the todo list.";
                    }
                }
                else
                    return ":eight_pointed_black_star:  This todo list is already empty.";
            }
            catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }
        #endregion

        #region Channel Topic Handling
        private async Task UpdateChannelTopic(ChannelTodo todo)
        {
            var guild = _client.GetGuild(todo.GuildId);
            var channel = guild.GetTextChannel(todo.ChannelId);
            var todoList = new StringBuilder();
            todoList.Append(":small_blue_diamond: Channel Todo List: (click here)\r\n------------------------------------------------------\r\n");
            int count = 1;
            foreach(var item in todo.Items)
            {
                todoList.AppendLine($"{count}.) {item}\r\n");
                count++;
            }
            await channel.ModifyAsync(x => x.Topic = todoList.ToString());
        }
        #endregion

        public async Task<string> CreateChannelListAsync(ulong guildId, ulong channelId)
        {
            try
            {
                var _in = FetchChannelLists();
                if (_in.Any(x => x.ChannelId == channelId))
                    return ":eight_pointed_black_star:  This channel is already a development channel.";
                var list = new ChannelTodo()
                {
                    GuildId = guildId,
                    ChannelId = channelId,
                    Items = new List<string> { }
                };
                _in.Add(list);
                var _out = JsonConvert.SerializeObject(_in);
                await Save(Xeno.DevListPath, _out);
                await UpdateChannelTopic(list);
                return ":heavy_check_mark: Successfully marked this as a dev channel.";
            } catch (Exception ex)
            {
                BetterConsole.LogError("List Service", $"{ex.Message}{ex.StackTrace}");
                return $"{ex.Message}";
            }
        }

        private async Task Save(string path, string json)
        {
            using (var stream = new FileStream(path, FileMode.Truncate))
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(json);
                }
            }
        }

        public void Initialize()
        {
            if (!File.Exists(Xeno.TodoPath))
            {
                List<UserTodo> lists = new List<UserTodo>();
                using (var file = new FileStream(Xeno.TodoPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.TodoPath, JsonConvert.SerializeObject(lists));
            }

            if (!File.Exists(Xeno.DevListPath))
            {
                List<ChannelTodo> chLists = new List<ChannelTodo>();
                using (var file = new FileStream(Xeno.TodoPath, FileMode.Create)) { }
                File.WriteAllText(Xeno.DevListPath, JsonConvert.SerializeObject(chLists));
            }
        }

        public ListService(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
