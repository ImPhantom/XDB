using Discord.Commands;
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
        {
            var amt = MathUtil.FToCelsius(x);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + "°F = " + amt.ToString() + "°C");
        }

        [Command("ctf"), Summary("Converts celsius to fahrenheit.")]
        [Name("ctf `<num>`")]
        public async Task CTF(double x)
        {
            var amt = MathUtil.CelsiusToF(x);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + "°C = " + amt.ToString() + "°F");
        }

        [Command("add"), Summary("Adds two doubles together.")]
        [Name("add `<num>` `<num>`")]
        public async Task Add(double x, double y)
        {
            var sum = MathUtil.Add(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " + " + y.ToString() + " = " + sum.ToString());
        }

        [Command("sub"), Summary("Subtracts a double from a double")]
        [Name("sub `<num>` `<num>`")]
        public async Task Sub(double x, double y)
        {
            var sum = MathUtil.Sub(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " - " + y.ToString() + " = " + sum.ToString());
        }

        [Command("mul"), Summary("Multiplys a double by a double.")]
        [Name("mul `<num>` `<num>`")]
        public async Task Mul(double x, double y)
        {
            var sum = MathUtil.Multiply(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " * " + y.ToString() + " = " + sum.ToString());
        }

        [Command("div"), Summary("Divides a double by a double.")]
        [Name("div `<num>` `<num>`")]
        public async Task Div(double x, double y)
        {
            var sum = MathUtil.Divide(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " / " + y.ToString() + " = " + sum.ToString());
        }
    }
}
