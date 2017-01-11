using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    [Summary("Stats Commands")]
    public class StatsModule : ModuleBase
    {
        private LockerService _locker;
        private MonsterInfoService _moninfo;
        private MotionValueService _mv;
        private CommandCacheService _cache;
        private UtilityService _rand;

        public StatsModule(LockerService ls, MonsterInfoService mis, MotionValueService mvs, CommandCacheService ccs, UtilityService rns)
        {
            _locker = ls;
            _moninfo = mis;
            _mv = mvs;
            _cache = ccs;
            _rand = rns;
        }

        [Command("stats")]
        [Summary("Gets statistics about the bot.")]
        [RequireUnlocked]
        public async Task GetStatistics()
        {
            EmbedBuilder statsEmbed;
            using (Process p = Process.GetCurrentProcess())
            {
                statsEmbed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { Name = "Wycademy", IconUrl = "https://discordapp.com/api/users/207172354101608448/avatars/67bb079bde2e9ed142ad824e4a31d5af.jpg", Url = @"https://github.com/Iwuh/Wycademy" })

                .WithColor(new Color((byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256)))

                .WithTitle("Statistics about the Wycademy:")

                .AddField(x => x.WithName("Uptime:").WithValue(GetUptime()).WithIsInline(true))

                .AddField(x => x.WithName("Queries:").WithValue($"{_moninfo.Queries + _mv.Queries}").WithIsInline(true))

                .AddField(x => x.WithName("Cached Messages:").WithValue($"{_cache.Count} / {_cache.MaxCapacity}").WithIsInline(true))

                .AddField(x => x.WithName("Connected Servers").WithValue((Context.Client as DiscordSocketClient).Guilds.Count.ToString()).WithIsInline(true))

                .AddField(x => x.WithName("Memory Use:").WithValue((p.PrivateMemorySize64 / 1024.0f / 1024.0f).ToString() + " MB").WithIsInline(true))

                .AddField(x => x.WithName("Total Users:").WithValue((Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString()).WithIsInline(true))

                .WithFooter(new EmbedFooterBuilder() { Text = DateTime.Now.ToUniversalTime().ToString("R") });
            }

            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, embed: statsEmbed);
        }

        [Command("about")]
        [Summary("Gets information about the bot.")]
        public async Task GetInfo()
        {
            var appInfo = await Context.Client.GetApplicationInfoAsync();

            EmbedBuilder aboutEmbed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder() { Name = "Wycademy", IconUrl = "https://discordapp.com/api/users/207172354101608448/avatars/67bb079bde2e9ed142ad824e4a31d5af.jpg", Url = @"https://github.com/Iwuh/Wycademy" })

                .WithColor(new Color((byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256)))

                .WithTitle("About the Wycademy")

                .WithDescription("Wycademy is a Monster Hunter info bot, to help you with your hunting without you needing to leave Discord! Check out what it can do by using the `<help` command." +
                " You can invite it to your server [here](https://discordapp.com/oauth2/authorize?client_id=207172340809859072&scope=bot&permissions=52224) and view the source code [here](https://github.com/Iwuh/Wycademy)." +
                " If you enjoy the bot, a star would be much appreciated! The icon was created by [@thechewer on Instagram](https://www.instagram.com/p/BH42dbZjiYt/?taken-by=thechewer) and the data is taken from Kiranico." +
                " Special thanks to my brother for helping me gather the monster data. Monster Hunter is © CAPCOM.")

                .AddField(x => x.WithName("Author:").WithValue($"{appInfo.Owner} (ID: {appInfo.Owner.Id})").WithIsInline(true))

                .AddField(x => x.WithName("Library:").WithValue($"Discord.Net version {DiscordConfig.Version}").WithIsInline(true))

                .AddField(x => x.WithName("Runtime:").WithValue($"{RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}"))

                .WithFooter(new EmbedFooterBuilder() { Text = DateTime.Now.ToUniversalTime().ToString("R") });

            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, embed: aboutEmbed);
        }

        private string GetUptime()
        {
            TimeSpan uptime = DateTime.Now - _rand.StartTime;

            return uptime.ToString(@"dd\.hh\:mm\:ss\:fff");
        }
    }
}
