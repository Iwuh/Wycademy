using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    class SettingsCommandModule : IModule
    {
        private DiscordClient _client;
        private ModuleManager _manager;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _client = manager.Client;

            manager.CreateCommands("", cgb =>
            {
                cgb.CreateGroup("set", igb =>
                {
                    igb.CreateCommand("game")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .Parameter("Game", ParameterType.Unparsed)
                    .Do(e =>
                    {
                        _client.SetGame(e.GetArg("Game"));
                    });
                    igb.CreateCommand("nickname")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .Parameter("Nickname", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                       await e.Server.CurrentUser.Edit(nickname: e.GetArg("Nickname"));
                    });
                });

                cgb.CreateCommand("stats")
                .MinPermissions((int)PermissionLevels.User)
                .Description("Provides stats about the bot.")
                .Do(async e =>
                {
                    var timeDifference = DateTime.Now - Program.startTime;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Statistics about the Wycademy:");
                    sb.AppendLine($"Uptime: {timeDifference.Days} days, {timeDifference.Hours} hours, {timeDifference.Minutes} minutes and {timeDifference.Seconds} seconds.");
                    sb.AppendLine($"Queries: {MonsterInfoBuilder.Queries}");

                    await e.Channel.SendMessage(sb.ToString());
                });
                cgb.CreateCommand("about")
                .MinPermissions((int)PermissionLevels.User)
                .Description("Provides information about the bot.")
                .Do(async e =>
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Language: C# running on .NET Framework 4.6.1");
                    sb.AppendLine("Library: Discord.Net 0.9.4");
                    sb.AppendLine("Created by DerpDargon (https://github.com/Iwuh/Wycademy)");
                    sb.AppendLine("Icon by @thechewer on Instagram.");
                    sb.AppendLine("Data taken from Kiranico.");
                    sb.AppendLine("Special thanks to my brother for helping me gather data on 71 monsters.");

                    await e.Channel.SendMessage(sb.ToString());
                });
            });
        }
    }
}
