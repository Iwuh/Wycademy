using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{

    public class SettingsModule
    {
        [Group("sys")]
        public class SystemGroup : ModuleBase
        {
            private LockerService _locker;

            public SystemGroup(LockerService locker)
            {
                _locker = locker;
            }

            [Command("lock")]
            [Summary("Locks the bot, preventing it from responding to commands. If the bot is already locked, unlocks it.")]
            public async Task SetLocked()
            {
                if (_locker.IsLocked)
                {
                    await _locker.Unlock();
                }
                else
                {
                    await _locker.Lock();
                }
            }
        }
    }
}
