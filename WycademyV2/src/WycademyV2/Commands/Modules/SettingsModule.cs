using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    [Group("sys")]
    [Summary("Settings Commands")]
    [Remarks("group")]
    [RequireOwner]
    public class SettingsModule : ModuleBase
    {
        private LockerService _locker;
        private CommandCacheService _cache;
        private UtilityService _utility;

        public SettingsModule(LockerService locker, CommandCacheService ccs, UtilityService us)
        {
            _locker = locker;
            _cache = ccs;
            _utility = us;
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

                // Dispose of the command cache.
                _cache.Dispose();

                // Cancel the infinite delay then dispose the token source.
                _utility.Shutdown.Cancel();
                _utility.Shutdown.Dispose();

                // Disconnect, log out, and dispose.
                var client = Context.Client as DiscordSocketClient;
                await client.StopAsync();
                await Task.Delay(1000);
                await client.LogoutAsync();
                await Task.Delay(1000);
                client.Dispose();

                // Finally, exit the console application.
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
            await (Context.Client as DiscordSocketClient).SetGameAsync(game);
        }
    }
}
