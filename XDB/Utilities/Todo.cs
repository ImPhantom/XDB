using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Types;

namespace XDB.Utilities
{
    public class Todo
    {
        public static async Task CheckTodoListAsync(CommandContext context)
        {
            Config.TodoCheck();
            var todolists = File.ReadAllText(Strings.TodoPath);
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(todolists);
            try
            {
                if (!json.Any(x => x.Id == context.User.Id))
                    await context.Channel.SendMessageAsync(":eight_pointed_black_star:  Your todo list is empty.");
                else
                {
                    var ret = json.Find(x => x.Id == context.User.Id);
                    if (!ret.ListItems.Any()) { await context.Channel.SendMessageAsync(":eight_pointed_black_star:  Your todo list is empty."); return; }
                    var list = new StringBuilder();
                    foreach (var item in ret.ListItems)
                    {
                        list.AppendLine($"~ {item}");
                    }
                    await context.Channel.SendMessageAsync($@":small_blue_diamond: **Your Todo List:**\n```{list.ToString()}```");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task AddListItemAsync(CommandContext context, string item)
        {
            Config.TodoCheck();
            var todolists = File.ReadAllText(Strings.TodoPath);
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(todolists);
            try
            {
                if (!json.Any(x => x.Id == context.User.Id))
                {
                    json.Add(new UserTodo() { Id = context.User.Id, ListItems = new List<string> { item } });
                    File.WriteAllText(Strings.TodoPath, JsonConvert.SerializeObject(json));
                    await context.Channel.SendMessageAsync(":heavy_check_mark:  Added item to your Todo List.");
                }
                else
                {
                    json.First(x => x.Id == context.User.Id).ListItems.Add(item);
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.TodoPath, _json);
                    await context.Channel.SendMessageAsync(":heavy_check_mark:  Added item to your Todo List.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task RemoveListItemAsync(CommandContext context, int index)
        {
            Config.TodoCheck();
            var todolists = File.ReadAllText(Strings.TodoPath);
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(todolists);
            try
            {
                index--;
                if (!json.Any(x => x.Id == context.User.Id))
                    await context.Channel.SendMessageAsync(":eight_pointed_black_star:  You do not have a todo list.");
                else
                {
                    if (index < json.First(x => x.Id == context.User.Id).ListItems.Count)
                    {
                        json.First(x => x.Id == context.User.Id).ListItems.RemoveAt(index);
                        var _json = JsonConvert.SerializeObject(json);
                        File.WriteAllText(Strings.TodoPath, _json);
                        await context.Channel.SendMessageAsync(":heavy_check_mark:  Removed item from your Todo List.");
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(":heavy_multiplication_x:  There is no list item for that index.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task ClearListAsync(CommandContext context)
        {
            Config.TodoCheck();
            var todolists = File.ReadAllText(Strings.TodoPath);
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(todolists);
            try
            {
                if (!json.Any(x => x.Id == context.User.Id))
                    await context.Channel.SendMessageAsync(":eight_pointed_black_star:  You do not have a todo list...");
                else
                {
                    var list = json.First(x => x.Id == context.User.Id);
                    if (!list.ListItems.Any()) { await context.Channel.SendMessageAsync(":heavy_multiplication_x:  Your todo list is already empty."); return; } 
                    list.ListItems.Clear();
                    var _json = JsonConvert.SerializeObject(json);
                    File.WriteAllText(Strings.TodoPath, _json);
                    await context.Channel.SendMessageAsync(":heavy_check_mark:  Cleared your todo list!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
