using Discord;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
using Discord.Commands.Permissions.Visibility;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    class OtherCommandModule : IModule
    {
        private DiscordClient _client;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateCommand("eyes")
                .Description(":eyes:")
                .MinPermissions((int)PermissionLevels.User)
                .UseGlobalBlacklist()
                .Hide()
                .Do(async e =>
                {
                    await e.Channel.SendMessage(":eyes:");
                });
            });
        }
    }
}
