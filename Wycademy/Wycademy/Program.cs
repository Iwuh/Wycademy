using Discord;
using Discord.Commands;
using Discord.Commands.Permissions;
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
                x.PrefixChar = '<';
                x.HelpMode = HelpMode.Private;
            });

            //Set up message logging
            _client.MessageReceived += (s, e) =>
            {
                if (e.Message.IsAuthor)
                {
                    _client.Log.Info(">>Message", $"{((e.Server != null) ? e.Server.Name : "Private")}/#{((!e.Channel.IsPrivate) ? e.Channel.Name : "")} by {e.User.Name}");
                }
                else if (!e.Message.IsAuthor)
                {
                    _client.Log.Info("<<Message", $"{((e.Server != null) ? e.Server.Name : "Private")}/#{((!e.Channel.IsPrivate) ? e.Channel.Name : "")} by {e.User.Name}");
                }
            };

            //Bot token is stored in an environment variable so that nobody sees it when I push to GitHub :D
            string token = Environment.GetEnvironmentVariable("WYCADEMY_TOKEN", EnvironmentVariableTarget.User);

            //Connect to all guilds that the bot is part of and then block until the bot is manually exited.
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token);
            });
        }

        //Whenever we log data, this method is called.
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}] {e.Message}");
        }
    }
}
