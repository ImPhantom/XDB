using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Types;
using System.Linq;

namespace XDB.Modules
{
    public class Todo : ModuleBase
    {
        [Command("todo")]
        [Remarks("Views your todo list.")]
        [RequireContext(ContextType.Guild)]
        public async Task ViewTodo()
        {
            Config.TodoCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var filetext = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var json = JsonConvert.DeserializeObject<List<TodoList>>(filetext);
            try
            {
                var ret = json.Find(x => x.Id == Context.User.Id);
                await ReplyAsync("**Your Todo List**");
                foreach(var item in ret.ListItems)
                {
                    await ReplyAsync(item.ToString());
                }
            } catch
            {
                var newtodo = new TodoList()
                {
                    Id = Context.User.Id,
                    ListItems = new List<string> { "" }
                };
                json.Add(newtodo);
                var outjson = JsonConvert.SerializeObject(json);
                File.WriteAllText(path, outjson);
            }

        }

        [Command("addtodo")]
        [Name("addtodo `<todoitem>`")]
        [Remarks("Add to your todo list.")]
        [RequireContext(ContextType.Guild)]
        public async Task AddTodo([Remainder] string listitem)
        {
            Config.TodoCheck();
            var all = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var json = JsonConvert.DeserializeObject<List<TodoList>>(all);
            try
            {
                if(!json.Any(x => x.Id == Context.User.Id))
                {
                    var newtodo = new TodoList()
                    {
                        Id = Context.User.Id,
                        ListItems = new List<string> { listitem }
                    };
                    json.Add(newtodo);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync("No todo list found, Creating and adding list item...");
                } else
                {
                    json.First(x => x.Id == Context.User.Id).ListItems.Add(listitem);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync("Added item to your Todo List.");
                }
            } catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }

        [Command("deltodo")]
        [Name("deltodo `<todoitem>`")]
        [Remarks("Deletes an item from your todo list.")]
        [RequireContext(ContextType.Guild)]
        public async Task DelTodo([Remainder] string listitem)
        {
            Config.TodoCheck();
            var all = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var json = JsonConvert.DeserializeObject<List<TodoList>>(all);
            try
            {
                if (!json.Any(x => x.Id == Context.User.Id))
                {
                    await ReplyAsync("You dont have a todo list...");
                }
                else
                {
                    if (json.First(x => x.Id == Context.User.Id).ListItems.Contains(listitem))
                    {
                        //Check if list item just contains one word
                        var index = json.FindIndex(x => x.ListItems.Contains(listitem));
                        json.First(x => x.Id == Context.User.Id).ListItems.RemoveAt(index);
                        var outjson = JsonConvert.SerializeObject(json);
                        File.WriteAllText(path, outjson);
                        await ReplyAsync("Removed item from your Todo List.");
                    }
                    else
                    {
                        await ReplyAsync(":anger: No list item was found with that keyword.");
                    }
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }
    }
}
