using Discord.Commands;
using System.Threading.Tasks;
using XDB.Utilities;

namespace XDB.Modules
{
    public class Maths : ModuleBase
    {
        [Command("ftc")]
        [Remarks("Converts fahrenheit to celsius.")]
        public async Task FTC(double x)
        {
            var amt = Mth.FToCelsius(x);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + "°F = " + amt.ToString() + "°C");
        }

        [Command("ctf")]
        [Remarks("Converts fahrenheit to celsius.")]
        public async Task CTF(double x)
        {
            var amt = Mth.CelsiusToF(x);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + "°C = " + amt.ToString() + "°F");
        }

        [Command("add")]
        [Remarks("Adds two doubles together.")]
        public async Task Add(double x, double y)
        {
            var sum = Mth.Add(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " + " + y.ToString() + " = " + sum.ToString());
        }

        [Command("sub")]
        [Remarks("Subtracts a double from a double.")]
        public async Task Sub(double x, double y)
        {
            var sum = Mth.Sub(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " - " + y.ToString() + " = " + sum.ToString());
        }

        [Command("mul")]
        [Remarks("Multiplys a double by a double.")]
        public async Task Mul(double x, double y)
        {
            var sum = Mth.Multiply(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " * " + y.ToString() + " = " + sum.ToString());
        }

        [Command("div")]
        [Remarks("Divides a double by a double.")]
        public async Task Div(double x, double y)
        {
            var sum = Mth.Divide(x, y);
            await ReplyAsync(":grey_exclamation:" + x.ToString() + " / " + y.ToString() + " = " + sum.ToString());
        }
    }
}
