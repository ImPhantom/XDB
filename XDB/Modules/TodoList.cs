using Discord.Commands;
using System.Threading.Tasks;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Todo")]
    [Group("todo")]
    public class TodoList : ModuleBase<SocketCommandContext>
    {
        [Command, Summary("Displays your todo list.")]
        public async Task ViewTodo()
            => await Todo.CheckTodoListAsync(Context);

        [Command("add"), Summary("Adds item to you todo list.")]
        public async Task AddTodo([Remainder] string listitem)
            => await Todo.AddListItemAsync(Context, listitem);

        [Command("del"), Summary("Removes an item from your todo list by index.")]
        public async Task DelTodo(int index)
            => await Todo.RemoveListItemAsync(Context, index);

        [Command("clear"), Summary("Clears your todo list.")]
        public async Task ClearTodo()
            => await Todo.ClearListAsync(Context);
    }
}
