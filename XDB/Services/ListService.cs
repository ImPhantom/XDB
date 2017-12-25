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
    public class ListService
    {
        private DiscordSocketClient _client;

        private List<UserTodo> FetchLists()
            => JsonConvert.DeserializeObject<List<UserTodo>>(File.ReadAllText(Xeno.TodoPath));

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

        public async Task<string> AddTodoListItemAsync(ulong userId, string item)
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

        public async Task<string> EditTodoListItemAsync(ulong userId, int index, string content)
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

        public async Task<string> RemoveTodoListItemAsync(ulong userId, int index)
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

        public async Task<string> ClearTodoListAsync(ulong userId)
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
        }

        public ListService(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
