using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wycademy.Commands.Enums;

namespace Wycademy.Commands.TypeReaders
{
    public class BlacklistTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider provider)
        {
            switch (input.ToLower())
            {
                case "user":
                case "u":
                    return Task.FromResult(TypeReaderResult.FromSuccess(BlacklistType.User));
                case "guild":
                case "g":
                    return Task.FromResult(TypeReaderResult.FromSuccess(BlacklistType.Guild));
                case "guildowner":
                case "go":
                    return Task.FromResult(TypeReaderResult.FromSuccess(BlacklistType.GuildOwner));
                default:
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Could not parse input as a BlacklistType."));
            }
        }
    }
}
