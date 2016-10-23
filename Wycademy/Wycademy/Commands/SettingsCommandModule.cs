using Discord;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;
using Discord.Commands.Permissions.Userlist;
using Discord.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                            Message m = await e.Channel.SendMessageZWSP("Bot has been unlocked and will now respond to commands.");
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        }
                        else
                        {
                            Program.locked = true;
                            Message m = await e.Channel.SendMessageZWSP("Bot has been locked and will no longer respond to commands.");
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        }
                    });
                    igb.CreateCommand("shutdown")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Shuts down the bot by closing the application.")
                    .Do(async e =>
                    {
                        if (Program.locked)
                        {
                            await e.Channel.SendMessageZWSP("Shutting down...");
                            await _client.Disconnect();
                            Program.MessageCache.DisposeTimer();
                            await Task.Delay(5000);
                            Environment.Exit(0);
                        }
                    });
                });

                cgb.CreateGroup("blacklist", igb =>
                {
                    igb.CreateGroup("add", ab =>
                    {
                        ab.CreateCommand("user")
                        .MinPermissions((int)PermissionLevels.BotOwner)
                        .UseGlobalBlacklist()
                        .Description("Adds a user to the blacklist.")
                        .Parameter("User", ParameterType.Required)
                        .Do(async e =>
                        {
                            _client.BlacklistUser(ulong.Parse(e.GetArg("User")));
                            await WycademyBlacklist.UpdateUserBlacklist(_client);
                            Message m = await e.Channel.SendMessageZWSP($"User with ID {e.GetArg("User")} added to blacklist.");
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        });
                        ab.CreateCommand("server")
                        .MinPermissions((int)PermissionLevels.BotOwner)
                        .UseGlobalBlacklist()
                        .Description("Adds a server to the blacklist.")
                        .Parameter("Server", ParameterType.Required)
                        .Do(async e =>
                        {
                            WycademyBlacklist.ServerBlacklist.Add(ulong.Parse(e.GetArg("Server")));
                            await WycademyBlacklist.UpdateServerBlacklist();
                            Message m = await e.Channel.SendMessageZWSP($"Server with ID {e.GetArg("Server")} added to blacklist.");
                            Server s = _client.GetServer(ulong.Parse(e.GetArg("Server")));
                            await s.DefaultChannel.SendMessageZWSP("This server has been blacklisted. Wycademy will now leave.");
                            await s.Leave();
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        });
                        ab.CreateCommand("serverowner")
                        .MinPermissions((int)PermissionLevels.BotOwner)
                        .UseGlobalBlacklist()
                        .Description("Adds a server owner to the blacklist.")
                        .Parameter("ServerOwner", ParameterType.Required)
                        .Do(async e =>
                        {
                            WycademyBlacklist.ServerOwnerBlacklist.Add(ulong.Parse(e.GetArg("ServerOwner")));
                            await WycademyBlacklist.UpdateServerOwnerBlacklist();
                            Message m = await e.Channel.SendMessageZWSP($"Server owner with ID {e.GetArg("ServerOwner")} added to blacklist.");
                            IEnumerable<Server> serversToLeave = _client.Servers.Where(x => x.Owner.Id == ulong.Parse(e.GetArg("ServerOwner")));
                            foreach (var s in serversToLeave)
                            {
                                await s.DefaultChannel.SendMessageZWSP("The server owner has been blacklisted. Wycademy will now leave.");
                                await s.Leave();
                            }
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        });
                    });
                    igb.CreateGroup("remove", rb =>
                    {
                        rb.CreateCommand("user")
                        .MinPermissions((int)PermissionLevels.BotOwner)
                        .UseGlobalBlacklist()
                        .Description("Removes a user from the blacklist.")
                        .Parameter("User", ParameterType.Required)
                        .Do(async e =>
                        {
                            _client.UnBlacklistUser(ulong.Parse(e.GetArg("User")));
                            await WycademyBlacklist.UpdateUserBlacklist(_client);
                            Message m = await e.Channel.SendMessageZWSP($"User with ID {e.GetArg("User")} removed from blacklist.");
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        });
                        rb.CreateCommand("server")
                        .MinPermissions((int)PermissionLevels.BotOwner)
                        .UseGlobalBlacklist()
                        .Description("Removes a server from the blacklist.")
                        .Parameter("Server", ParameterType.Required)
                        .Do(async e =>
                        {
                            WycademyBlacklist.ServerBlacklist.Remove(ulong.Parse(e.GetArg("Server")));
                            await WycademyBlacklist.UpdateServerBlacklist();
                            Message m = await e.Channel.SendMessageZWSP($"Server with ID {e.GetArg("Server")} removed from blacklist.");
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        });
                        rb.CreateCommand("serverowner")
                        .MinPermissions((int)PermissionLevels.BotOwner)
                        .UseGlobalBlacklist()
                        .Description("Removes a server owner from the blacklist.")
                        .Parameter("ServerOwner", ParameterType.Required)
                        .Do(async e =>
                        {
                            WycademyBlacklist.ServerOwnerBlacklist.Remove(ulong.Parse(e.GetArg("Server")));
                            await WycademyBlacklist.UpdateServerOwnerBlacklist();
                            Message m = await e.Channel.SendMessageZWSP($"Server owner with ID {e.GetArg("Server")} removed from blacklist.");
                            await Task.Delay(1000);
                            Program.MessageCache.Add(e.Message.Id, m.Id);
                        });
                    });
                    igb.CreateCommand("list")
                    .MinPermissions((int)PermissionLevels.BotOwner)
                    .UseGlobalBlacklist()
                    .Description("Lists all users on the blacklist.")
                    .Do(async e =>
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"There are {_client.GetBlacklistedUserIds().Count() + WycademyBlacklist.ServerBlacklist.Count + WycademyBlacklist.ServerOwnerBlacklist.Count} IDs on the blacklist:");
                        foreach (var id in _client.GetBlacklistedUserIds())
                        {
                            sb.AppendLine(id.ToString() + "(User)");
                        }
                        foreach (var id in WycademyBlacklist.ServerBlacklist)
                        {
                            sb.AppendLine(id.ToString() + "(Server)");
                        }
                        foreach (var id in WycademyBlacklist.ServerOwnerBlacklist)
                        {
                            sb.AppendLine(id.ToString() + "(Server Owner)");
                        }
                        Message m = await e.Channel.SendMessageZWSP(sb.ToString());
                        await Task.Delay(1000);
                        Program.MessageCache.Add(e.Message.Id, m.Id);
                    });
                });

                cgb.CreateCommand("clean")
                .MinPermissions((int)PermissionLevels.ServerModerator)
                .UseGlobalBlacklist()
                .Description("Downloads the last 100 messages in the channel that this command was called in and deletes any by Wycademy.")
                .Do(async e =>
                {
                    // Download the last 100 messages from the channel
                    Message[] messages = await e.Channel.DownloadMessages();
                    // Get an IEnumerable containing the ID of every Message in messages written by Wycademy
                    var messagesToDelete = from Message m in messages
                                           where m.IsAuthor
                                           select m;
                    int messagesDeleted = messagesToDelete.Count();

                    // Bulk delete requires the Manage Messages permission so we need to delete them 1 at a time,
                    // keeping in mind the 5/1s ratelimit.
                    int requests = 0;
                    foreach (Message msg in messagesToDelete)
                    {
                        await msg.Delete();
                        requests++;

                        if (requests >= 4)
                        {
                            await Task.Delay(1000);
                            requests = 0;
                        }
                    }
                    // SendMessageZWSP returns a Task<Message> so we can save it to a variable and manipulate it later.
                    Message response = await e.Channel.SendMessageZWSP($"Successfully deleted {messagesDeleted} messages.");
                    // Wait 4 seconds and then delete the response.
                    await Task.Delay(4000);
                    await response.Delete();
                });
            });
        }
    }
}
