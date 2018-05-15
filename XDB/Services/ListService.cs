using Discord.WebSocket;
using Newtonsoft.Json;
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

        private Dictionary<ulong, List<string>> FetchAllTodoLists()
            => JsonConvert.DeserializeObject<Dictionary<ulong, List<string>>>(File.ReadAllText(Xeno.Todo));

        public string FetchTodoList(ulong userId)
        {
            var lists = FetchAllTodoLists();
            if(lists.TryGetValue(userId, out List<string> todoList))
            {
                if (!todoList.Any())
                    return ":eight_pointed_black_star:  Your todo list is empty.";

                var list = new StringBuilder();
                int count = 1;
                foreach (var item in todoList)
                {
                    list.AppendLine($"{count}. {item}");
                    count++;
                }
                return $":small_blue_diamond: **Your Todo List:**```{list.ToString()}```";
            } else
                return ":eight_pointed_black_star:  Your todo list is empty.";
        }

        public async Task<bool> TryAddListItemAsync(ulong userId, string listItem)
        {
            var lists = FetchAllTodoLists();
            if(lists.TryGetValue(userId, out List<string> todoList))
            {
                todoList.Add(listItem);
                await Xeno.SaveJsonAsync(Xeno.Todo, JsonConvert.SerializeObject(lists));
                return true;
            } else
            {
                lists.Add(userId, new List<string> { listItem });
                await Xeno.SaveJsonAsync(Xeno.Todo, JsonConvert.SerializeObject(lists));
                return true;
            }
        }

        public async Task<bool> TryEditListItemAsync(ulong userId, int itemIndex, string listItem)
        {
            var lists = FetchAllTodoLists();
            if (lists.TryGetValue(userId, out List<string> todoList))
            {
                itemIndex--;
                todoList[itemIndex] = listItem;
                await Xeno.SaveJsonAsync(Xeno.Todo, JsonConvert.SerializeObject(lists));
                return true;
            } else
                return false;
        }

        public async Task<bool> TryRemoveListItemAsync(ulong userId, int itemIndex)
        {
            var lists = FetchAllTodoLists();
            if (lists.TryGetValue(userId, out List<string> todoList))
            {
                itemIndex--;
                todoList.RemoveAt(itemIndex);
                await Xeno.SaveJsonAsync(Xeno.Todo, JsonConvert.SerializeObject(lists));
                return true;
            }
            else
                return false;
        }

        public async Task<bool> TryClearListAsync(ulong userId)
        {
            var lists = FetchAllTodoLists();
            if (lists.TryGetValue(userId, out List<string> todoList))
            {
                todoList.Clear();
                await Xeno.SaveJsonAsync(Xeno.Todo, JsonConvert.SerializeObject(lists));
                return true;
            }
            else
                return false;
        }

        public void Initialize()
        {
            if(!File.Exists(Xeno.Todo))
            {
                var lists = new Dictionary<ulong, List<string>>();
                using (var file = new FileStream(Xeno.Todo, FileMode.Create)) { }
                File.WriteAllText(Xeno.Todo, JsonConvert.SerializeObject(lists));
            }
        }

        public ListService(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
