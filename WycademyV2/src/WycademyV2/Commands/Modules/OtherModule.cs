using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    [Summary("Misc. Commands")]
    public class OtherModule : ModuleBase
    {
        private RandomNumberService _random;
        private CommandCacheService _cache;

        public OtherModule(RandomNumberService rand, CommandCacheService cache)
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
    }
}
