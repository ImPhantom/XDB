using Discord.WebSocket;
using System;

namespace XDB
{
    class BetterConsole
    {
        public static void AppendText(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = foreground.Value;
            Console.BackgroundColor = background.Value;
            Console.Write(text);
        }

        public static void AppendLine(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = foreground.Value;
            Console.BackgroundColor = background.Value;
            Console.Write(Environment.NewLine + text);
        }

        public static void Log(object severity, string src, string message)
        {
            BetterConsole.AppendLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            BetterConsole.AppendText($"[{severity}] ", ConsoleColor.DarkCyan);
            BetterConsole.AppendText($"{src}: ", ConsoleColor.Cyan);
            BetterConsole.AppendText(message, ConsoleColor.White);
        }

        public static void LogError(string source, string error)
        {
            BetterConsole.AppendLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            BetterConsole.AppendText($"[Error] ", ConsoleColor.Red);
            BetterConsole.AppendText($"[{source}]: ", ConsoleColor.Cyan);
            BetterConsole.AppendText(error, ConsoleColor.White);
        }

        public static void LogDM(SocketMessage message)
        {
            BetterConsole.AppendLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            BetterConsole.AppendText("[DM] ", ConsoleColor.Green);
            BetterConsole.AppendText($"{message.Author.Username}: ", ConsoleColor.Gray);
            BetterConsole.AppendText($"{message.Content}", ConsoleColor.White);
        }
    }
}
