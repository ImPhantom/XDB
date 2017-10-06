using Discord.Commands;
using System;
using System.Threading.Tasks;
using XDB.Common.Models;

namespace XDB.Readers
{
    public class MutingTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            //Do default both type
            if (input.ToLower().Contains("voice") || input.ToLower().Contains("v"))
                return Task.FromResult(TypeReaderResult.FromSuccess(MuteType.Voice));
            else if (input.ToLower().Contains("text") || input.ToLower().Contains("t"))
                return Task.FromResult(TypeReaderResult.FromSuccess(MuteType.Text));
            else if (input.ToLower().Contains("both") || input.ToLower().Contains("b"))
                return Task.FromResult(TypeReaderResult.FromSuccess(MuteType.Both));
            else
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Invaild mute type"));
               
        }
    }
}
