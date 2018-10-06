using Discord.Commands;
using Discord.WebSocket;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wycademy.Commands.Preconditions;
using Wycademy.Commands.Services;

namespace Wycademy.Commands.Modules
{
    [Group("sys")]
    [Summary("Settings Commands")]
    [RequireOwner]
    public class SettingsModule : ModuleBase<SocketCommandContext>
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

        [Command("shutdown", RunMode = RunMode.Async)]
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

                // Disconnect and log out.
                await Context.Client.StopAsync();
                await Context.Client.LogoutAsync();

                // Shut down logging.
                LogManager.Shutdown();

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
            var botUser = Context.Guild.CurrentUser;

            await botUser.ModifyAsync(x => x.Nickname = name == "DEFAULT" ? null : name);
        }

        [Command("setgame")]
        [Summary("Sets the current game to show the bot as playing.")]
        [RequireUnlocked]
        public async Task SetGame([Remainder] string game)
        {
            await Context.Client.SetGameAsync(game);
        }
    }
}
