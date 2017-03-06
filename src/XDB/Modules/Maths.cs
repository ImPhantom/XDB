using Discord.Commands;
using System.Threading.Tasks;
using XDB.Utilities;

namespace XDB.Modules
{
    [Summary("Maths")]
    public class Maths : ModuleBase
    {
        [Command("ftc")]
        [Name("ftc `<num>`")]
        [Remarks("Converts fahrenheit to celsius.")]
        public async Task FTC(double x)
        {
            var amt = MathUtil.FToCelsius(x);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + "°F = " + amt.ToString() + "°C");
        }

        [Command("ctf")]
        [Name("ctf `<num>`")]
        [Remarks("Converts fahrenheit to celsius.")]
        public async Task CTF(double x)
        {
            var amt = MathUtil.CelsiusToF(x);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + "°C = " + amt.ToString() + "°F");
        }

        [Command("add")]
        [Name("add `<num>` `<num>`")]
        [Remarks("Adds two doubles together.")]
        public async Task Add(double x, double y)
        {
            var sum = MathUtil.Add(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " + " + y.ToString() + " = " + sum.ToString());
        }

        [Command("sub")]
        [Name("sub `<num>` `<num>`")]
        [Remarks("Subtracts a double from a double.")]
        public async Task Sub(double x, double y)
        {
            var sum = MathUtil.Sub(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " - " + y.ToString() + " = " + sum.ToString());
        }

        [Command("mul")]
        [Name("mul `<num>` `<num>`")]
        [Remarks("Multiplys a double by a double.")]
        public async Task Mul(double x, double y)
        {
            var sum = MathUtil.Multiply(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " * " + y.ToString() + " = " + sum.ToString());
        }

        [Command("div")]
        [Name("div `<num>` `<num>`")]
        [Remarks("Divides a double by a double.")]
        public async Task Div(double x, double y)
        {
            var sum = MathUtil.Divide(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " / " + y.ToString() + " = " + sum.ToString());
        }
    }
}
