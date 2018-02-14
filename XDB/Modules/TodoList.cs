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
            var result = _lists.FetchTodoList(Context.User.Id);
            await ReplyAsync(result);
        }
            
        [Command("add"), Summary("Adds item to you todo list.")]
        public async Task AddTodo([Remainder] string listitem)
        {
            var result = await _lists.TryAddListItemAsync(Context.User.Id, listitem);
            if (result)
                await ReplyAsync(_lists.FetchTodoList(Context.User.Id));
        }

        [Command("del"), Alias("rem","remove","delete"), Summary("Removes an item from your todo list by index.")]
        public async Task DelTodo(int index)
        {
            var result = await _lists.TryRemoveListItemAsync(Context.User.Id, index);
            if (result)
                await ReplyAsync(_lists.FetchTodoList(Context.User.Id));
            else
                await SendErrorEmbedAsync("You do not have a todo list.");
        }

        [Command("clear"), Summary("Clears your todo list.")]
        public async Task ClearTodo()
        {
            var result = await _lists.TryClearListAsync(Context.User.Id);
            if (result)
                await ReplyAsync(_lists.FetchTodoList(Context.User.Id));
            else
                await SendErrorEmbedAsync("You do not have a todo list.");
        }

        [Command("edit"), Summary("Edits an item on your todo list.")]
        public async Task EditTodo(int index, [Remainder] string edit)
        {
            var result = await _lists.TryEditListItemAsync(Context.User.Id, index, edit);
            if (result)
                await ReplyAsync(_lists.FetchTodoList(Context.User.Id));
            else
                await SendErrorEmbedAsync("You do not have a todo list.");
        }

        public TodoList(ListService lists)
        {
            _lists = lists;
        }
    }
}
