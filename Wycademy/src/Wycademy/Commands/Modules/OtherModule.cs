using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wycademy.Commands.Preconditions;
using Wycademy.Commands.Services;
using Wycademy.Commands.Utilities;

namespace Wycademy.Commands.Modules
{
    [Name("other")]
    [Summary("Misc. Commands")]
    public class OtherModule : ModuleBase<SocketCommandContext>
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

            await pingMessage.ModifyAsync(x => x.Content = $"Gateway Latency: {Context.Client.Latency}ms.\nREST Latency: {timeDifference.TotalMilliseconds}ms.");
        }

        [Command("announceupdate")]
        [RequireOwner]
        [Summary("Announces a large update by DMing the owners of all the guilds the bot is connected to.")]
        public async Task AnnounceUpdate([Remainder, Summary("The message to send.")] string message)
        {
            var owners = Context.Client.Guilds
                // Exclude Discord Bots because I don't want to get b&.
                .Where(g => g.Id != 110373943822540800)
                .Select(g => g.Owner)
                .Distinct(new UserEqualityComparer());
            
            foreach (var user in owners)
            {
                await user.SendMessageAsync(message);
            }
        }

        [Command("invite")]
        [RequireUnlocked]
        [Summary("Provides a link to invite Wycademy to your Discord server.")]
        public async Task Invite()
        {
            var sb = new StringBuilder()
                .AppendLine($"Want to add Wycademy to your server? Just click here: <https://discordapp.com/oauth2/authorize?client_id=207172340809859072&scope=bot&permissions=27712>")
                .AppendLine($"For support, feature requests, and feedback, join the development server: https://discord.gg/R8g3BCS");

            await Context.User.SendMessageAsync(sb.ToString());
        }
    }
}
