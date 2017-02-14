using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XDB.Common.Attributes;
using XDB.Common.Types;
using System.Linq;
using System.Text;

namespace XDB.Modules
{
    public class Todo : ModuleBase
    {
        [Command("todo")]
        [Remarks("Views your todo list.")]
        public async Task ViewTodo()
        {
            Config.TodoCheck();
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var filetext = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(filetext);
            try
            {
                if (!json.Any(x => x.Id == Context.User.Id))
                    await ReplyAsync(":anger: Your todo list is empty.");
                else
                {
                    var ret = json.Find(x => x.Id == Context.User.Id);
                    if (!ret.ListItems.Any()) { await ReplyAsync(":anger: Your todo list is empty."); return; }
                    if (ret.ListItems.Any(x => string.IsNullOrEmpty(x))) { ret.ListItems.RemoveAll(str => string.IsNullOrEmpty(str)); var outjson = JsonConvert.SerializeObject(json); File.WriteAllText(path, outjson); }
                    var list = new StringBuilder();
                    foreach (var item in ret.ListItems)
                    {
                        list.AppendLine($"~ {item}");
                    }
                    await ReplyAsync($@"**>>>>  Your Todo List  <<<<**```{list.ToString()}```");
                    
                }
                    
            } catch(Exception e)
            {
                await ReplyAsync(e.Message);
            }

        }

        [Command("addtodo")]
        [Name("addtodo `<todoitem>`")]
        [Remarks("Add to your todo list.")]
        public async Task AddTodo([Remainder] string listitem)
        {
            Config.TodoCheck();
            var all = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(all);
            try
            {
                if(!json.Any(x => x.Id == Context.User.Id))
                {
                    var newtodo = new UserTodo()
                    {
                        Id = Context.User.Id,
                        ListItems = new List<string> { listitem }
                    };
                    json.Add(newtodo);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":white_check_mark: Added item to your Todo List.");
                } else
                {
                    json.First(x => x.Id == Context.User.Id).ListItems.Add(listitem);
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":white_check_mark: Added item to your Todo List.");
                }
            } catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }

        [Command("deltodo")]
        [Name("deltodo `<todoitem>`")]
        [Remarks("Deletes an item from your todo list by string.")]
        public async Task DelTodo([Remainder] string listitem)
        {
            Config.TodoCheck();
            var all = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(all);
            try
            {
                if (!json.Any(x => x.Id == Context.User.Id))
                {
                    await ReplyAsync(":anger: You dont have a todo list...");
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
                        await ReplyAsync(":white_check_mark: Removed item from your Todo List.");
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

        [Command("deltodo")]
        [Name("deltodo `<index>`")]
        [Remarks("Deletes an item from your todo list by index.")]
        public async Task DelTodo(int index)
        {
            Config.TodoCheck();
            var all = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(all);
            try
            {
                index--;
                if (!json.Any(x => x.Id == Context.User.Id))
                {
                    await ReplyAsync(":anger: You dont have a todo list...");
                }
                else
                {
                    if (index < json.First(x => x.Id == Context.User.Id).ListItems.Count)
                    {
                        json.First(x => x.Id == Context.User.Id).ListItems.RemoveAt(index);
                        var outjson = JsonConvert.SerializeObject(json);
                        File.WriteAllText(path, outjson);
                        await ReplyAsync(":white_check_mark: Removed item from your Todo List.");
                    }
                    else
                    {
                        await ReplyAsync(":anger: There is no list item for that index.");
                    }
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }

        [Command("cleartodo")]
        [Name("cleartodo")]
        [Remarks("Clears your todo list.")]
        public async Task ClearTodo()
        {
            Config.TodoCheck();
            var all = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json"));
            var path = Path.Combine(AppContext.BaseDirectory, $"todo/todolists.json");
            var json = JsonConvert.DeserializeObject<List<UserTodo>>(all);
            try
            {
                if (!json.Any(x => x.Id == Context.User.Id))
                {
                    await ReplyAsync(":anger: You dont have a todo list...");
                }
                else
                {
                    json.First(x => x.Id == Context.User.Id).ListItems.Clear();
                    var outjson = JsonConvert.SerializeObject(json);
                    File.WriteAllText(path, outjson);
                    await ReplyAsync(":white_check_mark: Cleared your todo list!");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Exception: {e.Message}");
            }
        }
    }
}
