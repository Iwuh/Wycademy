﻿using Discord;
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
    class Program
    {
        //Call Start() so that the bot runs in a non static method
        static void Main(string[] args) => new Program().Start();

        private DiscordClient _client;
        public static DateTime startTime = DateTime.Now;
        public static bool locked = false;
        public static QueueDictionary<Message, Message> MessageCache = new QueueDictionary<Message, Message>(500);

        public void Start()
        {
            //Set basic settings
            _client = new DiscordClient(x =>
            {
                x.AppName = "Wycademy";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            })
            .UsingCommands(x =>
            {
                x.PrefixChar = WycademySettings.WYCADEMY_PREFIX;
                x.HelpMode = HelpMode.Private;
                x.ErrorHandler = CommandError;
            })
            .UsingPermissionLevels((u, c) => (int)GetPermissions(u, c)) // Pass User, Channel to GetPermissions and then cast the returned PermissionLevels to int.
            .UsingModules()
            .UsingGlobalBlacklist(WycademySettings.InitializeBlacklist().ToArray());

            //Set up message logging
            _client.MessageReceived += (s, e) =>
            {
                if (e.Message.IsAuthor)
                {
                    _client.Log.Info(">>Message", $"{((e.Server != null) ? e.Server.Name : "Private")}/#{((!e.Channel.IsPrivate) ? e.Channel.Name : "")}: {e.Message}");
                }
                else if (!e.Message.IsAuthor)
                {
                    _client.Log.Info("<<Message", $"{((e.Server != null) ? e.Server.Name : "Private")}/#{((!e.Channel.IsPrivate) ? e.Channel.Name : "")}: {e.Message}");
                }
            };
            _client.MessageDeleted += async (s, e) =>
            {
                if (MessageCache.ContainsKey(e.Message))
                {
                    await MessageCache[e.Message].Delete();
                    MessageCache.RemoveByKey(e.Message);
                }
            };

            // Add Modules to bot
            _client.AddModule<InfoCommandModule>("Info Commands", ModuleFilter.None);
            _client.AddModule<SettingsCommandModule>("Settings Commands", ModuleFilter.None);
            _client.AddModule<EvalCommandModule>("Eval Commands", ModuleFilter.None);
            _client.AddModule<OtherCommandModule>("Misc. Commands", ModuleFilter.None);

            //Bot token is stored in an environment variable so that nobody sees it when I push to GitHub :D
            string token = Environment.GetEnvironmentVariable("WYCADEMY_TOKEN", EnvironmentVariableTarget.User);

            //Connect to all guilds that the bot is part of and then block until the bot is manually exited.
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token);
            });
        }

        // Handle command errors
        public async void CommandError(object sender, CommandErrorEventArgs e)
        {
            switch (e.ErrorType)
            {
                case CommandErrorType.Exception:
                    await e.Channel.SendMessage($":interrobang: Error: An exception occurred and has been logged to the console. If this error happens again, contact DerpDargon.");
                    _client.Log.Error("Command Error", e.Exception);
                    break;
                case CommandErrorType.BadPermissions:
                    await e.Channel.SendMessage(":no_entry: You don't have the required permissions to use this command!");
                    break;
                case CommandErrorType.InvalidInput:
                    await e.Channel.SendMessage("Error: Invalid input.");
                    break;
                case CommandErrorType.BadArgCount:
                    await e.Channel.SendMessage("Error: Invalid argument count. Try <help [command].");
                    break;
            }
        }

        //Whenever we log data, this method is called.
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}] {e.Message}");
        }

        // Used for minimum permissions on commands.
        public PermissionLevels GetPermissions(User u, Channel c)
        {
            if (u.Id == WycademySettings.OWNER_ID)
            {
                // Only allows access to BotOwner level commands if I call them
                return PermissionLevels.BotOwner;
            }
            else if (u.IsBot)
            {
                // Ignores all bots and people on the blacklist
                return PermissionLevels.Ignored;
            }
            else if (c.Server != null)
            {
                // Prevents a NullReferenceException if the command was called from a direct message.
                if (u == c.Server.Owner || u.Roles.Select(x => x.Name.ToLower()).Contains("moderator"))
                {
                    return PermissionLevels.ServerModerator;
                }
            }
            // Otherwise it allows access to regular commands
            return PermissionLevels.User;
        }
    }
}
