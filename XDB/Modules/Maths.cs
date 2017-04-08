using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Maths")]
    public class Maths : ModuleBase
    {
        [Command("ftc"), Summary("Converts fahrenheit to celsius.")]
        [Name("ftc `<num>`")]
        public async Task FTC(double x)
            => await ReplyAsync(":grey_exclamation:" + x.ToString() + "°F = " + MathUtil.FToCelsius(x).ToString() + "°C");

        [Command("ctf"), Summary("Converts celsius to fahrenheit.")]
        [Name("ctf `<num>`")]
        public async Task CTF(double x)
            => await ReplyAsync(":grey_exclamation:" + x.ToString() + "°C = " + MathUtil.CelsiusToF(x).ToString() + "°F");

        [Command("add"), Summary("Adds two doubles together.")]
        [Name("add `<num>` `<num>`")]
        public async Task Add(params double[] nums)
            => await ReplyAsync($":grey_exclamation: Result: `{nums.Sum().ToString()}`");

        [Command("subtract"), Alias("sub"), Summary("Subtracts a double from a double")]
        [Name("subtract `<num>` `<num>`")]
        public async Task Subtract(params double[] nums)
            => await ReplyAsync($":grey_exclamation: Result: {nums.Aggregate((a, x) => a - x).ToString()}");

        [Command("multiply"), Alias("mul"), Summary("Multiplys a double by a double.")]
        [Name("multiply `<num>` `<num>`")]
        public async Task Multiply(params double[] nums)
            => await ReplyAsync($":grey_exclamation: Result: `{nums.Aggregate((a, x) => a * x).ToString()}`");

        [Command("divide"), Alias("div"), Summary("Divides a double by a double.")]
        [Name("divide `<num>` `<num>`")]
        public async Task Divide(params double[] nums)
            => await ReplyAsync($":grey_exclamation: Result: `{nums.Aggregate((a, x) => a / x).ToString()}`");
    }
}
