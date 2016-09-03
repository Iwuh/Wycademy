using Discord;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
using Discord.Commands.Permissions.Visibility;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    Message m = await e.Channel.SendMessage(WycademySettings.EYE_EMOJIS[WycademySettings.RandomNumbers.Next(WycademySettings.EYE_EMOJIS.Length)]);
                    await Task.Delay(1000);
                    Program.MessageCache.Add(e.Message.Id, m.Id);
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
                        using (Process p = Process.GetCurrentProcess())
                        {
                            sb.AppendLine($"Memory used: {(p.PrivateMemorySize64 / 1024f / 1024f).ToString()} MB");
                        }

                        Message m = await e.Channel.SendMessage(sb.ToString());
                        await Task.Delay(1000);
                        Program.MessageCache.Add(e.Message.Id, m.Id);
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

                        Message m = await e.Channel.SendMessage(sb.ToString());
                        await Task.Delay(1000);
                        Program.MessageCache.Add(e.Message.Id, m.Id);
                    }
                });

                cgb.CreateCommand("invite")
                .MinPermissions((int)PermissionLevels.User)
                .UseGlobalBlacklist()
                .Description("Provides an invite link for Wycademy.")
                .Do(async e =>
                {
                    Message m = await e.Channel.SendMessage("So you want Wycademy on your server? Just click the link below to add it to a server you own.\n" +
                        "https://discordapp.com/oauth2/authorize?client_id=207172340809859072&scope=bot&permissions=3072");
                    await Task.Delay(1000);
                    Program.MessageCache.Add(e.Message.Id, m.Id);
                });
            });
        }
    }
}
