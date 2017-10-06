using Discord.Commands;
using System.Threading.Tasks;
using XDB.Services;

namespace XDB.Modules
{
    [Summary("Todo")]
    [Group("todo")]
    public class TodoList : ModuleBase<SocketCommandContext>
    {
        private ListService _lists;

        [Command, Summary("Displays your todo list.")]
        public async Task ViewTodo()
        {
            var chList = _lists.FetchListContent(Context.User.Id, Context.Channel.Id);
            if (!chList.StartsWith(":"))
                await ReplyAsync("", embed: Xeno.ErrorEmbed(chList));
            else
                await ReplyAsync(chList);
        }
            
        [Command("add"), Summary("Adds item to you todo list.")]
        public async Task AddTodo([Remainder] string listitem)
        {
            var result = await _lists.AddListItemAsync(Context.User.Id, Context.Guild.Id, Context.Channel.Id, listitem);
            if (!result.StartsWith(":"))
                await ReplyAsync("", embed: Xeno.ErrorEmbed(result));
            else
                await ReplyAsync(result);
        }

        [Command("del"), Alias("rem","remove","delete"), Summary("Removes an item from your todo list by index.")]
        public async Task DelTodo(int index)
        {
            var result = await _lists.RemoveListItemAsync(Context.User.Id, Context.Guild.Id, Context.Channel.Id, index);
            if (!result.StartsWith(":"))
                await ReplyAsync("", embed: Xeno.ErrorEmbed(result));
            else
                await ReplyAsync(result);
        }

        [Command("clear"), Summary("Clears your todo list.")]
        public async Task ClearTodo()
        {
            var result = await _lists.ClearListAsync(Context.User.Id, Context.Channel.Id);
            if (!result.StartsWith(":"))
                await ReplyAsync("", embed: Xeno.ErrorEmbed(result));
            else
                await ReplyAsync(result);
        }

        [Command("edit"), Summary("Edits an item on your todo list.")]
        public async Task EditTodo(int index, [Remainder] string edit)
        {
            var result = await _lists.EditListItemAsync(Context.User.Id, Context.Guild.Id, Context.Channel.Id, index, edit);
            if (!result.StartsWith(":"))
                await ReplyAsync("", embed: Xeno.ErrorEmbed(result));
            else
                await ReplyAsync(result);
        }

        public TodoList(ListService lists)
        {
            _lists = lists;
        }
    }

    public class DevChannels : ModuleBase
    {
        private ListService _lists;

        [Command("dev"), Summary("Marks a text channel as a development channel")]
        [RequireOwner]
        public async Task MarkDev()
        {
            var result = await _lists.CreateChannelListAsync(Context.Guild.Id, Context.Channel.Id);
            if (!result.StartsWith(":"))
                await ReplyAsync("", embed: Xeno.ErrorEmbed(result));
            else
                await ReplyAsync(result);
        }

        public DevChannels(ListService lists)
        {
            _lists = lists;
        }
    }
}
