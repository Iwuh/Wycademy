using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
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
                    .UseGlobalBlacklist()
                    .Parameter("Game", ParameterType.Unparsed)
                    .Do(e =>
                    {
                        if (!Program.locked)
                        {
                            _client.SetGame(e.GetArg("Game"));
                        }
                    });
                    igb.CreateCommand("nickname")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Parameter("Nickname", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        if (!Program.locked)
                        {
                            await e.Server.CurrentUser.Edit(nickname: e.GetArg("Nickname"));
                        }
                    });
                });

                cgb.CreateCommand("stats")
                .MinPermissions((int)PermissionLevels.User)
                .UseGlobalBlacklist()
                .Description("Provides stats about the bot.")
                .Do(async e =>
                {
                    if (!Program.locked)
                    {
                        var timeDifference = DateTime.Now - Program.startTime;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Statistics about the Wycademy:");
                        sb.AppendLine($"Uptime: {timeDifference.Days} days, {timeDifference.Hours} hours, {timeDifference.Minutes} minutes and {timeDifference.Seconds} seconds.");
                        sb.AppendLine($"Queries: {MonsterInfoBuilder.Queries}");
                        sb.AppendLine($"Connected servers: {_client.Servers.Count()}");
                        sb.AppendLine($"Heap size: {(GC.GetTotalMemory(false) / 1024f) / 1024f} MB");

                        await e.Channel.SendMessage(sb.ToString());
                    }
                });
                cgb.CreateCommand("about")
                .MinPermissions((int)PermissionLevels.User)
                .UseGlobalBlacklist()
                .Description("Provides information about the bot.")
                .Do(async e =>
                {
                    if (!Program.locked)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Language: C# running on .NET Framework 4.6.1");
                        sb.AppendLine("Library: Discord.Net 0.9.4");
                        sb.AppendLine("Created by DerpDargon (https://github.com/Iwuh/Wycademy)");
                        sb.AppendLine("Special thanks to my brother for helping me gather data on 71 monsters.");
                        sb.AppendLine("Icon by @thechewer on Instagram.");
                        sb.AppendLine("Data taken from Kiranico.");
                        sb.AppendLine("Monster Hunter and the Wycademy are © CAPCOM.");

                        await e.Channel.SendMessage(sb.ToString());
                    }
                });

                cgb.CreateGroup("sys", igb =>
                {
                    igb.CreateCommand("lock")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Locks the bot, preventing it from responding to commands.")
                    .Do(async e =>
                    {
                        if (Program.locked)
                        {
                            Program.locked = false;
                            await e.Channel.SendMessage("Bot has been unlocked and will now respond to commands.");
                        }
                        else
                        {
                            Program.locked = true;
                            await e.Channel.SendMessage("Bot has been locked and will no longer respond to commands.");
                        }
                    });
                    igb.CreateCommand("shutdown")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Shuts down the bot by closing the application.")
                    .Do(e =>
                    {
                        if (Program.locked)
                        {
                            Environment.Exit(0);
                        }
                    });
                });

                cgb.CreateGroup("blacklist", igb =>
                {
                    igb.CreateCommand("add")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Adds a user to the blacklist.")
                    .Parameter("User", ParameterType.Required)
                    .Do(async e =>
                    {
                        _client.BlacklistUser(ulong.Parse(e.GetArg("User")));
                        //WycademySettings.Blacklist.Add(ulong.Parse(e.GetArg("User")));
                        await WycademySettings.UpdateBlacklist(_client);
                        await e.Channel.SendMessage($"ID {e.GetArg("User")} added to blacklist.");
                    });
                    igb.CreateCommand("remove")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Removes a user from the blacklist.")
                    .Parameter("User", ParameterType.Required)
                    .Do(async e =>
                    {
                        _client.UnBlacklistUser(ulong.Parse(e.GetArg("User")));
                        //WycademySettings.Blacklist.Remove(ulong.Parse(e.GetArg("User")));
                        await WycademySettings.UpdateBlacklist(_client);
                        await e.Channel.SendMessage($"ID {e.GetArg("User")} removed from blacklist.");
                    });
                    igb.CreateCommand("list")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Lists all users on the blacklist.")
                    .Do(async e =>
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"There are {_client.GetBlacklistedUserIds().Count()} users on the blacklist:");
                        foreach (var id in _client.GetBlacklistedUserIds())
                        {
                            sb.AppendLine(id.ToString());
                        }
                        await e.Channel.SendMessage(sb.ToString());
                    });
                });
            });
        }
    }
}
