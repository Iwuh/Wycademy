using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Preconditions;
using WycademyV2.Commands.Services;

namespace WycademyV2.Commands.Modules
{
    public class StatsModule : ModuleBase
    {
        private LockerService _locker;
        private MonsterInfoService _moninfo;
        private MotionValueService _mv;
        private CommandCacheService _cache;
        private RandomNumberService _rand;

        public StatsModule(LockerService ls, MonsterInfoService mis, MotionValueService mvs, CommandCacheService ccs, RandomNumberService rns)
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
                .WithAuthor(new EmbedAuthorBuilder() { Name = "Wycademy", IconUrl = "https://discordapp.com/api/users/207172354101608448/avatars/67bb079bde2e9ed142ad824e4a31d5af.jpg", Url = @"https://i.ytimg.com/vi/IRMFU3ZSvTc/hqdefault.jpg" })

                .WithColor(new Color((byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256), (byte)_rand.GetRandomNumber(0, 256)))

                .WithTitle("Statistics about the Wycademy:")

                .AddField(x => x.WithName("Uptime:").WithValue(GetUptime()).WithIsInline(true))

                .AddField(x => x.WithName("Queries:").WithValue($"{_moninfo.Queries + _mv.Queries}").WithIsInline(true))

                .AddField(x => x.WithName("Cached Messages:").WithValue($"{_cache.Count} / {_cache.MaxCapacity}").WithIsInline(true))

                .AddField(async x => x.WithName("Connected Servers").WithValue((await Context.Client.GetGuildsAsync()).Count().ToString()).WithIsInline(true))

                .AddField(x => x.WithName("Memory Use:").WithValue((p.PrivateMemorySize64 / 1024.0f / 1024.0f).ToString() + " MB").WithIsInline(true))

                .WithFooter(new EmbedFooterBuilder() { Text = DateTime.Now.ToUniversalTime().ToString("R") });
            }

            await Context.Channel.SendCachedMessageAsync(Context.Message.Id, _cache, embed: statsEmbed);
        }

        private string GetUptime()
        {
            TimeSpan uptime = DateTime.Now - WycademyConst.START_TIME;

            return uptime.ToString(@"dd\.hh\:mm\:ss\:fff");
        }
    }
}
