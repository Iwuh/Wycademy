using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wycademy.Commands.Enums;
using Wycademy.Commands.Services;

namespace Wycademy.Commands.Modules
{
    [Group("blacklist")]
    [Summary("Blacklist Commmands")]
    [Remarks("group")]
    [RequireOwner]
    public class BlacklistModule : ModuleBase<SocketCommandContext>
    {
        private BlacklistService _blacklist;
        private CommandCacheService _cache;

        public BlacklistModule(BlacklistService bs, CommandCacheService ccs)
        {
            _blacklist = bs;
            _cache = ccs;
        }

        [Command("add")]
        [Summary("Adds an ID to a blacklist.")]
        public async Task AddToBlacklist([Summary("The ID to add.")] ulong id, [Summary("The blacklist to add to (parsed into a BlacklistType).")] BlacklistType category)
        {
            await _blacklist.AddToBlacklist(id, category);
            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: $"ID {id} successfully added to blacklist.", prependZWSP: true);
        }

        [Command("remove")]
        [Summary("Removes an ID from a blacklist.")]
        public async Task RemoveFromBlacklist([Summary("The ID to remove.")] ulong id, [Summary("The blacklist to remove from (parsed into a BlacklistType).")] BlacklistType category)
        {
            bool result = await _blacklist.RemoveFromBlacklist(id, category);
            string message = result ? $"ID {id} successfully removed from blacklist." : "The ID could not be removed.";
            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: message, prependZWSP: true);
        }

        [Command("list")]
        [Summary("Lists all IDs on the blacklist.")]
        public async Task ListBlacklist()
        {
            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: _blacklist.GetBlacklist(), prependZWSP: true);
        }
    }
}
