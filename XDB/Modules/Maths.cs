using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XDB.Modules
{
    [Summary("Maths")]
    public class Maths : ModuleBase<SocketCommandContext>
    {
        [Command("ftc"), Summary("Converts fahrenheit to celsius.")]
        [Name("ftc `<num>`")]
        public async Task FTC(double x)
            => await ReplyAsync(":thermometer: " + x.ToString() + "°F = " + FToCelsius(x).ToString() + "°C");

        [Command("ctf"), Summary("Converts celsius to fahrenheit.")]
        [Name("ctf `<num>`")]
        public async Task CTF(double x)
            => await ReplyAsync(":thermometer: " + x.ToString() + "°C = " + CelsiusToF(x).ToString() + "°F");

        [Command("add"), Summary("Adds two doubles together.")]
        [Name("add `<num>` `<num>`")]
        public async Task Add(params double[] nums)
            => await ReplyAsync($":heavy_plus_sign:  Result: `{nums.Sum().ToString()}`");

        [Command("subtract"), Alias("sub"), Summary("Subtracts a double from a double")]
        [Name("subtract `<num>` `<num>`")]
        public async Task Subtract(params double[] nums)
            => await ReplyAsync($":heavy_minus_sign:  Result: `{nums.Aggregate((a, x) => a - x).ToString()}`");

        [Command("multiply"), Alias("mul"), Summary("Multiplys a double by a double.")]
        [Name("multiply `<num>` `<num>`")]
        public async Task Multiply(params double[] nums)
            => await ReplyAsync($":heavy_multiplication_x:  Result: `{nums.Aggregate((a, x) => a * x).ToString()}`");

        [Command("divide"), Alias("div"), Summary("Divides a double by a double.")]
        [Name("divide `<num>` `<num>`")]
        public async Task Divide(params double[] nums)
            => await ReplyAsync($":heavy_division_sign:  Result: `{nums.Aggregate((a, x) => a / x).ToString()}`");

        public static double FToCelsius(double f)
        {
            return Math.Round(5.0 / 9.0 * (f - 32));
        }

        public static double CelsiusToF(double c)
        {
            return Math.Round(((9.0 / 5.0) * c) + 32);
        }
    }
}
