using Discord.Commands;
using System.Threading.Tasks;
using XDB.Common;
using XDB.Services;

namespace XDB.Modules
{
    [Summary("Todo")]
    [Group("todo")]
    public class TodoList : XenoBase
    {
        private ListService _lists;

        [Command, Summary("Displays your todo list.")]
        public async Task ViewTodo()
        {
            var result = _lists.FetchListItems(Context.User.Id);
            if (!result.StartsWith(":"))
                await SendErrorEmbedAsync(result);
            else
                await ReplyAsync(result);
        }
            
        [Command("add"), Summary("Adds item to you todo list.")]
        public async Task AddTodo([Remainder] string listitem)
        {
            var result = await _lists.AddTodoListItemAsync(Context.User.Id, listitem);
            if (!result.StartsWith(":"))
                await SendErrorEmbedAsync(result);
            else
                await ReplyAsync(result);
        }

        [Command("del"), Alias("rem","remove","delete"), Summary("Removes an item from your todo list by index.")]
        public async Task DelTodo(int index)
        {
            var result = await _lists.RemoveTodoListItemAsync(Context.User.Id, index);
            if (!result.StartsWith(":"))
                await SendErrorEmbedAsync(result);
            else
                await ReplyAsync(result);
        }

        [Command("clear"), Summary("Clears your todo list.")]
        public async Task ClearTodo()
        {
            var result = await _lists.ClearTodoListAsync(Context.User.Id);
            if (!result.StartsWith(":"))
                await SendErrorEmbedAsync(result);
            else
                await ReplyAsync(result);
        }

        [Command("edit"), Summary("Edits an item on your todo list.")]
        public async Task EditTodo(int index, [Remainder] string edit)
        {
            var result = await _lists.EditTodoListItemAsync(Context.User.Id, index, edit);
            if (!result.StartsWith(":"))
                await SendErrorEmbedAsync(result);
            else
                await ReplyAsync(result);
        }

        public TodoList(ListService lists)
        {
            _lists = lists;
        }
    }
}
