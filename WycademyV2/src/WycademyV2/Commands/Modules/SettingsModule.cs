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
    [Group("sys")]
    [Summary("Settings Commands")]
    [RequireOwner]
    public class SettingsModule : ModuleBase
    {
        private LockerService _locker;
        private CommandCacheService _cache;

        public SettingsModule(LockerService locker, CommandCacheService ccs)
        {
            _locker = locker;
            _cache = ccs;
        }

        [Command("lock")]
        [Summary("Locks the bot, preventing it from responding to commands. If the bot is already locked, unlocks it.")]
        public Task SetLocked()
        {
            if (_locker.IsLocked)
            {
                _locker.Unlock();
            }
            else
            {
                _locker.Lock();
            }

            return Task.CompletedTask;
        }

        [Command("shutdown")]
        [Summary("Disconnects and closes the bot.")]
        public async Task Shutdown()
        {
            if (!_locker.IsLocked)
            {
                await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, text: "Commands must be locked in order to shut down.", prependZWSP: true);
            }
            else
            {
                await ReplyAsync("Shutting down...");
                await Task.Delay(1000);

                var client = Context.Client as DiscordSocketClient;

                await client.DisconnectAsync();
                await client.LogoutAsync();
                client.Dispose();

                Environment.Exit(0);
            }
        }

        [Command("setnick")]
        [Summary("Set the nickname of the bot for the current server.")]
        [RequireUnlocked]
        [RequireContext(ContextType.Guild)]
        public async Task SetNickname([Remainder] string name)
        {
            var botUser = await Context.Guild.GetCurrentUserAsync();

            await botUser.ModifyAsync(x => x.Nickname = name == "DEFAULT" ? null : name);
        }

        [Command("setgame")]
        [Summary("Sets the current game to show the bot as playing.")]
        [RequireUnlocked]
        public async Task SetGame([Remainder] string game)
        {
            await (Context.Client as DiscordSocketClient).SetGame(game);
        }
    }
}
