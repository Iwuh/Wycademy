using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;
using WycademyV2.Commands.Utilities;

namespace WycademyV2.Commands.Modules
{
    [Summary("Misc. Commands")]
    public class OtherModule : ModuleBase
    {
        private UtilityService _random;
        private CommandCacheService _cache;

        public OtherModule(UtilityService rand, CommandCacheService cache)
        {
            _random = rand;
            _cache = cache;
        }

        [Command("ping")]
        [Alias("pang", "peng", "pong", "pung", "pyng")]
        [RequireUnlocked]
        [Summary("Gets the websocket latency and REST latency for the bot.")]
        public async Task GetLatency()
        {
            var pingMessage = await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: "[...]", prependZWSP: true);
            TimeSpan timeDifference = pingMessage.Timestamp.ToUniversalTime() - Context.Message.Timestamp;

            await pingMessage.ModifyAsync(x => x.Content = $"Gateway Latency: {(Context.Client as DiscordSocketClient).Latency}ms.\nREST Latency: {timeDifference.TotalMilliseconds}ms.");
        }

        [Command("announceupdate")]
        [RequireOwner]
        [Summary("Announces a large update by DMing the owners of all the guilds the bot is connected to.")]
        public async Task AnnounceUpdate([Remainder, Summary("The message to send.")] string message)
        {
            var owners = (Context.Client as DiscordSocketClient).Guilds
                // Exclude Discord Bots because I don't want to get b&.
                .Where(g => g.Id != 110373943822540800)
                .Select(g => g.Owner)
                .Distinct(new UserEqualityComparer());
            
            foreach (var user in owners)
            {
                var dm = await user.CreateDMChannelAsync();
                await dm.SendMessageAsync(message);
            }
        }
    }
}
