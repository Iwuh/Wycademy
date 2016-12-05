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

    public class SettingsModule
    {
        [Group("sys")]
        public class SystemGroup : ModuleBase
        {
            private LockerService _locker;
            private CommandCacheService _cache;

            public SystemGroup(LockerService locker, CommandCacheService ccs)
            {
                _locker = locker;
                _cache = ccs;
            }

            [Command("lock")]
            [Summary("Locks the bot, preventing it from responding to commands. If the bot is already locked, unlocks it.")]
            [RequireOwner]
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
            [RequireOwner]
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
        }
    }
}
