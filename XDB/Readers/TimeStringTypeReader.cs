﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XDB.Readers
{
    public class TimeStringTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var times = new Dictionary<string, int>();

            var regex = new Regex(@"(\d+)\s{0,1}([a-zA-Z]*)");
            var matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                int value;
                if (!int.TryParse(match.Groups[1].Value, out value))
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Invalid timestring specified."));

                string range = match.Groups[2].Value;
                if (string.IsNullOrWhiteSpace(range))
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Invalid timestring specified."));

                times.Add(range.Trim(), value);
            }

            var finalTime = new TimeSpan();
            foreach (var range in times)
            {
                switch (range.Key)
                {
                    case "y":
                        finalTime = finalTime.Add(new TimeSpan(range.Value * 365, 0, 0, 0));
                        break;
                    case "w":
                        finalTime = finalTime.Add(new TimeSpan(range.Value*7, 0, 0, 0));
                        break;
                    case "d":
                        finalTime = finalTime.Add(new TimeSpan(range.Value, 0, 0, 0));
                        break;
                    case "h":
                        finalTime = finalTime.Add(new TimeSpan(range.Value, 0, 0));
                        break;
                    case "m":
                        finalTime = finalTime.Add(new TimeSpan(0, range.Value, 0));
                        break;
                    case "s":
                        finalTime = finalTime.Add(new TimeSpan(0, 0, range.Value));
                        break;
                    
                    default:
                        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"Unknown timestring! {range.Key}"));
                }
            }

            return Task.FromResult(TypeReaderResult.FromSuccess(finalTime));
        }
    }
}
